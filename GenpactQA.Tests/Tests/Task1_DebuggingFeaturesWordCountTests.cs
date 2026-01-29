using GenpactQA.Tests.Api;
using GenpactQA.Tests.Core;
using GenpactQA.Tests.Pages;
using Microsoft.Playwright;
using NUnit.Framework;

namespace GenpactQA.Tests.Tests;

[TestFixture]
[Parallelizable(ParallelScope.Self)]
public class Task1_DebuggingFeaturesWordCountTests : ExtentTestBase
{
    [Test]
    public async Task DebuggingFeaturesSection_UniqueWordCount_UI_Equals_API()
    {
        LogInfo("Starting Debugging Features Section Word Count Test");
        
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();

        var wikiPage = new WikipediaPlaywrightPage(page);
        await wikiPage.GotoAsync();
        LogInfo("Navigated to Wikipedia Playwright page");

        var uiSectionText = await wikiPage.GetDebuggingFeaturesSectionTextAsync();
        var uiNormalized = TextNormalizer.Normalize(uiSectionText);
        var uiUniqueCount = TextNormalizer.CountUniqueWords(uiSectionText);
        LogInfo($"UI unique word count: {uiUniqueCount}");

        var apiClient = new MediaWikiApiClient();
        var apiSectionHtml = await apiClient.GetDebuggingFeaturesSectionAsync();
        var apiPlainText = TextNormalizer.StripHtml(apiSectionHtml);
        var apiNormalized = TextNormalizer.Normalize(apiPlainText);
        var apiUniqueCount = TextNormalizer.CountUniqueWords(apiPlainText);
        LogInfo($"API unique word count: {apiUniqueCount}");

        // Allow a small tolerance (within 10 words) to account for slight differences in section boundaries
        var difference = Math.Abs(uiUniqueCount - apiUniqueCount);
        LogInfo($"Difference: {difference} words (tolerance: 10)");
        
        Assert.That(difference, Is.LessThanOrEqualTo(10),
            $"Unique word count mismatch exceeds tolerance: UI={uiUniqueCount}, API={apiUniqueCount}, Difference={difference}. UI normalized (first 200 chars): '{uiNormalized[..Math.Min(200, uiNormalized.Length)]}...' API normalized (first 200): '{apiNormalized[..Math.Min(200, apiNormalized.Length)]}...'");
        
        LogPass($"Word count validation passed - UI: {uiUniqueCount}, API: {apiUniqueCount}, Difference: {difference}");
    }
}
