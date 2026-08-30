# DevDock — AI-Powered Developer Workspace

DevDock is a full-stack developer productivity platform that brings project management, task tracking, and AI-powered code review together in a single workspace. Built with a Clean Architecture ASP.NET Core backend and an Angular frontend, it lets teams manage projects, assign tasks on a Kanban board, and get instant AI feedback on code — all secured with JWT authentication and fully containerized with Docker.

## ✨ Features

- **Authentication** — Register/login with JWT access tokens and refresh token rotation, role-based access control
- **Project Management** — Create projects, invite team members, manage roles (Owner/Member)
- **Task Management** — Kanban-style board (Todo → In Progress → In Review → Done), task assignment, priority levels, deadlines
- **Project Dashboard** — Live task-status breakdown and overdue task tracking per project
- **AI Code Review** — Paste any code snippet and get an instant, structured review (bug count, security issues, performance issues, and actionable suggestions) powered by Google's Gemini API
- **Review History** — Every AI review is persisted to the database for later reference
- **Containerized** — Backend, frontend, and database each run in their own Docker container, orchestrated with a single `docker-compose` file

## 🛠️ Tech Stack

**Backend**
- ASP.NET Core 8 Web API
- Entity Framework Core + PostgreSQL
- Clean Architecture (Domain / Application / Infrastructure / API layers)
- JWT Bearer Authentication with refresh tokens
- Google Gemini API integration for AI code review

**Frontend**
- Angular 21 (standalone components, signals, new control-flow syntax)
- Reactive Forms
- RxJS
- Custom HTTP interceptor for automatic JWT attachment

**Infrastructure**
- Docker & Docker Compose (multi-container: API + Angular/Nginx + PostgreSQL)
- .NET User Secrets for local secret management

## 🏗️ Architecture

The backend follows **Clean Architecture**, keeping business logic independent of frameworks and infrastructure:

```
┌─────────────────────────────────────────────┐
│                 DevDock.API                  │  Controllers, middleware, DI wiring
├─────────────────────────────────────────────┤
│              DevDock.Infrastructure          │  EF Core, services, Gemini client, JWT
├─────────────────────────────────────────────┤
│              DevDock.Application             │  Interfaces, DTOs, business rules
├─────────────────────────────────────────────┤
│                DevDock.Domain                │  Entities — no external dependencies
└─────────────────────────────────────────────┘
```

Dependencies point inward: `Domain` has zero dependencies, `Application` depends only on `Domain`, and `Infrastructure`/`API` implement those abstractions. This keeps the core business rules testable and swappable (e.g. the database or AI provider could be replaced without touching business logic).

### Request flow — AI Code Review

```
Angular UI  →  POST /api/code-review  →  CodeReviewController
                                              │
                                              ▼
                                     CodeReviewService
                                    (builds structured prompt)
                                              │
                                              ▼
                                       Gemini API call
                                              │
                                              ▼
                                  Parse + validate JSON response
                                              │
                                              ▼
                              Persist to PostgreSQL (CodeReviews)
                                              │
                                              ▼
                                 Structured JSON → Angular UI
```

## 📂 Project Structure

```
DevDock/
├── src/
│   ├── DevDock.Domain/          # Entities (User, Project, TaskItem, CodeReview...)
│   ├── DevDock.Application/     # Interfaces, DTOs, custom exceptions
│   ├── DevDock.Infrastructure/  # EF Core, services, Gemini integration, JWT
│   └── DevDock.API/             # Controllers, middleware, Program.cs
├── frontend/                    # Angular application
│   └── src/app/
│       ├── pages/                # Login, Register, Projects, Project Detail, Code Review
│       ├── services/              # Auth, Project, Task, CodeReview services
│       ├── interceptors/          # JWT auto-attach interceptor
│       └── models/                # TypeScript interfaces
├── docker-compose.yml
└── .env                          # Local secrets (not committed)
```

## 🚀 Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Node.js 20+](https://nodejs.org/) and Angular CLI (`npm install -g @angular/cli`)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- A [Google Gemini API key](https://aistudio.google.com/apikey) (free tier available)

### Option 1 — Run everything with Docker (recommended)

1. Clone the repository
   ```bash
   git clone https://github.com/Minahil-Shahid23/DevDock.git
   cd DevDock
   ```

2. Create a `.env` file in the project root:
   ```
   GEMINI_API_KEY=your-gemini-api-key-here
   ```

3. Build and start all containers:
   ```bash
   docker compose up --build
   ```

4. Apply database migrations (first run only):
   ```bash
   dotnet ef database update --project src/DevDock.Infrastructure --startup-project src/DevDock.API
   ```

5. Open the app:
   - Frontend: [http://localhost:4200](http://localhost:4200)
   - API + Swagger: [http://localhost:5296/swagger](http://localhost:5296/swagger)

### Option 2 — Run locally without Docker

**Backend**
```bash
# Start PostgreSQL (via Docker, or a local install)
docker run --name devdock-postgres -e POSTGRES_USER=devdock -e POSTGRES_PASSWORD=devdock123 -e POSTGRES_DB=devdockdb -p 5434:5432 -d postgres:16

# Set secrets
cd src/DevDock.API
dotnet user-secrets init
dotnet user-secrets set "GeminiSettings:ApiKey" "your-gemini-api-key-here"

# Apply migrations and run
cd ../..
dotnet ef database update --project src/DevDock.Infrastructure --startup-project src/DevDock.API
cd src/DevDock.API
dotnet run
```

**Frontend**
```bash
cd frontend
npm install
ng serve
```

## 🔑 Environment Configuration

| Setting | Location | Purpose |
|---|---|---|
| `ConnectionStrings:DefaultConnection` | `appsettings.json` / Docker env | PostgreSQL connection |
| `JwtSettings:SecretKey` | `appsettings.json` / User Secrets | JWT signing key |
| `GeminiSettings:ApiKey` | User Secrets / `.env` | Google Gemini API key — **never commit this** |

## 📡 Key API Endpoints

| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/auth/register` | Create a new account |
| POST | `/api/auth/login` | Authenticate and receive tokens |
| POST | `/api/auth/refresh` | Exchange refresh token for a new access token |
| GET | `/api/projects` | List the current user's projects |
| POST | `/api/projects` | Create a new project |
| POST | `/api/projects/{id}/members` | Add a team member by email |
| GET | `/api/projects/{id}/dashboard` | Task status breakdown for a project |
| POST | `/api/projects/{id}/tasks` | Create a task in a project |
| PUT | `/api/tasks/{id}` | Update task status/details |
| POST | `/api/code-review` | Submit code for AI review |

Full interactive API documentation is available via Swagger at `/swagger` when running the backend.

## 🗺️ Roadmap

- [ ] Real-time task notifications via SignalR
- [ ] Redis caching and rate limiting on the AI review endpoint
- [ ] Background job processing for AI reviews (async queue)
- [ ] GitHub webhook integration — automatic AI review on pull requests
- [ ] Cloud deployment with CI/CD pipeline
- [ ] Unit and integration test coverage (xUnit)

## 📄 License

This project was built as a personal learning and portfolio project.
