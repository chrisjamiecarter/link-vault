namespace LinkVault.Web.Api.RateLimiters;

public sealed class RateLimiterOptions
{
    public const string Key = "RateLimiters";

    public int Limit { get; init; } = 5;

    public TimeSpan Window { get; init; } = TimeSpan.FromSeconds(10);
}
