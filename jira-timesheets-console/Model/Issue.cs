using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace jiratimesheetsconsole.Model
{
    [JsonObject]
    public class JiraTimecards
    {
        [JsonProperty("total")]
        public string Total { get; set; }

        [JsonProperty("expand")]
        public string Expand { get; set; }

        [JsonProperty("issues")]
        public List<Issue> Issues { get; set; }
    }

    [JsonObject]
    public class Issue
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("self")]
        public string Self { get; set; }
        [JsonProperty("key")]
        public string Key { get; set; }
        [JsonProperty("fields")]
        public Field Fields { get; set; }
    }

    [JsonObject]
    public class ParentIssue
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("self")]
        public string Self { get; set; }
        [JsonProperty("key")]
        public string Key { get; set; }
        [JsonProperty("fields")]
        public Field Fields { get; set; }
    }

    [JsonObject]
    public class Field
    {
        [JsonProperty("summary")]
        public string Summary { get; set; }
        [JsonProperty("issuetype")]
        public IssueType Type { get; set; }
        //[JsonProperty("project")]
        //public string Project { get; set; }
        [JsonProperty("parent")]
        public ParentIssue Parent { get; set; }
        [JsonProperty("worklog")]
        public Worklog Worklog { get; set; }
    }

    [JsonObject]
    public class IssueType
    {
        [JsonProperty("name")]
        public string TypeName { get; set; }
    }

    [JsonObject]
    public class Worklog
    {
        [JsonProperty("total")]
        public string Total { get; set; }
        [JsonProperty("worklogs")]
        public List<WorklogEntry> WorklogEntries { get; set; }
    }

    [JsonObject]
    public class WorklogEntry
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("self")]
        public string Self { get; set; }
        [JsonProperty("comment")]
        public string Comment { get; set; }
        [JsonProperty("started")]
        public DateTime Started { get; set; }
        [JsonProperty("timeSpent")]
        public string TimeSpent { get; set; }
        [JsonProperty("timeSpentSeconds")]
        public double TimeSpentSeconds { get; set; }
    }
}
