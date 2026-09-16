using System.Text.Json.Serialization;

namespace GithubRepositorySearch.Api.Models
{
    public class GitHubRepositoryOwner
    {
        [JsonPropertyName("login")]
        public string? Login { get; set; }

        [JsonPropertyName("avatar_url")]
        public string? AvatarUrl { get; set; }
    }
}
