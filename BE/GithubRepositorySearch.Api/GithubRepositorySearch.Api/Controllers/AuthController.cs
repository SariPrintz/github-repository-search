using System;
using GithubRepositorySearch.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace GithubRepositorySearch.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly JwtService _jwtService;

        public AuthController(JwtService jwtService)
        {
            _jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
        }

        public class LoginRequest
        {
            public string? Username { get; set; }
        }

        [HttpPost("login")]
        public ActionResult Login([FromBody] LoginRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Username))
            {
                return BadRequest("Username is required.");
            }

            var token = _jwtService.GenerateToken(request.Username);

            return Ok(new { token, username = request.Username });
        }
    }
}
