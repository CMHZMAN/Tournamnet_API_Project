# Tournament API - Implementation Summary

## ✅ Completed Requirements

### Core Requirements (Betyg G)

✅ **Controllers + Services + DI**
- Thin controllers with business logic in services
- Dependency Injection using AddScoped for services
- Custom RateLimitingService as singleton

✅ **DTOs with Validation**
- TournamentCreateDTO (POST)
- TournamentUpdateDTO (PUT)
- TournamentResponseDTO (GET response)
- GameCreateDTO (POST)
- GameUpdateDTO (PUT)
- GameResponseDTO (GET response)
- All include DataAnnotations validation

✅ **CRUD Endpoints for Tournaments**
- GET /api/tournaments (with search support)
- GET /api/tournaments/{id}
- POST /api/tournaments
- PUT /api/tournaments/{id}
- DELETE /api/tournaments/{id}

✅ **Database Configuration**
- Entity Framework Core 10.0.2
- SQL Server provider
- Code First approach
- Automatic migrations on startup
- Connection string in appsettings.json

✅ **Async Operations**
- All CRUD operations use async/await
- Async database queries with EF Core

### VG-Requirements (Higher Grade)

✅ **Two Entities with 1-to-Many Relationship**
- Tournament entity (parent)
- Game entity (child)
- Cascade delete configured

✅ **CRUD for Games**
- GET /api/tournaments/{tournamentId}/games
- GET /api/tournaments/{tournamentId}/games/{id}
- POST /api/tournaments/{tournamentId}/games
- PUT /api/tournaments/{tournamentId}/games/{id}
- DELETE /api/tournaments/{tournamentId}/games/{id}

### Extra Features Implemented

✅ **Search Functionality**
- Case-insensitive partial matching
- Query parameter: ?search=keyword
- Only alphanumeric, spaces, and hyphens allowed
- Invalid characters rejected with error message

✅ **Rate Limiting**
- 2 requests per second per IP address
- Applied to POST endpoints
- Returns HTTP 429 when exceeded
- Thread-safe implementation

✅ **Validation Rules**
- Tournament Title: Required, 3-100 characters
- Tournament Description: Optional, max 500 characters
- Tournament MaxPlayers: Must be > 0
- Tournament Date: Must be in the future
- Game Title: Required, 3-100 characters
- Game Time: Must be in the future
- TournamentId: Must reference existing tournament

✅ **Cascade Delete**
- Games automatically deleted when Tournament is deleted
- Configured in DbContext OnModelCreating

## 📁 Project Structure

```
TournamentAPI/
├── Controllers/
│   ├── TournamentsController.cs     # Tournament CRUD endpoints
│   └── GamesController.cs            # Game CRUD endpoints
├── DTOs/
│   ├── TournamentCreateDTO.cs
│   ├── TournamentUpdateDTO.cs
│   ├── TournamentResponseDTO.cs
│   ├── GameCreateDTO.cs
│   ├── GameUpdateDTO.cs
│   └── GameResponseDTO.cs
├── Models/
│   ├── Tournament.cs                # Domain entity
│   └── Game.cs                      # Domain entity
├── Services/
│   ├── ITournamentService.cs        # Tournament service interface
│   ├── TournamentService.cs         # Tournament service implementation
│   ├── IGameService.cs              # Game service interface
│   ├── GameService.cs               # Game service implementation
│   └── RateLimitingService.cs       # Rate limiting implementation
├── Data/
│   └── TournamentContext.cs         # EF Core DbContext
├── Migrations/
│   └── [Auto-generated migrations]
├── Program.cs                       # Startup configuration
├── appsettings.json                # Configuration
└── TournamentAPI.csproj            # Project file
```

## 🔄 Data Flow

1. **Request arrives** → API Controller
2. **Validation** → DTOs with DataAnnotations
3. **Business Logic** → Service layer (TournamentService/GameService)
4. **Database Operations** → EF Core with async/await
5. **Response** → Transformed to ResponseDTO
6. **Rate Limiting** → Checked before POST operations

## 🛠️ Technology Stack

- **.NET** 10.0
- **ASP.NET Core** Web API
- **Entity Framework Core** 10.0.2
- **SQL Server** (via Microsoft.Data.SqlClient)
- **C# 12.0**
- **Dependency Injection** (built-in)
- **DataAnnotations** (validation)

## 📊 Database Schema

### Tournaments Table
```sql
CREATE TABLE Tournaments (
    Id INT PRIMARY KEY IDENTITY,
    Title NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500),
    MaxPlayers INT NOT NULL,
    Date DATETIME2 NOT NULL
)
```

### Games Table
```sql
CREATE TABLE Games (
    Id INT PRIMARY KEY IDENTITY,
    Title NVARCHAR(100) NOT NULL,
    Time DATETIME2 NOT NULL,
    TournamentId INT NOT NULL FOREIGN KEY REFERENCES Tournaments(Id) ON DELETE CASCADE
)
```

## 🧪 Testing

Example test of create tournament then add game:

```bash
# 1. Create Tournament
curl -X POST https://localhost:7001/api/tournaments \
  -H "Content-Type: application/json" \
  -d '{"title":"Spring Series","maxPlayers":16,"date":"2026-06-01T10:00:00Z"}'

# Returns: {"id":1,"title":"Spring Series","maxPlayers":16,"date":"2026-06-01T10:00:00Z"}

# 2. Create Game for Tournament
curl -X POST https://localhost:7001/api/tournaments/1/games \
  -H "Content-Type: application/json" \
  -d '{"title":"Quarter Finals","time":"2026-06-01T14:00:00Z","tournamentId":1}'

# 3. Search Tournaments
curl "https://localhost:7001/api/tournaments?search=spring"

# 4. Delete Tournament (cascades to delete games)
curl -X DELETE https://localhost:7001/api/tournaments/1
```

## 🔐 Error Handling

- **400 Bad Request** - Validation errors, invalid search characters
- **404 Not Found** - Resource not found
- **429 Too Many Requests** - Rate limit exceeded
- **500 Internal Server Error** - Database connection issues

## 📝 Configuration

Connection string can be customized in `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=TournamentDb;User Id=sa;Password=YourPassword123!;Encrypt=false;TrustServerCertificate=true;"
}
```

## 🚀 Deployment Ready

- Clean architecture with separation of concerns
- Async operations throughout
- Proper error handling and logging
- Configurable via appsettings.json
- Automatic database migrations
- Scalable service layer
