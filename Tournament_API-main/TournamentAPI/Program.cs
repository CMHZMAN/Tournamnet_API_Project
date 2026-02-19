using Microsoft.EntityFrameworkCore;
using TournamentAPI.Data;
using TournamentAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Customize invalid model state responses so we log validation problems and return details
builder.Services.Configure<Microsoft.AspNetCore.Mvc.ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogWarning("Invalid model state: {ModelState}", context.ModelState);

        var problem = new Microsoft.AspNetCore.Mvc.ValidationProblemDetails(context.ModelState)
        {
            Title = "Model validation error",
            Status = StatusCodes.Status400BadRequest
        };
        return new Microsoft.AspNetCore.Mvc.BadRequestObjectResult(problem);
    };
});
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
    // Show developer exception page to surface errors during development
    app.UseDeveloperExceptionPage();

    app.MapOpenApi();

    // Exposes Swagger UI: /swagger
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "TournamentAPI");
        options.RoutePrefix = "swagger";
    });

}

// Global exception logging middleware to capture unhandled errors and return JSON
app.Use(async (context, next) =>
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Unhandled exception processing request {Method} {Path}", context.Request.Method, context.Request.Path);
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";
        var result = new { error = "An unexpected error occurred" };
        await context.Response.WriteAsJsonAsync(result);
    }
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
// MapOpenApi already called in development environment above. Avoid mapping twice.

app.Run();
