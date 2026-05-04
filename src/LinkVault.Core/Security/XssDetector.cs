using System.Text.RegularExpressions;

namespace LinkVault.Core.Security;

public static partial class XssDetector
{
    [GeneratedRegex("data:", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex DataUriPattern();

    [GeneratedRegex(@"\s+on\w+\s*=", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex EventHandlerPattern();

    [GeneratedRegex(@"<(img|svg|iframe|body|input|button|a|form|object|embed|video|audio|marquee|details|keygen|textarea|select)", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex HtmlTagPattern();

    [GeneratedRegex("javascript:", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex JavaScriptProtocolPattern();

    [GeneratedRegex(@"[""'][^""']*\s+on\w+\s*=", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex QuotedAttributeEventPattern();

    [GeneratedRegex("<script", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex ScriptTagPattern();

    public static string? GetDetectionReason(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return null;
        }

        if (ScriptTagPattern().IsMatch(input))
        {
            return "Contains <script> tags";
        }

        if (JavaScriptProtocolPattern().IsMatch(input))
        {
            return "Contains javascript: protocol";
        }

        if (DataUriPattern().IsMatch(input))
        {
            return "Contains data: URI";
        }

        if (HtmlTagPattern().IsMatch(input))
        {
            return "Contains HTML tags (img, svg, iframe, etc.)";
        }

        if (EventHandlerPattern().IsMatch(input))
        {
            return "Contains event handlers (onclick, onerror, etc.)";
        }

        if (QuotedAttributeEventPattern().IsMatch(input))
        {
            return "Contains event handlers in attributes";
        }

        return null;
    }

    public static bool IsUnsafe(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return false;
        }

        return ContainsXssPattern(input);
    }

    private static bool ContainsXssPattern(string input)
    {
        // Check for script tags.
        if (ScriptTagPattern().IsMatch(input))
        {
            return true;
        }

        // Check for javascript: protocol.
        if (JavaScriptProtocolPattern().IsMatch(input))
        {
            return true;
        }

        // Check for data: URIs.
        if (DataUriPattern().IsMatch(input))
        {
            return true;
        }

        // Check for HTML tags (img, svg, iframe, etc.).
        if (HtmlTagPattern().IsMatch(input))
        {
            return true;
        }

        // Check for event handlers (onclick, onerror, onload, etc.).
        if (EventHandlerPattern().IsMatch(input))
        {
            return true;
        }

        // Check for event handlers in quoted attributes.
        if (QuotedAttributeEventPattern().IsMatch(input))
        {
            return true;
        }

        return false;
    }
}
