using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using GithubRepositorySearch.Api.Models;

namespace GithubRepositorySearch.Api.Services
{
    public class GitHubService
    {
        private readonly HttpClient _httpClient;
        private const string GitHubSearchUrlTemplate = "https://api.github.com/search/repositories?q={0}";

        public GitHubService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        /// <summary>
        /// Searches GitHub repositories by keyword and returns a strongly typed response.
        /// </summary>
        public async Task<GitHubSearchResponse> SearchRepositoriesAsync(string keyword, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                throw new ArgumentException("Search keyword must be provided.", nameof(keyword));

            var requestUri = string.Format(GitHubSearchUrlTemplate, Uri.EscapeDataString(keyword));

            using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            // GitHub requires a User-Agent header
            request.Headers.UserAgent.ParseAdd("GithubRepositorySearch.Api");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                throw new HttpRequestException($"GitHub API request failed with status {(int)response.StatusCode} ({response.ReasonPhrase}). Content: {errorContent}");
            }

            var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);

            var result = await JsonSerializer.DeserializeAsync<GitHubSearchResponse>(stream, cancellationToken: cancellationToken).ConfigureAwait(false);

            if (result is null)
            {
                throw new InvalidOperationException("Failed to deserialize GitHub search response.");
            }

            return result;
        }
    }
}
