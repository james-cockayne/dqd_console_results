namespace DqdConsoleResults;

public static class ConsoleRenderer
{
    private const int LineWidth = 120;

    public static void Render(List<ProcessedResult> results, HashSet<string> suppressedTests)
    {
        RenderTestList(results);
        RenderFailureDetails(results);
        RenderSuppressedPassWarnings(results, suppressedTests);
    }

    private static readonly int MaxStatusLength = new[] { "SUCCESS", "FAIL", "SKIP", "SUPPRESSED" }.Max(s => s.Length);

    private static void RenderTestList(List<ProcessedResult> results)
    {
        int total = results.Count;
        for (int i = 0; i < results.Count; i++)
        {
            var result = results[i];
            string status = FormatStatus(result.Outcome);
            string paddedStatus = status.PadRight(MaxStatusLength);
            string prefix = $"{i + 1} of {total} {result.CheckId} ";
            string suffix = $" [{paddedStatus} in {result.ExecutionTime}]";
            int dotsNeeded = LineWidth - prefix.Length - suffix.Length;
            string dots = dotsNeeded > 0 ? new string('.', dotsNeeded) : "..";

            Console.Write(prefix);
            Console.Write(dots);
            Console.Write(" [");
            WriteColoredText(paddedStatus, result.Outcome);
            Console.WriteLine($" in {result.ExecutionTime}]");
        }
    }

    private static void RenderFailureDetails(List<ProcessedResult> results)
    {
        var failures = results.Where(r => r.Outcome == TestOutcome.Fail).ToList();
        if (failures.Count == 0)
            return;

        Console.WriteLine();
        Console.WriteLine("Failures:");

        foreach (var failure in failures)
        {
            Console.WriteLine();
            WriteColoredText("FAIL", TestOutcome.Fail);
            Console.WriteLine($": {failure.CheckId}");
            Console.WriteLine(FormatSql(failure.QueryText));
        }
    }

    private static void RenderSuppressedPassWarnings(List<ProcessedResult> results, HashSet<string> suppressedTests)
    {
        var suppressedPasses = results
            .Where(r => r.Outcome == TestOutcome.Pass && suppressedTests.Contains(r.CheckId))
            .ToList();

        if (suppressedPasses.Count == 0)
            return;

        Console.WriteLine();
        foreach (var result in suppressedPasses)
        {
            var original = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Warning: {result.CheckId} is suppressed but passed. Consider removing it from the suppression list.");
            Console.ForegroundColor = original;
        }
    }

    public static string FormatStatus(TestOutcome outcome)
    {
        return outcome switch
        {
            TestOutcome.Pass => "SUCCESS",
            TestOutcome.Fail => "FAIL",
            TestOutcome.Skip => "SKIP",
            TestOutcome.Suppressed => "SUPPRESSED",
            _ => throw new ArgumentOutOfRangeException(nameof(outcome))
        };
    }

    public static string FormatSql(string queryText)
    {
        return queryText
            .Replace("\r\n", "\n")
            .Replace("\r", "\n")
            .TrimEnd();
    }

    private static void WriteColoredText(string text, TestOutcome outcome)
    {
        var original = Console.ForegroundColor;
        Console.ForegroundColor = outcome switch
        {
            TestOutcome.Pass => ConsoleColor.Green,
            TestOutcome.Fail => ConsoleColor.Red,
            _ => original
        };
        Console.Write(text);
        Console.ForegroundColor = original;
    }
}
