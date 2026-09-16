using System;
using System.Threading;
using System.Threading.Tasks;
using GithubRepositorySearch.Api.Models;
using GithubRepositorySearch.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace GithubRepositorySearch.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class RepositoriesController : ControllerBase
    {
        private readonly GitHubService _gitHubService;

        public RepositoriesController(GitHubService gitHubService)
        {
            _gitHubService = gitHubService ?? throw new ArgumentNullException(nameof(gitHubService));
        }

        [HttpGet("search")]
        public async Task<ActionResult<GitHubSearchResponse>> Search([FromQuery] string keyword, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return BadRequest("Query parameter 'keyword' is required.");
            }

            var result = await _gitHubService.SearchRepositoriesAsync(keyword, cancellationToken).ConfigureAwait(false);

            return Ok(result);
        }
    }
}
