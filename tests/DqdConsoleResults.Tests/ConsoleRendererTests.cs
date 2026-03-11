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
}
