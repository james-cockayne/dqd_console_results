namespace DqdConsoleResults.Tests;

public class ConsoleRendererTests
{
    [Fact]
    public void FormatSql_ReplacesCarriageReturnLineFeed()
    {
        string input = "SELECT\r\n    *\r\nFROM\r\n    table1\r\n";
        string expected = "SELECT\n    *\nFROM\n    table1";

        Assert.Equal(expected, ConsoleRenderer.FormatSql(input));
    }

    [Fact]
    public void FormatSql_ReplacesCarriageReturnOnly()
    {
        string input = "SELECT\r    *\rFROM table1\r";
        string expected = "SELECT\n    *\nFROM table1";

        Assert.Equal(expected, ConsoleRenderer.FormatSql(input));
    }

    [Fact]
    public void FormatSql_TrimsTrailingWhitespace()
    {
        Assert.Equal("SELECT 1", ConsoleRenderer.FormatSql("SELECT 1   "));
    }

    [Theory]
    [InlineData(TestOutcome.Pass, "SUCCESS")]
    [InlineData(TestOutcome.Fail, "FAIL")]
    [InlineData(TestOutcome.Skip, "SKIP")]
    [InlineData(TestOutcome.Suppressed, "SUPPRESSED")]
    public void FormatStatus_ReturnsCorrectString(TestOutcome outcome, string expected)
    {
        Assert.Equal(expected, ConsoleRenderer.FormatStatus(outcome));
    }

    [Fact]
    public void Render_SuppressedTestThatPassed_PrintsYellowWarning()
    {
        var results = new List<ProcessedResult>
        {
            new() { CheckId = "test1", Outcome = TestOutcome.Pass, ExecutionTime = "1s", QueryText = "", PctViolatedRows = 0, ThresholdValue = 0 }
        };
        var suppressed = new HashSet<string> { "test1" };

        var output = CaptureConsoleOutput(() => ConsoleRenderer.Render(results, suppressed));

        Assert.Contains("Warning: test1 is suppressed but passed. Consider removing it from the suppression list.", output);
    }

    [Fact]
    public void Render_SuppressedTestThatFailed_DoesNotPrintWarning()
    {
        var results = new List<ProcessedResult>
        {
            new() { CheckId = "test1", Outcome = TestOutcome.Suppressed, ExecutionTime = "1s", QueryText = "", PctViolatedRows = 0, ThresholdValue = 0 }
        };
        var suppressed = new HashSet<string> { "test1" };

        var output = CaptureConsoleOutput(() => ConsoleRenderer.Render(results, suppressed));

        Assert.DoesNotContain("Warning:", output);
    }

    [Fact]
    public void Render_PassingTestNotSuppressed_DoesNotPrintWarning()
    {
        var results = new List<ProcessedResult>
        {
            new() { CheckId = "test1", Outcome = TestOutcome.Pass, ExecutionTime = "1s", QueryText = "", PctViolatedRows = 0, ThresholdValue = 0 }
        };
        var suppressed = new HashSet<string>();

        var output = CaptureConsoleOutput(() => ConsoleRenderer.Render(results, suppressed));

        Assert.DoesNotContain("Warning:", output);
    }

    [Fact]
    public void Render_MultipleSuppressedPasses_PrintsWarningForEach()
    {
        var results = new List<ProcessedResult>
        {
            new() { CheckId = "test1", Outcome = TestOutcome.Pass, ExecutionTime = "1s", QueryText = "", PctViolatedRows = 0, ThresholdValue = 0 },
            new() { CheckId = "test2", Outcome = TestOutcome.Pass, ExecutionTime = "1s", QueryText = "", PctViolatedRows = 0, ThresholdValue = 0 },
            new() { CheckId = "test3", Outcome = TestOutcome.Fail, ExecutionTime = "1s", QueryText = "SELECT 1", PctViolatedRows = 0, ThresholdValue = 0 }
        };
        var suppressed = new HashSet<string> { "test1", "test2" };

        var output = CaptureConsoleOutput(() => ConsoleRenderer.Render(results, suppressed));

        Assert.Contains("Warning: test1 is suppressed but passed.", output);
        Assert.Contains("Warning: test2 is suppressed but passed.", output);
    }

    private static string CaptureConsoleOutput(Action action)
    {
        var originalOut = Console.Out;
        using var writer = new StringWriter();
        Console.SetOut(writer);
        try
        {
            action();
            return writer.ToString();
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }
}
