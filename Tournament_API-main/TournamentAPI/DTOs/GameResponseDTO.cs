namespace TournamentAPI.DTOs;

public class GameResponseDTO
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public DateTime Time { get; set; }
    public int TournamentId { get; set; }
}
