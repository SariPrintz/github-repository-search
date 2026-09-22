using System;
using System.Threading.Tasks;
using GithubRepositorySearch.Api.Models;
using GithubRepositorySearch.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GithubRepositorySearch.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BookmarksController : ControllerBase
    {
        private readonly ISessionService _sessionService;

        public BookmarksController(ISessionService sessionService)
        {
            _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
        }

        // GET: /api/Bookmarks
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var bookmarks = await _sessionService.GetBookmarksAsync().ConfigureAwait(false);
            return Ok(bookmarks);
        }

        // POST: /api/Bookmarks
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] GitHubRepository repository)
        {
            if (repository == null)
            {
                return BadRequest("Repository is required.");
            }

            try
            {
                var added = await _sessionService.AddBookmarkAsync(repository).ConfigureAwait(false);
                if (!added)
                {
                    return Conflict("Repository is already bookmarked.");
                }

                // Return 201 Created with the repository in the body
                return Created(string.Empty, repository);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: /api/Bookmarks?htmlUrl={htmlUrl}
        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] string htmlUrl)
        {
            if (string.IsNullOrWhiteSpace(htmlUrl))
            {
                return BadRequest("htmlUrl is required.");
            }

            var removed = await _sessionService.RemoveBookmarkAsync(htmlUrl).ConfigureAwait(false);
            if (!removed)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
