using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;

namespace GenpactQA.Tests.Core;

public static class ExtentReportManager
{
    private static ExtentReports? _extent;
    private static readonly object _lock = new();

    public static ExtentReports GetInstance()
    {
        if (_extent == null)
        {
            lock (_lock)
            {
                if (_extent == null)
                {
                    var reportPath = Path.Combine(GetReportDirectory(), "TestReport.html");
                    var htmlReporter = new ExtentSparkReporter(reportPath);
                    
                    htmlReporter.Config.DocumentTitle = "Genpact QA Test Report";
                    htmlReporter.Config.ReportName = "Playwright Automation Test Results";
                    htmlReporter.Config.Theme = AventStack.ExtentReports.Reporter.Config.Theme.Standard;
                    
                    _extent = new ExtentReports();
                    _extent.AttachReporter(htmlReporter);
                    
                    // Add system info
                    _extent.AddSystemInfo("OS", Environment.OSVersion.ToString());
                    _extent.AddSystemInfo(".NET Version", Environment.Version.ToString());
                    _extent.AddSystemInfo("User", Environment.UserName);
                    _extent.AddSystemInfo("Framework", "NUnit + Playwright");
                }
            }
        }
        return _extent;
    }

    public static void Flush()
    {
        _extent?.Flush();
    }

    private static string GetReportDirectory()
    {
        // Get project root directory (go up from bin/Debug/net10.0)
        var currentDir = Directory.GetCurrentDirectory();
        var projectRoot = currentDir;
        
        // If running from bin directory, navigate to project root
        if (currentDir.Contains("bin"))
        {
            var binIndex = currentDir.IndexOf("bin");
            projectRoot = currentDir.Substring(0, binIndex);
        }
        
        var reportDir = Path.Combine(projectRoot, "TestResults", "HtmlReport");
        Directory.CreateDirectory(reportDir);
        return reportDir;
    }
}
