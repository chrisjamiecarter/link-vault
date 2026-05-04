using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace LinkVault.Web.Api.RateLimiters;

internal sealed partial class SlidingWindowRateLimiter(
    IConnectionMultiplexer redis,
    ILogger<SlidingWindowRateLimiter> logger,
    IOptions<RateLimiterOptions> options)
    : IRateLimiter
{
    private const string RedisKeyPrefix = "rate_limit:sliding:";

    private readonly IDatabase _redis = redis.GetDatabase() ?? throw new InvalidOperationException("Failed to get Redis database.");
    private readonly ILogger<SlidingWindowRateLimiter> _logger = logger;
    private readonly int _limit = options.Value.Limit;
    private readonly TimeSpan _window = options.Value.Window;

    public async Task<bool> IsAllowedAsync(string key)
    {
        var redisKey = $"{RedisKeyPrefix}{key}";
        var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var windowStart = now - (long)_window.TotalMilliseconds;

        await _redis.SortedSetRemoveRangeByScoreAsync(redisKey, 0, windowStart);

        var currentCount = await _redis.SortedSetLengthAsync(redisKey);

        if (currentCount >= _limit)
        {
            LogRateLimitExceeded(key, currentCount, _limit);
            return false;
        }

        var requestId = $"{now}-{Guid.CreateVersion7()}";
        await _redis.SortedSetAddAsync(redisKey, requestId, now);

        await _redis.KeyExpireAsync(redisKey, _window);

        return true;
    }

    [LoggerMessage(LogLevel.Warning, "Rate limit exceeded for key: {key}. Count: {count}, Limit: {limit}")]
    private partial void LogRateLimitExceeded(string key, long count, int limit);
}
