using Microsoft.AspNetCore.Mvc;
using TournamentAPI.DTOs;
using TournamentAPI.Services;

namespace TournamentAPI.Controllers;

[ApiController]
[Route("api/tournaments/{tournamentId}/[controller]")]
public class GamesController : ControllerBase
{
    private readonly IGameService _gameService;
    private readonly ITournamentService _tournamentService;
    private readonly RateLimitingService _rateLimitingService;
    private readonly ILogger<GamesController> _logger;

    public GamesController(
        IGameService gameService,
        ITournamentService tournamentService,
        RateLimitingService rateLimitingService,
        ILogger<GamesController> logger)
    {
        _gameService = gameService;
        _tournamentService = tournamentService;
        _rateLimitingService = rateLimitingService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<GameResponseDTO>>> GetGames([FromRoute] int tournamentId)
    {
        _logger.LogInformation("GetGames called for tournamentId={TournamentId}", tournamentId);
        // Verify tournament exists
        var tournament = await _tournamentService.GetByIdAsync(tournamentId);
        if (tournament == null)
        {
            return NotFound(new { error = "Tournament not found" });
        }

        var games = await _gameService.GetAllAsync(tournamentId);
        return Ok(games);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GameResponseDTO>> GetGame([FromRoute] int tournamentId, int id)
    {
        _logger.LogInformation("GetGame called for tournamentId={TournamentId} id={Id}", tournamentId, id);
        // Verify tournament exists
        var tournament = await _tournamentService.GetByIdAsync(tournamentId);
        if (tournament == null)
        {
            return NotFound(new { error = "Tournament not found" });
        }

        var game = await _gameService.GetByIdAsync(id);
        if (game == null || game.TournamentId != tournamentId)
        {
            return NotFound();
        }
        return Ok(game);
    }

    /// <summary>
    /// Create a new game for the specified tournament.
    /// </summary>
    /// <remarks>
    /// The `tournamentId` is provided in the route. Do not include `tournamentId` in the request body.
    /// Example:
    /// {
    ///   "title": "Qualifying Match 1",
    ///   "time": "2026-03-02T13:00:00Z"
    /// }
    /// </remarks>
    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(GameResponseDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GameResponseDTO>> CreateGame(
        [FromRoute] int tournamentId,
        [FromBody] GameCreateDTO createDTO)
    {
        _logger.LogInformation("CreateGame called for tournamentId={TournamentId} with title={Title}", tournamentId, createDTO?.Title);
        // Check rate limit
        var clientIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        if (!_rateLimitingService.IsRequestAllowed(clientIp))
        {
            return StatusCode(429, new { error = "Too many requests" });
        }

        // Verify tournament exists
        var tournament = await _tournamentService.GetByIdAsync(tournamentId);
        if (tournament == null)
        {
            return NotFound(new { error = "Tournament not found" });
        }

        // TournamentId is taken from the route; no need to provide it in the body

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var game = await _gameService.CreateAsync(tournamentId, createDTO);
            return CreatedAtAction(nameof(GetGame), new { tournamentId = tournamentId, id = game.Id }, game);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Update an existing game. The `tournamentId` in the route identifies the parent tournament.
    /// </summary>
    [HttpPut("{id}")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(GameResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GameResponseDTO>> UpdateGame(
        [FromRoute] int tournamentId,
        int id,
        [FromBody] GameUpdateDTO updateDTO)
    {
        _logger.LogInformation("UpdateGame called for tournamentId={TournamentId} id={Id}", tournamentId, id);
        // Verify tournament exists
        var tournament = await _tournamentService.GetByIdAsync(tournamentId);
        if (tournament == null)
        {
            return NotFound(new { error = "Tournament not found" });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var game = await _gameService.UpdateAsync(id, updateDTO);
            
            // Verify the game belongs to this tournament
            if (game.TournamentId != tournamentId)
            {
                return NotFound();
            }
            
            return Ok(game);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteGame([FromRoute] int tournamentId, int id)
    {
        _logger.LogInformation("DeleteGame called for tournamentId={TournamentId} id={Id}", tournamentId, id);
        // Verify tournament exists
        var tournament = await _tournamentService.GetByIdAsync(tournamentId);
        if (tournament == null)
        {
            return NotFound(new { error = "Tournament not found" });
        }

        // Verify game exists and belongs to this tournament
        var game = await _gameService.GetByIdAsync(id);
        if (game == null || game.TournamentId != tournamentId)
        {
            return NotFound();
        }

        var success = await _gameService.DeleteAsync(id);
        if (!success)
        {
            return NotFound();
        }
        return NoContent();
    }
}
