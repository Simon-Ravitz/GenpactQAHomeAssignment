using Microsoft.Playwright;
using NUnit.Framework;

namespace GenpactQA.Tests;

[SetUpFixture]
public class PlaywrightSetup
{
    [OneTimeSetUp]
    public void InstallBrowsers()
    {
        var exitCode = Microsoft.Playwright.Program.Main(new[] { "install", "chromium" });
        if (exitCode != 0)
            throw new InvalidOperationException($"Playwright install exited with code {exitCode}");
    }
}
