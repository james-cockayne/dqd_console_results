using Newtonsoft.Json;

namespace DqdConsoleResults;

public static class DqdProcessor
{
    public static DqdResults Parse(string json)
    {
        return JsonConvert.DeserializeObject<DqdResults>(json)
            ?? throw new InvalidOperationException("Failed to deserialize results JSON.");
    }

    public static TestOutcome DetermineOutcome(CheckResult check)
    {
        if (check.Failed == 1 || check.IsError == 1)
            return TestOutcome.Fail;
        if (check.Passed == 1)
            return TestOutcome.Pass;
        if (check.NotApplicable == 1)
            return TestOutcome.Skip;
        throw new InvalidOperationException($"Unable to determine outcome for check '{check.CheckId}'.");
    }

    public static List<ProcessedResult> ProcessResults(DqdResults results, HashSet<string> suppressedTests)
    {
        return results.CheckResults.Select(check =>
        {
            var outcome = DetermineOutcome(check);
            if (outcome == TestOutcome.Fail && suppressedTests.Contains(check.CheckId))
                outcome = TestOutcome.Suppressed;

            return new ProcessedResult
            {
                CheckId = check.CheckId,
                Outcome = outcome,
                ExecutionTime = check.ExecutionTime,
                QueryText = check.QueryText,
                PctViolatedRows = check.PctViolatedRows,
                ThresholdValue = check.ThresholdValue
            };
        }).ToList();
    }

    public static bool HasNonSuppressedFailures(List<ProcessedResult> results)
    {
        return results.Any(r => r.Outcome == TestOutcome.Fail);
    }
}
