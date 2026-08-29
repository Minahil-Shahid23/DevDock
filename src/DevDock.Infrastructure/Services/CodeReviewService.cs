using System.Text;
using System.Text.Json;
using DevDock.Application.DTOs;
using DevDock.Application.Exceptions;
using DevDock.Application.Interfaces;
using DevDock.Domain.Entities;
using DevDock.Infrastructure.Models;
using DevDock.Infrastructure.Persistence;
using DevDock.Infrastructure.Settings;
using Microsoft.Extensions.Options;

namespace DevDock.Infrastructure.Services;

public class CodeReviewService : ICodeReviewService
{
    private readonly AppDbContext _context;
    private readonly HttpClient _httpClient;
    private readonly GeminiSettings _geminiSettings;

    public CodeReviewService(AppDbContext context, HttpClient httpClient, IOptions<GeminiSettings> geminiSettings)
    {
        _context = context;
        _httpClient = httpClient;
        _geminiSettings = geminiSettings.Value;
    }

    public async Task<CodeReviewResponseDto> ReviewCodeAsync(Guid userId, CodeReviewRequestDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Code))
            throw new AuthException("Code cannot be empty.");

        var prompt = BuildPrompt(dto.Code, dto.Language);
        var aiResult = await CallGeminiApiAsync(prompt);

        var codeReview = new CodeReview
        {
            UserId = userId,
            ProjectId = dto.ProjectId,
            Code = dto.Code,
            Language = dto.Language,
            BugCount = aiResult.BugCount,
            SecurityIssueCount = aiResult.SecurityIssueCount,
            PerformanceIssueCount = aiResult.PerformanceIssueCount,
            Suggestions = JsonSerializer.Serialize(aiResult.Suggestions)
        };

        _context.CodeReviews.Add(codeReview);
        await _context.SaveChangesAsync();

        return new CodeReviewResponseDto
        {
            Id = codeReview.Id,
            BugCount = codeReview.BugCount,
            SecurityIssueCount = codeReview.SecurityIssueCount,
            PerformanceIssueCount = codeReview.PerformanceIssueCount,
            Suggestions = aiResult.Suggestions,
            CreatedAt = codeReview.CreatedAt
        };
    }

    private static string BuildPrompt(string code, string language)
    {
        return $@"You are a senior code reviewer. Review the following {language} code and respond ONLY with valid JSON (no markdown, no code fences, no extra text) in exactly this format:
{{
  ""bugCount"": <number>,
  ""securityIssueCount"": <number>,
  ""performanceIssueCount"": <number>,
  ""suggestions"": [""short suggestion 1"", ""short suggestion 2""]
}}

Each suggestion should be one concise sentence mentioning the issue and a fix. If there are no issues in a category, set its count to 0.

Code to review:
````{language}
{code}
```";
    }

    private async Task<AiReviewResult> CallGeminiApiAsync(string prompt)
    {
        var url = $"https://generativelanguage.googleapis.com/v1beta/models/{_geminiSettings.Model}:generateContent?key={_geminiSettings.ApiKey}";

        var requestBody = new GeminiRequest
        {
            Contents = new List<GeminiContent>
            {
                new GeminiContent
                {
                    Parts = new List<GeminiPart> { new GeminiPart { Text = prompt } }
                }
            }
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(url, content);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            throw new AuthException($"AI review failed: {response.StatusCode} - {errorBody}");
        }

        var responseJson = await response.Content.ReadAsStringAsync();
        var geminiResponse = JsonSerializer.Deserialize<GeminiResponse>(responseJson);

        var aiText = geminiResponse?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text;

        if (string.IsNullOrWhiteSpace(aiText))
            throw new AuthException("AI did not return a valid response.");

        // AI kabhi kabhi ```json ... ``` markdown fences mein wrap kar deta hai, clean karte hain
        aiText = aiText.Trim();
        if (aiText.StartsWith("```"))
        {
            aiText = aiText.Replace("```json", "").Replace("```", "").Trim();
        }

        try
        {
            var result = JsonSerializer.Deserialize<AiReviewResult>(aiText, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            return result ?? new AiReviewResult();
        }
        catch (JsonException)
        {
            throw new AuthException("Failed to parse AI response as JSON.");
        }
    }
}
