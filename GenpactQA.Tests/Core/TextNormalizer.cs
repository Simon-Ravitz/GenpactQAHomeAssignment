using System.Text.RegularExpressions;

namespace GenpactQA.Tests.Core;

/// <summary>
/// Normalizes text for comparison: lowercase, remove non-word chars, collapse whitespace.
/// </summary>
public static class TextNormalizer
{
    private static readonly Regex NonWordRegex = new(@"[^\p{L}\p{N}\s]", RegexOptions.Compiled);
    private static readonly Regex WhitespaceRegex = new(@"\s+", RegexOptions.Compiled);

    /// <summary>
    /// Normalizes text: strip HTML, lowercase, remove punctuation, collapse spaces.
    /// </summary>
    public static string Normalize(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        var stripped = StripHtml(text);
        var lower = stripped.ToLowerInvariant();
        var noPunctuation = NonWordRegex.Replace(lower, " ");
        var singleSpaces = WhitespaceRegex.Replace(noPunctuation.Trim(), " ");
        return singleSpaces;
    }

    /// <summary>
    /// Strips HTML tags and decodes common entities.
    /// </summary>
    public static string StripHtml(string html)
    {
        if (string.IsNullOrWhiteSpace(html))
            return string.Empty;

        var text = Regex.Replace(html, @"<[^>]+>", " ");
        text = System.Net.WebUtility.HtmlDecode(text);
        return WhitespaceRegex.Replace(text, " ").Trim();
    }

    /// <summary>
    /// Returns the number of unique words (after normalization).
    /// </summary>
    public static int CountUniqueWords(string? text)
    {
        var normalized = Normalize(text);
        if (string.IsNullOrWhiteSpace(normalized))
            return 0;

        var words = normalized.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return words.Distinct().Count();
    }
}
