namespace DqdConsoleResults;

public class ProcessedResult
{
    public required string CheckId { get; init; }
    public required TestOutcome Outcome { get; init; }
    public required string ExecutionTime { get; init; }
    public required string QueryText { get; init; }
}
