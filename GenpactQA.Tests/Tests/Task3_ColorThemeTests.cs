using GenpactQA.Tests.Core;
using GenpactQA.Tests.Pages;
using Microsoft.Playwright;
using NUnit.Framework;

namespace GenpactQA.Tests.Tests;

[TestFixture]
[Parallelizable(ParallelScope.Self)]
public class Task3_ColorThemeTests : ExtentTestBase
{
    [Test]
    public async Task ColorBeta_ChangeToDark_ThemeActuallyChanged()
    {
        LogInfo("Starting Color Theme Dark Mode Test");
        
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();

        var wikiPage = new WikipediaPlaywrightPage(page);
        await wikiPage.GotoAsync();
        LogInfo("Navigated to Wikipedia Playwright page");

        // Open Appearance (right sidebar) and set Color to Dark
        await wikiPage.OpenAppearanceAndSetColorToDarkAsync();
        LogInfo("Set color theme to Dark in Appearance settings");

        // Validate that the color actually changed
        var isDark = await wikiPage.IsDarkThemeActiveAsync();
        LogInfo($"Dark theme active: {isDark}");
        
        Assert.That(isDark, Is.True, "Expected Dark theme to be active after selecting Dark in Color (beta).");
        
        LogPass("Dark theme successfully activated and validated");
    }
}
