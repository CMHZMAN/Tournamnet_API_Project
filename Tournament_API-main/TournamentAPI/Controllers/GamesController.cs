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
    public async Task<ActionResult<IEnumerable<GameResponseDTO>>> GetGames(int tournamentId)
    {
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
    public async Task<ActionResult<GameResponseDTO>> GetGame(int tournamentId, int id)
    {
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

    [HttpPost]
    public async Task<ActionResult<GameResponseDTO>> CreateGame(
        int tournamentId,
        [FromBody] GameCreateDTO createDTO)
    {
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

        // Validate that TournamentId matches the route parameter
        if (createDTO.TournamentId != tournamentId)
        {
            return BadRequest(new { error = "Tournament ID in body does not match route parameter" });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var game = await _gameService.CreateAsync(createDTO);
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

    [HttpPut("{id}")]
    public async Task<ActionResult<GameResponseDTO>> UpdateGame(
        int tournamentId,
        int id,
        [FromBody] GameUpdateDTO updateDTO)
    {
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
    public async Task<ActionResult> DeleteGame(int tournamentId, int id)
    {
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
