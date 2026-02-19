# Tournament_API
🚀 Web API-uppgift – Game Tournament API ✅

Du ska bygga ett komplett ASP.NET Web API där följande ingår:
📌 Controllers + Services + DI + DTO + CRUD-endpoints + Entity Framework Core med databaskoppling mot SQL Server

🧩 Funktion: Vad ska API:t göra?

API:t ska hantera CRUD med turneringar (Tournaments).

📦 Datamodell

När vi pratar om vad som sparas i  databasen så använder vi begreppet "Entity".
Objektet som vi jobbar med i C# kallas för "Model".
Men i vår uppgift syftar båda ungefär på samma sak.
Vi kommer att använda Entity Framework Core och "Code First"-database approach, för att automatiskt generera SQL-tables utifrån våra C# klasser. Vi slipper skriva SQL-queries.
Denna entity ska användas:

📘 Entity: Tournament
🆔 Id (int)
🏷️ Title (string)
🏷️ Description (string)
🏆 MaxPlayers (int)
⏰ Date (DateTime)

 
🗂️ Strukturkrav på projektet (Clean-ish)
📁 Förslag på mappar:
Controllers/
Services/
Dtos/
Data/
Models/ (entities)
✅ Krav:
Controllers ska vara thin
Logik ska ligga i services
Dependency Injection (DI) används för TournamentService (med AddScoped lifetime (inte AddSingleton eller AddTransient))
🔁 DTO-krav

Du ska använda följande DTOs (inte entities direkt i endpoints):
Minimikravet på skillnad mellan Model och DTO är att POST och PUT  DTO:s inte ska innehålla Id property. ResponseDTO ska alltid innehålla Id.
📥 TournamentCreateDTO (för POST-request)
DTO:n innehåller inget Id.
🔄 TournamentUpdateDTO (för PUT-request)
DTO:n innehåller inget Id (den kommer från URL).
🏷️TournamentResponseDTO
DTO:n ska innehålla id. Men valfri annan property ska tas bort.
Mappning mellan Model och DTO får lov att ske i Controller, men den kan ske i en egen extern mappnings-klass också. Automatisk mappning av modell och DTO med NuGet-paket så som Automapper får också lov att användas.

🛡️ Validering (DataAnnotations eller FluentValidation)

Validering behöver finnas på alla DTO:s. Ingen validering krävs på vår Model.
Minimikrav på regler:
Title är obligatorisk ✅
Title minst 3 tecken 🔡
Date får inte vara i dåtid ⏳
🗄️ Databas (Entity Framework Core + SQL Server)

API:t ska använda:
✅ EF Core som ORM
✅ DbContext, för att jobba mot databasen
✅ Migrations, för att omvandla våra C# klasser till SQL-tables
✅ Async CRUD (async/await), asynkrona databas-anrop
✅ Connection string via config, för att ansluta till SQL-server (tex. appsettings.json)
🔍 Lista-funktioner på GET /api/tournaments

GET /api/tournaments ska stödja:

🔎 Search
?search=tournament (sök i Title)
Alltså sökning via query parameter.

📡 Endpoints för hela API:et (krav)

🏆 Tournaments
 🟢 GET /api/tournaments
 🟢 GET /api/tournaments/{id}
 🔒 POST /api/tournaments 
 🔒 PUT /api/tournaments/{id} 
 🔒 DELETE /api/tournaments/{id}



Allt ovan är krav för G på uppgfiten: 

🧩 VG-krav: Använd två entities, samt ha en 1-till-många relation

API:t ska hantera CRUD med turneringar (Tournaments) där varje turnering kan ha flera matcher (Games).

Alla krav ovan måste gälla även för Game entity:
Notera de extra properties så som:
Games (ICollection<Game>)
och att Game har två properties som hänvisar till Tournament:
TournamentId (int) 
Tournament (Tournament) 

📘 Entity: Tournament
🆔 Id (int)
🏷️ Title (string)
🏷️ Description (string)
🏆 MaxPlayers (int)
⏰ Date (DateTime)
🎮 Games (ICollection<Game>)

📘 Entity: Game 
🆔 Id (int) 
🏷️ Title (string) 
⏰ Time (DateTime) 
🔗 TournamentId (int) 
🏆 Tournament (Tournament) 


🔒FRIVILLIGA EXTRA KRAV!
Säkra DELETE-endpoint med JWT token.
Lägg till funktionalitet för Seed data (startdata). Lägg till data som automatiskt läggs till i databasen med hjälp av EF Core när man startar web API:t
Lägg på Rate Limiting på POST-endpoint för att skydda mot spam med hjälp av:
using System.Threading.RateLimiting;
Lägg på en PATCH-endpoint för updates av delar av entity (PUT ändrar hela entities).
Det är tillåtet att ändra tema från Tournament till något annat på uppgiften så länge alla tekniska krav är uppfyllda.
Det är också tillåtet att lägga till fler Entities och mer funktionalitet än vad kraven säger.
