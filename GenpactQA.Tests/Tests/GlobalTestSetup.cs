using GenpactQA.Tests.Core;
using NUnit.Framework;

namespace GenpactQA.Tests.Tests;

[SetUpFixture]
public class GlobalTestSetup
{
    [OneTimeSetUp]
    public void GlobalSetup()
    {
        // Initialize ExtentReports
        ExtentReportManager.GetInstance();
    }

    [OneTimeTearDown]
    public void GlobalTeardown()
    {
        // Flush and save the report
        ExtentReportManager.Flush();
        
        var reportPath = Path.Combine(
            Directory.GetCurrentDirectory().Contains("bin") 
                ? Directory.GetCurrentDirectory().Substring(0, Directory.GetCurrentDirectory().IndexOf("bin"))
                : Directory.GetCurrentDirectory(),
            "TestResults", "HtmlReport", "TestReport.html");
            
        Console.WriteLine($"\n========================================");
        Console.WriteLine($"HTML Report Generated: {reportPath}");
        Console.WriteLine($"========================================\n");
    }
}
