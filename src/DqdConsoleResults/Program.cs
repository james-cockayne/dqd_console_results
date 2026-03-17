namespace DqdConsoleResults;

public class Program
{
    public static int Main(string[] args)
    {
        if (args.Length < 1)
        {
            Console.Error.WriteLine("Usage: dqd_console_results <path-to-results.json>");
            return 1;
        }

        var suppressedEnv = Environment.GetEnvironmentVariable("SUPPRESSED_TESTS") ?? string.Empty;
        var suppressedTests = new HashSet<string>(
            suppressedEnv.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));

        string json = File.ReadAllText(args[0]);
        var dqdResults = DqdProcessor.Parse(json);
        var processedResults = DqdProcessor.ProcessResults(dqdResults, suppressedTests);

        ConsoleRenderer.Render(processedResults, suppressedTests);

        return DqdProcessor.HasNonSuppressedFailures(processedResults) ? 1 : 0;
    }
}
