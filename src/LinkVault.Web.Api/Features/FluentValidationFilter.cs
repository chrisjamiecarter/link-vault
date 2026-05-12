using FluentValidation;

namespace LinkVault.Web.Api.Features;

public sealed class FluentValidationFilter<T>(IValidator<T> validator)
    : IEndpointFilter
    where T : class
{
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var argument = context.Arguments.OfType<T>().FirstOrDefault();
        if (argument is null)
        {
            return TypedResults.BadRequest("Request body is required");
        }

        var result = await validator.ValidateAsync(argument);
        if (!result.IsValid)
        {
            return TypedResults.ValidationProblem(result.ToDictionary());
        }

        return await next(context);
    }
}
