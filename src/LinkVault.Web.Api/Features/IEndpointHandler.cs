namespace LinkVault.Web.Api.Features;

public interface IHandler<TRequest, TResponse>
{
    Task<HandleResult<TResponse>> HandleAsync(TRequest request, CancellationToken ct);
}
