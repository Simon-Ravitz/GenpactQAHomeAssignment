using System.Text.Json;
using System.Text.Json.Nodes;

namespace GenpactQA.Tests.Api;

/// <summary>
/// Client for Wikipedia MediaWiki Parse API to fetch section content.
/// </summary>
public class MediaWikiApiClient
{
    private readonly HttpClient _http;
    private const string ApiBase = "https://en.wikipedia.org/w/api.php";

    public MediaWikiApiClient(HttpClient? httpClient = null)
    {
        _http = httpClient ?? new HttpClient();
        _http.DefaultRequestHeaders.Add("User-Agent", "GenpactQA-Playwright/1.0");
    }

    /// <summary>
    /// Gets the section index for "Debugging features" (or by title).
    /// </summary>
    public async Task<int?> GetSectionIndexAsync(string pageTitle, string sectionTitle, CancellationToken ct = default)
    {
        var url = $"{ApiBase}?format=json&action=parse&prop=sections&page={Uri.EscapeDataString(pageTitle)}";
        var json = await _http.GetStringAsync(url, ct).ConfigureAwait(false);
        var doc = JsonNode.Parse(json);
        var sections = doc?["parse"]?["sections"]?.AsArray();
        if (sections == null)
            return null;

        foreach (var sec in sections)
        {
            var line = sec?["line"]?.GetValue<string>();
            if (string.Equals(line, sectionTitle, StringComparison.OrdinalIgnoreCase))
            {
                var indexNode = sec?["index"];
                if (indexNode == null) return null;
                var s = indexNode.GetValue<string>();
                return int.TryParse(s, out var idx) ? idx : null;
            }
        }

        return null;
    }

    /// <summary>
    /// Gets the parsed HTML text of a section by index.
    /// </summary>
    public async Task<string> GetSectionTextAsync(string pageTitle, int sectionIndex, CancellationToken ct = default)
    {
        var url = $"{ApiBase}?format=json&action=parse&page={Uri.EscapeDataString(pageTitle)}&prop=text&section={sectionIndex}";
        var json = await _http.GetStringAsync(url, ct).ConfigureAwait(false);
        var doc = JsonNode.Parse(json);
        var text = doc?["parse"]?["text"]?["*"]?.GetValue<string>();
        return text ?? string.Empty;
    }

    /// <summary>
    /// Gets the "Debugging features" section content via Parse API.
    /// </summary>
    public async Task<string> GetDebuggingFeaturesSectionAsync(string pageTitle = "Playwright_(software)", CancellationToken ct = default)
    {
        var index = await GetSectionIndexAsync(pageTitle, "Debugging features", ct).ConfigureAwait(false);
        if (index == null)
            return string.Empty;
        return await GetSectionTextAsync(pageTitle, index.Value, ct).ConfigureAwait(false);
    }
}
