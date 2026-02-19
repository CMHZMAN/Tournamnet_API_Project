using Microsoft.EntityFrameworkCore;
using TournamentAPI.Data;
using TournamentAPI.DTOs;
using TournamentAPI.Models;

namespace TournamentAPI.Services;

public interface IGameService
{
    Task<IEnumerable<GameResponseDTO>> GetAllAsync(int tournamentId);
    Task<GameResponseDTO?> GetByIdAsync(int id);
    Task<GameResponseDTO> CreateAsync(GameCreateDTO dto);
    Task<GameResponseDTO> UpdateAsync(int id, GameUpdateDTO dto);
    Task<bool> DeleteAsync(int id);
}

public class GameService : IGameService
{
    private readonly TournamentContext _context;

    public GameService(TournamentContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<GameResponseDTO>> GetAllAsync(int tournamentId)
    {
        var games = await _context.Games
            .Where(g => g.TournamentId == tournamentId)
            .ToListAsync();

        return games.Select(g => MapToResponseDTO(g)).ToList();
    }

    public async Task<GameResponseDTO?> GetByIdAsync(int id)
    {
        var game = await _context.Games.FirstOrDefaultAsync(g => g.Id == id);
        return game != null ? MapToResponseDTO(game) : null;
    }

    public async Task<GameResponseDTO> CreateAsync(GameCreateDTO dto)
    {
        // Verify tournament exists
        var tournament = await _context.Tournaments.FirstOrDefaultAsync(t => t.Id == dto.TournamentId);
        if (tournament == null)
        {
            throw new KeyNotFoundException($"Tournament with id {dto.TournamentId} not found");
        }

        var game = new Game
        {
            Title = dto.Title,
            Time = dto.Time,
            TournamentId = dto.TournamentId,
            Tournament = tournament
        };

        _context.Games.Add(game);
        await _context.SaveChangesAsync();

        return MapToResponseDTO(game);
    }

    public async Task<GameResponseDTO> UpdateAsync(int id, GameUpdateDTO dto)
    {
        var game = await _context.Games.FirstOrDefaultAsync(g => g.Id == id);
        if (game == null)
        {
            throw new KeyNotFoundException($"Game with id {id} not found");
        }

        game.Title = dto.Title;
        game.Time = dto.Time;

        _context.Games.Update(game);
        await _context.SaveChangesAsync();

        return MapToResponseDTO(game);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var game = await _context.Games.FirstOrDefaultAsync(g => g.Id == id);
        if (game == null)
        {
            return false;
        }

        _context.Games.Remove(game);
        await _context.SaveChangesAsync();
        return true;
    }

    private static GameResponseDTO MapToResponseDTO(Game game)
    {
        return new GameResponseDTO
        {
            Id = game.Id,
            Title = game.Title,
            Time = game.Time,
            TournamentId = game.TournamentId
        };
    }
}
