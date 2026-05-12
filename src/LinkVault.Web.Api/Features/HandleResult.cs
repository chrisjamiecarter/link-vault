namespace LinkVault.Web.Api.Features;

public abstract record HandleResult<T>
{
    public sealed record Success(T Value) : HandleResult<T>;
    public sealed record NotFound(string Detail) : HandleResult<T>;
    public sealed record ValidationFailed(IDictionary<string, string[]> Errors) : HandleResult<T>;
    public sealed record Conflict(string Detail) : HandleResult<T>;
    public sealed record Created(T Value, string Location) : HandleResult<T>;
}
