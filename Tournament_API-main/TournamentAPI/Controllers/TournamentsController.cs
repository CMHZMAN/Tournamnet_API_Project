using Microsoft.AspNetCore.Mvc;
using TournamentAPI.DTOs;
using TournamentAPI.Services;

namespace TournamentAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TournamentsController : ControllerBase
{
    private readonly ITournamentService _tournamentService;
    private readonly RateLimitingService _rateLimitingService;
    private readonly ILogger<TournamentsController> _logger;

    public TournamentsController(
        ITournamentService tournamentService,
        RateLimitingService rateLimitingService,
        ILogger<TournamentsController> logger)
    {
        _tournamentService = tournamentService;
        _rateLimitingService = rateLimitingService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TournamentResponseDTO>>> GetTournaments(
        [FromQuery] string? search = null)
    {
        try
        {
            var tournaments = await _tournamentService.GetAllAsync(search);
            return Ok(tournaments);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TournamentResponseDTO>> GetTournament(int id)
    {
        var tournament = await _tournamentService.GetByIdAsync(id);
        if (tournament == null)
        {
            return NotFound();
        }
        return Ok(tournament);
    }

    [HttpPost]
    public async Task<ActionResult<TournamentResponseDTO>> CreateTournament(
        [FromBody] TournamentCreateDTO createDTO)
    {
        // Check rate limit
        var clientIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        if (!_rateLimitingService.IsRequestAllowed(clientIp))
        {
            return StatusCode(429, new { error = "Too many requests" });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var tournament = await _tournamentService.CreateAsync(createDTO);
            return CreatedAtAction(nameof(GetTournament), new { id = tournament.Id }, tournament);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<TournamentResponseDTO>> UpdateTournament(
        int id,
        [FromBody] TournamentUpdateDTO updateDTO)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var tournament = await _tournamentService.UpdateAsync(id, updateDTO);
            return Ok(tournament);
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
    public async Task<ActionResult> DeleteTournament(int id)
    {
        var success = await _tournamentService.DeleteAsync(id);
        if (!success)
        {
            return NotFound();
        }
        return NoContent();
    }
}
