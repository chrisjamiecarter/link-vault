using System.Reflection;

namespace LinkVault.Web.Api;

internal static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
