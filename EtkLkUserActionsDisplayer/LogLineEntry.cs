using System.Text.RegularExpressions;

public record LogLineEntry(DateTime DateTimeUtc, string User, string Url)
{
    static readonly string RegexPattern = "^(?<datetime>\\d{4}-\\d{2}-\\d{2} \\d{2}:\\d{2}:\\d{2}\\.\\d+)\\|[^|]*\\|[^|]*\\|User \"(?<user>[^\"]+)\" opened page: \"(?<url>[^\"]+)\"";
    internal static bool TryParse(string? line, out LogLineEntry? entry)
    {
        var match = Regex.Match(line, RegexPattern);
        if (match.Success && DateTime.TryParse(match.Groups["datetime"].Value, out var dt))
        {
            entry = new LogLineEntry(dt, match.Groups["user"].Value, match.Groups["url"].Value);
            return true;
        }

        entry = null;
        return false;
    }
}