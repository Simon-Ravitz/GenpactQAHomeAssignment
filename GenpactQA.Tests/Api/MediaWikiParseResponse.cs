using System.Text.Json.Serialization;

namespace GenpactQA.Tests.Api;

public class MediaWikiParseResponse
{
    [JsonPropertyName("parse")]
    public ParseResult? Parse { get; set; }
}

public class ParseResult
{
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("sections")]
    public List<SectionInfo>? Sections { get; set; }

    [JsonPropertyName("text")]
    public TextResult? Text { get; set; }
}

public class SectionInfo
{
    [JsonPropertyName("index")]
    public string? Index { get; set; }

    [JsonPropertyName("line")]
    public string? Line { get; set; }

    [JsonPropertyName("anchor")]
    public string? Anchor { get; set; }
}

public class TextResult
{
    [JsonPropertyName("*")]
    public string? Html { get; set; }
}
