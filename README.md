# GitHub Repository Search

## Project Overview

This repository contains a take-home assignment implementing a GitHub repository search application. The solution is split into a frontend (Angular) and a backend (ASP.NET Core Web API). The frontend (Angular) calls the backend API; the backend performs GitHub REST API requests server-side and exposes endpoints for search, authentication (simple JWT issuance), and bookmarking.

All GitHub API requests are executed server-side by the ASP.NET Core backend — the browser does not call the GitHub API directly.

## Technologies

- Angular 21
- .NET 10
- ASP.NET Core Web API
- Angular Material
- JWT authentication
- xUnit
- GitHub REST API

> Note: The frontend source lives under `FE/github-repository-search-client` and is an Angular application. The backend is an ASP.NET Core Web API under `BE/GithubRepositorySearch.Api`.

## Main Features

- Repository search (server-side) using the GitHub REST API
- Repository gallery with owner avatar and repository information (frontend)
- Bookmark repositories (backend session storage)
- View bookmarked repositories (per authenticated user)
- JWT authentication (simple login issuing a token)
 - Route protection for the bookmarks page (Angular route protected by an authGuard; backend bookmark endpoints are protected with `[Authorize]`)
- Custom in-memory session storage for bookmarks (per-user)
- Error handling for external API failures and invalid requests

## Project Structure

- BE/
  - GithubRepositorySearch.Api/ - ASP.NET Core 10 backend project (project file: `BE/GithubRepositorySearch.Api/GithubRepositorySearch.Api/GithubRepositorySearch.Api.csproj`)
  - GithubRepositorySearch.Api.Tests/ - xUnit backend tests

- FE/
  - github-repository-search-client/ - Angular 21 frontend application (package.json in this folder)

## Application Flow

1. Angular frontend → calls the ASP.NET Core backend API.
2. Backend → calls GitHub REST API (https://api.github.com/search/repositories) and returns structured results to the frontend.

The GitHub API is not called directly from the browser — all GitHub queries are proxied through the backend.

## Prerequisites

- .NET 10 SDK installed
- Node.js (and npm) installed for the frontend
- Optional: Angular CLI if you prefer to use it directly for frontend tasks

## Backend Configuration (JWT)

The backend reads JWT configuration from IConfiguration keys:

- `Jwt:Key` (signing key)
- `Jwt:Issuer`
- `Jwt:Audience`

The signing key must NOT be committed to source or stored in `appsettings.json`. `Jwt:Issuer` and `Jwt:Audience` are already configured in `BE/GithubRepositorySearch.Api/appsettings.json`; for local development store only the signing key using the Secret Manager. Example (run from the `BE/GithubRepositorySearch.Api` folder):

```bash
cd BE/GithubRepositorySearch.Api
dotnet user-secrets init
dotnet user-secrets set "Jwt:Key" "<your-secret-key>"
```

You can also set the signing key via an environment variable (e.g. `Jwt__Key`) in production or CI.

## Running the Backend

From the repository root, run:

```bash
dotnet run --project BE/GithubRepositorySearch.Api/GithubRepositorySearch.Api/GithubRepositorySearch.Api.csproj
```

In development, the project exposes Swagger UI at `https://localhost:7268/swagger/index.html` (developer environment). The API endpoints are available under `/api`.

## Running the Frontend

Frontend project path: `FE/github-repository-search-client`

Install dependencies and start the dev server:

```bash
cd FE/github-repository-search-client
npm install
npm start
```

The frontend `package.json` uses `ng serve` for `npm start`.
A proxy configuration file is present in the frontend project (`FE/github-repository-search-client/proxy.conf.json`). During Angular development the dev server forwards `/api/**` requests to the backend (configured to `https://localhost:7268` in `proxy.conf.json`). The development `serve` configuration in `FE/github-repository-search-client/angular.json` references this proxy file.

## API Endpoints

The backend implements the following endpoints:

| Method | Route | Description |
| ------ | ----- | ----------- |
| POST | /api/auth/login | Accepts `{ "username": "..." }` and returns a JWT token for the provided username (simplified login for the assignment). |
| GET  | /api/repositories/search?keyword={keyword} | Searches GitHub repositories (server-side) by keyword and returns a strongly-typed search response. |
| GET  | /api/Bookmarks | Returns the current authenticated user's bookmarked repositories. Requires JWT. |
| POST | /api/Bookmarks | Adds a GitHubRepository (JSON body) to the current user's bookmarks. Requires JWT. |
| DELETE | /api/Bookmarks?htmlUrl={htmlUrl} | Removes the bookmark with the specified `htmlUrl` for the current user. Requires JWT. |

> All bookmark endpoints are protected with `[Authorize]` and require a valid JWT in the `Authorization: Bearer <token>` header.

## Testing

The backend contains xUnit tests that exercise the `SessionService` behavior. The tests cover:

- Add / Get / Remove bookmark behavior
- Prevention of duplicate bookmarks
- Isolation of bookmarks between different authenticated users

Run the backend tests from the repository root:

```bash
dotnet test BE/GithubRepositorySearch.Api.Tests/GithubRepositorySearch.Api.Tests.csproj
```

## Session Storage

Bookmarks are stored in a custom in-memory session implementation (`ISessionService` / `SessionService`) that keeps per-user bookmarks in process using thread-safe collections. Limitations:

- Data is stored in memory and is lost when the backend process restarts.
- The implementation is not suitable for multi-instance/scale-out production scenarios (no distributed/shared storage).

## Authentication Note

The current login flow is intentionally simplified for the assignment: the `POST /api/auth/login` endpoint accepts a username and returns a signed JWT containing a `ClaimTypes.Name` claim. This is not a full production identity solution; it is a lightweight mechanism used for demonstration and for protecting bookmark endpoints in this assignment.

## Assignment Notes / Design Decisions

- Server-side GitHub API calls are implemented in `Services/GitHubService.cs` using an injected `HttpClient` and System.Text.Json to deserialize responses into typed models.
- JWT generation is provided by `Services/JwtService.cs`, which reads configuration from `IConfiguration` and validates that `Jwt:Key` is present.
- Bookmarks/session storage is implemented as a custom in-memory service (`ISessionService` / `SessionService`) keyed by the authenticated username and using `ConcurrentDictionary` for thread safety.
- Unit tests use xUnit and exercise the session service without any external mocking libraries.

---
