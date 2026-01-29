using GenpactQA.Tests.Core;
using GenpactQA.Tests.Pages;
using Microsoft.Playwright;
using NUnit.Framework;

namespace GenpactQA.Tests.Tests;

[TestFixture]
[Parallelizable(ParallelScope.Self)]
public class Task2_MicrosoftDevelopmentToolsLinksTests : ExtentTestBase
{
    [Test]
    public async Task MicrosoftDevelopmentToolsSection_AllTechnologyNames_AreTextLinks()
    {
        LogInfo("Starting Microsoft Development Tools Links Test");
        
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();

        var wikiPage = new WikipediaPlaywrightPage(page);
        await wikiPage.GotoAsync();
        LogInfo("Navigated to Wikipedia Playwright page");

        var (allAreLinks, notLinkNames) = await wikiPage.ValidateMicrosoftDevelopmentToolsTechnologyNamesAreLinksAsync();
        LogInfo($"Validation complete - All are links: {allAreLinks}");
        
        if (!allAreLinks)
        {
            LogWarning($"Found non-link technology names: {string.Join(", ", notLinkNames)}");
        }

        Assert.That(allAreLinks, Is.True,
            $"The following technology names under Microsoft development tools are NOT text links: {string.Join(", ", notLinkNames)}");
        
        LogPass("All technology names are properly linked");
    }
}
