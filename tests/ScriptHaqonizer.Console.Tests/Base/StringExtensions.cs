using System.Text.RegularExpressions;

namespace ScriptHaqonizer.Console.Tests.Base;

internal static class StringExtensions
{
    public static string Escape(this string original)
    {
        if (string.IsNullOrEmpty(original))
        {
            return original;
        }

        var value = Regex.Replace(original, @"(\\*)" + "\"", @"$1\$0");
        value = Regex.Replace(value, @"^(.*\s.*?)(\\*)$", "\"$1$2$2\"");
        return value;
    }
}