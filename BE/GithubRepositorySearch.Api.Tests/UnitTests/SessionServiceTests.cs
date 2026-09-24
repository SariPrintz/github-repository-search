using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using GithubRepositorySearch.Api.Models;
using GithubRepositorySearch.Api.Services;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace GithubRepositorySearch.Api.Tests.UnitTests
{
    public class SessionServiceTests
    {
        [Fact]
        public async Task AddGetRemove_Behavior()
        {
            // Arrange - authenticated user
            var username = "alice";
            var context = new DefaultHttpContext();
            context.User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, username) }, "TestAuth"));
            var accessor = new HttpContextAccessor { HttpContext = context };

            var service = new SessionService(accessor);

            var repo = new GitHubRepository
            {
                Name = "repo-one",
                HtmlUrl = "https://github.com/alice/repo-one",
                Description = "Test repo",
                StargazersCount = 5,
                Owner = new GitHubRepositoryOwner { Login = "alice", AvatarUrl = "https://example.com/avatar.png" }
            };

            // Act - add
            var added = await service.AddBookmarkAsync(repo);

            // Assert add succeeded
            Assert.True(added);

            // Act - get
            var bookmarks = (await service.GetBookmarksAsync()).ToList();

            Assert.Single(bookmarks);
            Assert.Equal(repo.HtmlUrl, bookmarks[0].HtmlUrl);

            // Act - remove
            var removed = await service.RemoveBookmarkAsync(repo.HtmlUrl);
            Assert.True(removed);

            var after = (await service.GetBookmarksAsync()).ToList();
            Assert.Empty(after);
        }

        [Fact]
        public async Task PreventDuplicateBookmarks()
        {
            // Arrange
            var username = "bob";
            var context = new DefaultHttpContext();
            context.User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, username) }, "TestAuth"));
            var accessor = new HttpContextAccessor { HttpContext = context };
            var service = new SessionService(accessor);

            var repo = new GitHubRepository
            {
                Name = "repo-dup",
                HtmlUrl = "https://github.com/bob/repo-dup",
                Description = "Duplicate test",
                StargazersCount = 2,
                Owner = new GitHubRepositoryOwner { Login = "bob", AvatarUrl = "https://example.com/bob.png" }
            };

            // Act
            var first = await service.AddBookmarkAsync(repo);
            var second = await service.AddBookmarkAsync(repo);

            // Assert
            Assert.True(first);
            Assert.False(second);

            var list = (await service.GetBookmarksAsync()).ToList();
            Assert.Single(list);
            Assert.Equal(repo.HtmlUrl, list[0].HtmlUrl);
        }

        [Fact]
        public async Task IsolationBetweenUsers()
        {
            // Arrange - create accessor that we can swap contexts on
            var accessor = new HttpContextAccessor();
            var service = new SessionService(accessor);

            var userA = "carol";
            var ctxA = new DefaultHttpContext();
            ctxA.User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, userA) }, "TestAuth"));

            var userB = "dave";
            var ctxB = new DefaultHttpContext();
            ctxB.User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, userB) }, "TestAuth"));

            var repoA = new GitHubRepository
            {
                Name = "repo-a",
                HtmlUrl = "https://github.com/carol/repo-a",
                Owner = new GitHubRepositoryOwner { Login = "carol" }
            };

            var repoB = new GitHubRepository
            {
                Name = "repo-b",
                HtmlUrl = "https://github.com/dave/repo-b",
                Owner = new GitHubRepositoryOwner { Login = "dave" }
            };

            // Act - set to user A and add
            accessor.HttpContext = ctxA;
            var addedA = await service.AddBookmarkAsync(repoA);
            Assert.True(addedA);

            // Switch to user B and verify no bookmarks
            accessor.HttpContext = ctxB;
            var bookmarksB = (await service.GetBookmarksAsync()).ToList();
            Assert.Empty(bookmarksB);

            // Add for user B
            var addedB = await service.AddBookmarkAsync(repoB);
            Assert.True(addedB);

            // Switch back to A and verify only A's bookmark
            accessor.HttpContext = ctxA;
            var bookmarksA = (await service.GetBookmarksAsync()).ToList();
            Assert.Single(bookmarksA);
            Assert.Equal(repoA.HtmlUrl, bookmarksA[0].HtmlUrl);

            // And B still has only its bookmark
            accessor.HttpContext = ctxB;
            var bookmarksBAfter = (await service.GetBookmarksAsync()).ToList();
            Assert.Single(bookmarksBAfter);
            Assert.Equal(repoB.HtmlUrl, bookmarksBAfter[0].HtmlUrl);
        }
    }
}
