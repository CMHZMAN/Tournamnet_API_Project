using Microsoft.EntityFrameworkCore;
using TournamentAPI.Data;
using TournamentAPI.DTOs;
using TournamentAPI.Models;

namespace TournamentAPI.Services;

public interface ITournamentService
{
    Task<IEnumerable<TournamentResponseDTO>> GetAllAsync(string? search = null);
    Task<TournamentResponseDTO?> GetByIdAsync(int id);
    Task<TournamentResponseDTO> CreateAsync(TournamentCreateDTO dto);
    Task<TournamentResponseDTO> UpdateAsync(int id, TournamentUpdateDTO dto);
    Task<bool> DeleteAsync(int id);
}

public class TournamentService : ITournamentService
{
    private readonly TournamentContext _context;

    public TournamentService(TournamentContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TournamentResponseDTO>> GetAllAsync(string? search = null)
    {
        var query = _context.Tournaments.AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            // Validate search - no special characters allowed
            if (!IsValidSearchString(search))
            {
                throw new ArgumentException("Search string contains invalid characters");
            }

            // Case-insensitive partial match
            query = query.Where(t => t.Title.ToLower().Contains(search.ToLower()));
        }

        var tournaments = await query.ToListAsync();
        return tournaments.Select(t => MapToResponseDTO(t)).ToList();
    }

    public async Task<TournamentResponseDTO?> GetByIdAsync(int id)
    {
        var tournament = await _context.Tournaments.FirstOrDefaultAsync(t => t.Id == id);
        return tournament != null ? MapToResponseDTO(tournament) : null;
    }

    public async Task<TournamentResponseDTO> CreateAsync(TournamentCreateDTO dto)
    {
        var tournament = new Tournament
        {
            Title = dto.Title,
            Description = dto.Description,
            MaxPlayers = dto.MaxPlayers,
            Date = dto.Date
        };

        _context.Tournaments.Add(tournament);
        await _context.SaveChangesAsync();

        return MapToResponseDTO(tournament);
    }

    public async Task<TournamentResponseDTO> UpdateAsync(int id, TournamentUpdateDTO dto)
    {
        var tournament = await _context.Tournaments.FirstOrDefaultAsync(t => t.Id == id);
        if (tournament == null)
        {
            throw new KeyNotFoundException($"Tournament with id {id} not found");
        }

        tournament.Title = dto.Title;
        tournament.Description = dto.Description;
        tournament.MaxPlayers = dto.MaxPlayers;
        tournament.Date = dto.Date;

        _context.Tournaments.Update(tournament);
        await _context.SaveChangesAsync();

        return MapToResponseDTO(tournament);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var tournament = await _context.Tournaments.FirstOrDefaultAsync(t => t.Id == id);
        if (tournament == null)
        {
            return false;
        }

        _context.Tournaments.Remove(tournament);
        await _context.SaveChangesAsync();
        return true;
    }

    private static TournamentResponseDTO MapToResponseDTO(Tournament tournament)
    {
        return new TournamentResponseDTO
        {
            Id = tournament.Id,
            Title = tournament.Title,
            Description = tournament.Description,
            MaxPlayers = tournament.MaxPlayers,
            Date = tournament.Date
        };
    }

    private static bool IsValidSearchString(string search)
    {
        // Allow only alphanumeric, spaces, and hyphens
        return System.Text.RegularExpressions.Regex.IsMatch(search, @"^[a-zA-Z0-9\s\-]*$");
    }
}
