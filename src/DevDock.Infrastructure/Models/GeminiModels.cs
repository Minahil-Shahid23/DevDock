using System.Text.Json.Serialization;

namespace DevDock.Infrastructure.Models;

public class GeminiRequest
{
    [JsonPropertyName("contents")]
    public List<GeminiContent> Contents { get; set; } = new();
}

public class GeminiContent
{
    [JsonPropertyName("parts")]
    public List<GeminiPart> Parts { get; set; } = new();
}

public class GeminiPart
{
    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;
}

public class GeminiResponse
{
    [JsonPropertyName("candidates")]
    public List<GeminiCandidate>? Candidates { get; set; }
}

public class GeminiCandidate
{
    [JsonPropertyName("content")]
    public GeminiContent? Content { get; set; }
}

// AI se structured JSON parse karne ke liye
public class AiReviewResult
{
    [JsonPropertyName("bugCount")]
    public int BugCount { get; set; }

    [JsonPropertyName("securityIssueCount")]
    public int SecurityIssueCount { get; set; }

    [JsonPropertyName("performanceIssueCount")]
    public int PerformanceIssueCount { get; set; }

    [JsonPropertyName("suggestions")]
    public List<string> Suggestions { get; set; } = new();
}