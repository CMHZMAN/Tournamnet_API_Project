namespace TournamentAPI.DTOs;

public class TournamentResponseDTO
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public int MaxPlayers { get; set; }
    public DateTime Date { get; set; }
}
