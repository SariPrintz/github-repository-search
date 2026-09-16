using System.Text.Json.Serialization;

namespace GithubRepositorySearch.Api.Models
{
    public class GitHubRepository
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("owner")]
        public GitHubRepositoryOwner? Owner { get; set; }

        [JsonPropertyName("html_url")]
        public string? HtmlUrl { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("stargazers_count")]
        public int StargazersCount { get; set; }
    }
}
