namespace DqdConsoleResults.Tests;

public class DqdProcessorTests
{
    [Fact]
    public void DetermineOutcome_WhenFailed_ReturnsFail()
    {
        var check = new CheckResult { CheckId = "test1", Failed = 1, Passed = 0, IsError = 0, NotApplicable = 0 };
        Assert.Equal(TestOutcome.Fail, DqdProcessor.DetermineOutcome(check));
    }

    [Fact]
    public void DetermineOutcome_WhenIsError_ReturnsFail()
    {
        var check = new CheckResult { CheckId = "test1", Failed = 0, Passed = 0, IsError = 1, NotApplicable = 0 };
        Assert.Equal(TestOutcome.Fail, DqdProcessor.DetermineOutcome(check));
    }

    [Fact]
    public void DetermineOutcome_WhenFailedAndIsError_ReturnsFail()
    {
        var check = new CheckResult { CheckId = "test1", Failed = 1, Passed = 0, IsError = 1, NotApplicable = 0 };
        Assert.Equal(TestOutcome.Fail, DqdProcessor.DetermineOutcome(check));
    }

    [Fact]
    public void DetermineOutcome_WhenPassed_ReturnsPass()
    {
        var check = new CheckResult { CheckId = "test1", Failed = 0, Passed = 1, IsError = 0, NotApplicable = 0 };
        Assert.Equal(TestOutcome.Pass, DqdProcessor.DetermineOutcome(check));
    }

    [Fact]
    public void DetermineOutcome_WhenNotApplicable_ReturnsSkip()
    {
        var check = new CheckResult { CheckId = "test1", Failed = 0, Passed = 0, IsError = 0, NotApplicable = 1 };
        Assert.Equal(TestOutcome.Skip, DqdProcessor.DetermineOutcome(check));
    }

    [Fact]
    public void DetermineOutcome_WhenAllZero_Throws()
    {
        var check = new CheckResult { CheckId = "test1", Failed = 0, Passed = 0, IsError = 0, NotApplicable = 0 };
        Assert.Throws<InvalidOperationException>(() => DqdProcessor.DetermineOutcome(check));
    }

    [Fact]
    public void ProcessResults_SuppressesKnownFailures()
    {
        var results = new DqdResults
        {
            CheckResults = new List<CheckResult>
            {
                new() { CheckId = "test1", Failed = 1, Passed = 0, IsError = 0, NotApplicable = 0, ExecutionTime = "1s", QueryText = "SELECT 1" },
                new() { CheckId = "test2", Failed = 1, Passed = 0, IsError = 0, NotApplicable = 0, ExecutionTime = "1s", QueryText = "SELECT 2" }
            }
        };
        var suppressed = new HashSet<string> { "test1" };

        var processed = DqdProcessor.ProcessResults(results, suppressed);

        Assert.Equal(TestOutcome.Suppressed, processed[0].Outcome);
        Assert.Equal(TestOutcome.Fail, processed[1].Outcome);
    }

    [Fact]
    public void ProcessResults_PassingTestNotAffectedBySuppression()
    {
        var results = new DqdResults
        {
            CheckResults = new List<CheckResult>
            {
                new() { CheckId = "test1", Failed = 0, Passed = 1, IsError = 0, NotApplicable = 0, ExecutionTime = "1s", QueryText = "SELECT 1" }
            }
        };
        var suppressed = new HashSet<string> { "test1" };

        var processed = DqdProcessor.ProcessResults(results, suppressed);

        Assert.Equal(TestOutcome.Pass, processed[0].Outcome);
    }

    [Fact]
    public void ProcessResults_PreservesCheckIdAndExecutionTime()
    {
        var results = new DqdResults
        {
            CheckResults = new List<CheckResult>
            {
                new() { CheckId = "my_check", Failed = 0, Passed = 1, IsError = 0, NotApplicable = 0, ExecutionTime = "2.5 secs", QueryText = "SELECT 1" }
            }
        };

        var processed = DqdProcessor.ProcessResults(results, new HashSet<string>());

        Assert.Equal("my_check", processed[0].CheckId);
        Assert.Equal("2.5 secs", processed[0].ExecutionTime);
    }

    [Fact]
    public void HasNonSuppressedFailures_WithFailures_ReturnsTrue()
    {
        var results = new List<ProcessedResult>
        {
            new() { CheckId = "test1", Outcome = TestOutcome.Pass, ExecutionTime = "1s", QueryText = "" },
            new() { CheckId = "test2", Outcome = TestOutcome.Fail, ExecutionTime = "1s", QueryText = "" }
        };

        Assert.True(DqdProcessor.HasNonSuppressedFailures(results));
    }

    [Fact]
    public void HasNonSuppressedFailures_OnlySuppressed_ReturnsFalse()
    {
        var results = new List<ProcessedResult>
        {
            new() { CheckId = "test1", Outcome = TestOutcome.Pass, ExecutionTime = "1s", QueryText = "" },
            new() { CheckId = "test2", Outcome = TestOutcome.Suppressed, ExecutionTime = "1s", QueryText = "" }
        };

        Assert.False(DqdProcessor.HasNonSuppressedFailures(results));
    }

    [Fact]
    public void HasNonSuppressedFailures_AllPassed_ReturnsFalse()
    {
        var results = new List<ProcessedResult>
        {
            new() { CheckId = "test1", Outcome = TestOutcome.Pass, ExecutionTime = "1s", QueryText = "" }
        };

        Assert.False(DqdProcessor.HasNonSuppressedFailures(results));
    }

    [Fact]
    public void Parse_ValidJson_ReturnsPopulatedResults()
    {
        string json = """
        {
            "CheckResults": [
                {
                    "checkId": "test_check_1",
                    "failed": 0,
                    "passed": 1,
                    "isError": 0,
                    "notApplicable": 0,
                    "executionTime": "1.0 secs",
                    "queryText": "SELECT 1",
                    "checkName": "testCheck",
                    "checkLevel": "TABLE",
                    "checkDescription": "A test",
                    "cdmTableName": "TEST",
                    "sqlFile": "test.sql",
                    "category": "Conformance",
                    "subcategory": "Relational",
                    "context": "Verification",
                    "numViolatedRows": 0,
                    "pctViolatedRows": 0.0,
                    "numDenominatorRows": 1
                }
            ],
            "Metadata": [],
            "Overview": {}
        }
        """;

        var results = DqdProcessor.Parse(json);

        Assert.Single(results.CheckResults);
        Assert.Equal("test_check_1", results.CheckResults[0].CheckId);
        Assert.Equal(1, results.CheckResults[0].Passed);
        Assert.Equal("SELECT 1", results.CheckResults[0].QueryText);
    }

    [Fact]
    public void Parse_InvalidJson_Throws()
    {
        Assert.ThrowsAny<Exception>(() => DqdProcessor.Parse("not json"));
    }
}
