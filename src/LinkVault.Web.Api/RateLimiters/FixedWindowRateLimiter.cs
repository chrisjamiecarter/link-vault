using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace LinkVault.Web.Api.RateLimiters;

internal sealed partial class FixedWindowRateLimiter(
    IConnectionMultiplexer redis,
    ILogger<FixedWindowRateLimiter> logger,
    IOptions<RateLimiterOptions> options)
    : IRateLimiter
{
    private const string RedisKeyPrefix = "rate_limit:fixed:";

    private readonly IDatabase _redis = redis.GetDatabase() ?? throw new InvalidOperationException("Failed to get Redis database.");
    private readonly ILogger<FixedWindowRateLimiter> _logger = logger;
    private readonly int _limit = options.Value.Limit;
    private readonly TimeSpan _window = options.Value.Window;

    public async Task<bool> IsAllowedAsync(string key)
    {
        var redisKey = $"{RedisKeyPrefix}{key}";

        var count = await _redis.StringIncrementAsync(redisKey);

        if (count == 1)
        {
            await _redis.KeyExpireAsync(redisKey, _window);
        }

        if (count > _limit)
        {
            LogRateLimitExceeded(key, count, _limit);
            return false;
        }

        return true;
    }

    [LoggerMessage(LogLevel.Warning, "Rate limit limit exceeded for key: {key}. Count: {count}, Limit: {limit}")]
    private partial void LogRateLimitExceeded(string key, long count, int limit);
}
