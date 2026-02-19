using Microsoft.EntityFrameworkCore;
using TournamentAPI.Data;
using TournamentAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Add DbContext with SQL Server connection
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Server=localhost;Database=TournamentDb;User Id=sa;Password=YourPassword123!;Encrypt=false;TrustServerCertificate=true;";

builder.Services.AddDbContext<TournamentContext>(options =>
    options.UseSqlServer(connectionString));

// Register services with appropriate lifetimes
builder.Services.AddScoped<ITournamentService, TournamentService>();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddSingleton<RateLimitingService>();

// Add logging
builder.Services.AddLogging();

var app = builder.Build();

// Apply migrations automatically (with error handling)
try
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<TournamentContext>();
        db.Database.Migrate();
    }
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogWarning($"Database migration failed: {ex.Message}. Make sure your SQL Server is running and the connection string is correct.");
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    // Exposes Swagger UI: /swagger
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "TournamentAPI");
        options.RoutePrefix = "swagger";
    });

}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapOpenApi();

app.Run();
