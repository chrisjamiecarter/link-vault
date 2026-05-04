namespace LinkVault.Web.Api.RateLimiters;

public interface IRateLimiter
{
    Task<bool> IsAllowedAsync(string key);
}
