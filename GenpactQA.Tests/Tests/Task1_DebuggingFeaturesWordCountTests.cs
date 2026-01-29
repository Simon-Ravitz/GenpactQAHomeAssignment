using GenpactQA.Tests.Api;
using GenpactQA.Tests.Core;
using GenpactQA.Tests.Pages;
using Microsoft.Playwright;
using NUnit.Framework;

namespace GenpactQA.Tests.Tests;

[TestFixture]
[Parallelizable(ParallelScope.Self)]
public class Task1_DebuggingFeaturesWordCountTests
{
    [Test]
    public async Task DebuggingFeaturesSection_UniqueWordCount_UI_Equals_API()
    {
        await using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();

        var wikiPage = new WikipediaPlaywrightPage(page);
        await wikiPage.GotoAsync();

        var uiSectionText = await wikiPage.GetDebuggingFeaturesSectionTextAsync();
        var uiNormalized = TextNormalizer.Normalize(uiSectionText);
        var uiUniqueCount = TextNormalizer.CountUniqueWords(uiSectionText);

        var apiClient = new MediaWikiApiClient();
        var apiSectionHtml = await apiClient.GetDebuggingFeaturesSectionAsync();
        var apiPlainText = TextNormalizer.StripHtml(apiSectionHtml);
        var apiNormalized = TextNormalizer.Normalize(apiPlainText);
        var apiUniqueCount = TextNormalizer.CountUniqueWords(apiPlainText);

        Assert.That(uiUniqueCount, Is.EqualTo(apiUniqueCount),
            $"Unique word count mismatch: UI={uiUniqueCount}, API={apiUniqueCount}. UI normalized (first 200 chars): '{uiNormalized[..Math.Min(200, uiNormalized.Length)]}...' API normalized (first 200): '{apiNormalized[..Math.Min(200, apiNormalized.Length)]}...'");
    }
}
