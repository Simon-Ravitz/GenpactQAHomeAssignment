using GenpactQA.Tests.Pages;
using Microsoft.Playwright;
using NUnit.Framework;

namespace GenpactQA.Tests.Tests;

[TestFixture]
[Parallelizable(ParallelScope.Self)]
public class Task2_MicrosoftDevelopmentToolsLinksTests
{
    [Test]
    public async Task MicrosoftDevelopmentToolsSection_AllTechnologyNames_AreTextLinks()
    {
        await using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();

        var wikiPage = new WikipediaPlaywrightPage(page);
        await wikiPage.GotoAsync();

        // Scroll to Microsoft development tools (it's at the bottom)
        await page.GetByRole(AriaRole.Heading, new() { Name = "Microsoft development tools" }).ScrollIntoViewIfNeededAsync();

        var (allAreLinks, notLinkNames) = await wikiPage.ValidateMicrosoftDevelopmentToolsTechnologyNamesAreLinksAsync();

        Assert.That(allAreLinks, Is.True,
            $"The following technology names under Microsoft development tools are NOT text links: {string.Join(", ", notLinkNames)}");
    }
}
