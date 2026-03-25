using Newtonsoft.Json;

namespace DqdConsoleResults;

public class DqdResults
{
    [JsonProperty("CheckResults")]
    public List<CheckResult> CheckResults { get; set; } = new();

    [JsonProperty("Metadata")]
    public List<MetadataEntry> Metadata { get; set; } = new();

    [JsonProperty("Overview")]
    public Overview Overview { get; set; } = new();
}

public class CheckResult
{
    [JsonProperty("numViolatedRows")]
    public long NumViolatedRows { get; set; }

    [JsonProperty("pctViolatedRows")]
    public double PctViolatedRows { get; set; }

    [JsonProperty("numDenominatorRows")]
    public long NumDenominatorRows { get; set; }

    [JsonProperty("executionTime")]
    public string ExecutionTime { get; set; } = string.Empty;

    [JsonProperty("queryText")]
    public string QueryText { get; set; } = string.Empty;

    [JsonProperty("checkName")]
    public string CheckName { get; set; } = string.Empty;

    [JsonProperty("checkLevel")]
    public string CheckLevel { get; set; } = string.Empty;

    [JsonProperty("checkDescription")]
    public string CheckDescription { get; set; } = string.Empty;

    [JsonProperty("cdmTableName")]
    public string CdmTableName { get; set; } = string.Empty;

    [JsonProperty("sqlFile")]
    public string SqlFile { get; set; } = string.Empty;

    [JsonProperty("category")]
    public string Category { get; set; } = string.Empty;

    [JsonProperty("subcategory")]
    public string Subcategory { get; set; } = string.Empty;

    [JsonProperty("context")]
    public string Context { get; set; } = string.Empty;

    [JsonProperty("checkId")]
    public string CheckId { get; set; } = string.Empty;

    [JsonProperty("thresholdValue")]
    public double ThresholdValue { get; set; }

    [JsonProperty("failed")]
    public int Failed { get; set; }

    [JsonProperty("passed")]
    public int Passed { get; set; }

    [JsonProperty("isError")]
    public int IsError { get; set; }

    [JsonProperty("notApplicable")]
    public int NotApplicable { get; set; }
}

public class MetadataEntry
{
    [JsonProperty("cdmSourceName")]
    public string CdmSourceName { get; set; } = string.Empty;

    [JsonProperty("cdmSourceAbbreviation")]
    public string CdmSourceAbbreviation { get; set; } = string.Empty;

    [JsonProperty("cdmHolder")]
    public string CdmHolder { get; set; } = string.Empty;

    [JsonProperty("sourceDescription")]
    public string SourceDescription { get; set; } = string.Empty;

    [JsonProperty("cdmVersion")]
    public string CdmVersion { get; set; } = string.Empty;

    [JsonProperty("vocabularyVersion")]
    public string VocabularyVersion { get; set; } = string.Empty;

    [JsonProperty("dqdVersion")]
    public string DqdVersion { get; set; } = string.Empty;
}

public class Overview
{
    [JsonProperty("countTotal")]
    public List<int> CountTotal { get; set; } = new();

    [JsonProperty("countPassed")]
    public List<int> CountPassed { get; set; } = new();

    [JsonProperty("countOverallFailed")]
    public List<int> CountOverallFailed { get; set; } = new();

    [JsonProperty("percentPassed")]
    public List<double> PercentPassed { get; set; } = new();

    [JsonProperty("percentFailed")]
    public List<double> PercentFailed { get; set; } = new();
}
