using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using GithubRepositorySearch.Api.Models;
using Microsoft.AspNetCore.Http;

namespace GithubRepositorySearch.Api.Services
{
    public class SessionService : ISessionService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        // user -> (repoHtmlUrl -> repository)
        private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, GitHubRepository>> _store
            = new(StringComparer.OrdinalIgnoreCase);

        public SessionService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public Task<IEnumerable<GitHubRepository>> GetBookmarksAsync()
        {
            var username = GetUsernameOrThrow();

            if (_store.TryGetValue(username, out var userDict))
            {
                var list = userDict.Values.ToList();
                return Task.FromResult<IEnumerable<GitHubRepository>>(list);
            }

            return Task.FromResult<IEnumerable<GitHubRepository>>(Array.Empty<GitHubRepository>());
        }

        public Task<bool> AddBookmarkAsync(GitHubRepository repository)
        {
            if (repository == null) throw new ArgumentNullException(nameof(repository));
            if (string.IsNullOrWhiteSpace(repository.HtmlUrl)) throw new ArgumentException("Repository must have HtmlUrl.", nameof(repository));

            var username = GetUsernameOrThrow();

            var userDict = _store.GetOrAdd(username, _ => new ConcurrentDictionary<string, GitHubRepository>(StringComparer.OrdinalIgnoreCase));

            var added = userDict.TryAdd(repository.HtmlUrl, repository);
            return Task.FromResult(added);
        }

        public Task<bool> RemoveBookmarkAsync(string htmlUrl)
        {
            if (string.IsNullOrWhiteSpace(htmlUrl)) throw new ArgumentException("htmlUrl must be provided.", nameof(htmlUrl));

            var username = GetUsernameOrThrow();

            if (_store.TryGetValue(username, out var userDict))
            {
                var removed = userDict.TryRemove(htmlUrl, out _);
                return Task.FromResult(removed);
            }

            return Task.FromResult(false);
        }

        private string GetUsernameOrThrow()
        {
            var context = _httpContextAccessor.HttpContext;
            if (context?.User?.Identity?.IsAuthenticated != true)
            {
                throw new InvalidOperationException("No authenticated user available in the current context.");
            }

            // Prefer Name claim then ClaimTypes.Name
            var name = context.User.Identity?.Name;
            if (string.IsNullOrWhiteSpace(name))
            {
                name = context.User.FindFirst(ClaimTypes.Name)?.Value;
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new InvalidOperationException("Authenticated user does not have a name claim.");
            }

            return name;
        }
    }
}
