# dqd_console_results

A .NET console application that parses OHDSI Data Quality Dashboard `results.json` files and displays test outcomes in the terminal, styled after dbt's log output.

## Features

- Lists all checks by `checkId` with outcome: SUCCESS, FAIL, SKIP, or SUPPRESSED
- Colour-coded status in terminal output (green for success, red for fail)
- Displays full SQL for any failed checks
- Supports suppressing known failures via environment variable
- Returns a non-zero exit code if there are any non-suppressed failures

## Usage

```
DqdConsoleResults <path-to-results.json>
```

### Suppressing known failures

Set the `SUPPRESSED_TESTS` environment variable to a comma-separated list of `checkId` values:

```
SUPPRESSED_TESTS=table_cdmtable_metadata,table_cdmtable_visit_occurrence DqdConsoleResults results.json
```

Suppressed failures are listed with a SUPPRESSED status and do not cause a non-zero exit code.

## Docker

### Build

```bash
docker build -t dqd_console_results .
```

### Run

```bash
docker run --rm -v /path/to/results:/data dqd_console_results /data/results.json
```

### Run with suppressed tests

```bash
docker run --rm -v /path/to/results:/data -e SUPPRESSED_TESTS=table_cdmtable_metadata,table_cdmtable_visit_occurrence dqd_console_results /data/results.json
```

## Build from source

```bash
dotnet build
dotnet test
dotnet run --project src/DqdConsoleResults -- path/to/results.json
```
