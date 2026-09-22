using System.Collections.Generic;
using System.Threading.Tasks;
using GithubRepositorySearch.Api.Models;

namespace GithubRepositorySearch.Api.Services
{
    public interface ISessionService
    {
        Task<IEnumerable<GitHubRepository>> GetBookmarksAsync();

        Task<bool> AddBookmarkAsync(GitHubRepository repository);

        Task<bool> RemoveBookmarkAsync(string htmlUrl);
    }
}
