# Tournament API - Installation & Setup Guide

## Prerequisites

- .NET 10.0 SDK
- SQL Server (local installation or SQL Server in Docker)

## Installation Steps

### 1. Database Setup

#### Option A: Using Docker (Recommended for Linux/Mac)

```bash
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourPassword123!" \
  -p 1433:1433 \
  --name tournament-mssql \
  -d mcr.microsoft.com/mssql/server:2022-latest
```

#### Option B: Local SQL Server Installation

If you have SQL Server installed locally, ensure it's running before proceeding.

### 2. Update Connection String

Edit `appsettings.json` if needed. Default configuration:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=TournamentDb;User Id=sa;Password=YourPassword123!;Encrypt=false;TrustServerCertificate=true;"
}
```

If using Docker with different password, update the connection string accordingly.

### 3. Build and Run

```bash
cd TournamentAPI

# Build project
dotnet build

# Run (migrations will be applied automatically)
dotnet run
```

The API will be available at:
- HTTPS: `https://localhost:7001`
- HTTP: `http://localhost:5012`

## API Endpoints

### Tournament Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/tournaments` | Get all tournaments (supports ?search=keyword) |
| GET | `/api/tournaments/{id}` | Get tournament by ID |
| POST | `/api/tournaments` | Create new tournament |
| PUT | `/api/tournaments/{id}` | Update tournament |
| DELETE | `/api/tournaments/{id}` | Delete tournament |

### Game Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/tournaments/{tournamentId}/games` | Get all games for tournament |
| GET | `/api/tournaments/{tournamentId}/games/{id}` | Get specific game |
| POST | `/api/tournaments/{tournamentId}/games` | Create new game |
| PUT | `/api/tournaments/{tournamentId}/games/{id}` | Update game |
| DELETE | `/api/tournaments/{tournamentId}/games/{id}` | Delete game |

## Example Requests

### Create Tournament

```bash
curl -X POST https://localhost:7001/api/tournaments \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Spring Tournament 2026",
    "description": "Annual spring gaming tournament",
    "maxPlayers": 32,
    "date": "2026-03-15T10:00:00Z"
  }'
```

### Search Tournaments

```bash
curl https://localhost:7001/api/tournaments?search=spring
```

### Create Game

```bash
curl -X POST https://localhost:7001/api/tournaments/1/games \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Final Match",
    "time": "2026-03-15T14:00:00Z",
    "tournamentId": 1
  }'
```

## Rate Limiting

- POST requests are limited to **2 requests per second** per IP address
- Exceeding limit returns HTTP 429 (Too Many Requests)

## Validation Rules

### Tournament

- **Title**: Required, 3-100 characters
- **Description**: Optional, max 500 characters
- **MaxPlayers**: Required, must be > 0
- **Date**: Must be in the future

### Game

- **Title**: Required, 3-100 characters
- **Time**: Must be in the future
- **TournamentId**: Must reference existing tournament

## Features Implemented

✅ CRUD endpoints for Tournaments and Games  
✅ Entity Framework Core with SQL Server  
✅ Async/await database operations  
✅ DTOs with validation (DataAnnotations)  
✅ Dependency Injection (AddScoped)  
✅ Search functionality (case-insensitive, partial match)  
✅ Cascade delete for Games when Tournament is deleted  
✅ Rate limiting (2 requests/second on POST)  
✅ 1-to-many relationship (Tournament has many Games)  

## Troubleshooting

### "Connection refused" error

Ensure SQL Server is running:
- **Docker**: `docker ps` should show the tournament-mssql container running
- **Local**: Verify SQL Server is started in Services (Windows) or appropriate manager

### "Cannot find path" error during migrations

Make sure you're in the TournamentAPI directory when running `dotnet run`

### Database already exists error

The migrations will handle creating the database automatically. If you need to reset:

```bash
# Remove database
dotnet ef database drop

# Reapply migrations
dotnet ef database update
```

## Project Structure

```
TournamentAPI/
├── Controllers/          # API endpoints
├── DTOs/                # Data Transfer Objects
├── Models/              # Domain entities
├── Services/            # Business logic
├── Data/                # DbContext
├── Migrations/          # EF Core migrations
├── Program.cs           # Startup configuration
└── appsettings.json     # Configuration
```
