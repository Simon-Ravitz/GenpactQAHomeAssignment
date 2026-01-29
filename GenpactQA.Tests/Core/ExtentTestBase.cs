using AventStack.ExtentReports;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace GenpactQA.Tests.Core;

public class ExtentTestBase
{
    protected ExtentTest? test;

    [SetUp]
    public void TestSetup()
    {
        var testName = TestContext.CurrentContext.Test.Name;
        var className = TestContext.CurrentContext.Test.ClassName;
        var extent = ExtentReportManager.GetInstance();
        test = extent.CreateTest(testName, className);
    }

    [TearDown]
    public void TestTeardown()
    {
        var outcome = TestContext.CurrentContext.Result.Outcome.Status;
        var message = TestContext.CurrentContext.Result.Message;
        var stackTrace = TestContext.CurrentContext.Result.StackTrace;

        switch (outcome)
        {
            case TestStatus.Failed:
                test?.Fail($"Test Failed: {message}");
                if (!string.IsNullOrEmpty(stackTrace))
                {
                    test?.Fail($"<pre>{stackTrace}</pre>");
                }
                break;
            case TestStatus.Passed:
                test?.Pass("Test Passed");
                break;
            case TestStatus.Skipped:
                test?.Skip($"Test Skipped: {message}");
                break;
            case TestStatus.Inconclusive:
                test?.Warning($"Test Inconclusive: {message}");
                break;
        }
    }

    protected void LogInfo(string message)
    {
        test?.Info(message);
    }

    protected void LogPass(string message)
    {
        test?.Pass(message);
    }

    protected void LogFail(string message)
    {
        test?.Fail(message);
    }

    protected void LogWarning(string message)
    {
        test?.Warning(message);
    }
}
