using System.Diagnostics.CodeAnalysis;

namespace ogybot.Utility.Extensions;

public static class StringExtensions
{
    public static bool IsNullOrWhitespace([NotNullWhen(false)] this string? str)
    {
        return string.IsNullOrWhiteSpace(str);
    }
}