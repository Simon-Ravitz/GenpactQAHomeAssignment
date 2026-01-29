using GenpactQA.Tests.Pages;
using Microsoft.Playwright;
using NUnit.Framework;

namespace GenpactQA.Tests.Tests;

[TestFixture]
[Parallelizable(ParallelScope.Self)]
public class Task3_ColorThemeTests
{
    [Test]
    public async Task ColorBeta_ChangeToDark_ThemeActuallyChanged()
    {
        await using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();

        var wikiPage = new WikipediaPlaywrightPage(page);
        await wikiPage.GotoAsync();

        // Open Appearance (right sidebar) and set Color to Dark
        await wikiPage.OpenAppearanceAndSetColorToDarkAsync();

        // Validate that the color actually changed
        var isDark = await wikiPage.IsDarkThemeActiveAsync();
        Assert.That(isDark, Is.True, "Expected Dark theme to be active after selecting Dark in Color (beta).");
    }
}
