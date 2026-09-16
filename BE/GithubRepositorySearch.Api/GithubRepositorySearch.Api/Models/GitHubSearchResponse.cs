using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GithubRepositorySearch.Api.Models
{
    public class GitHubSearchResponse
    {
        [JsonPropertyName("total_count")]
        public int TotalCount { get; set; }

        [JsonPropertyName("items")]
        public List<GitHubRepository>? Items { get; set; }
    }
}
