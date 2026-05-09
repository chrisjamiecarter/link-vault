using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace LinkVault.Web.Blazor.Exceptions;

public sealed class ApiException(HttpStatusCode statusCode, ProblemDetails problem)
    : Exception(problem.Detail ?? problem.Title ?? "An unexpected error occurred.")
{
    public HttpStatusCode StatusCode { get; } = statusCode;
    public ProblemDetails Problem { get; } = problem;
}
