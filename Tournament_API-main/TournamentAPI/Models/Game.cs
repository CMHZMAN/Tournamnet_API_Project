namespace TournamentAPI.Models;

public class Game
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public DateTime Time { get; set; }
    
    // Foreign key
    public int TournamentId { get; set; }
    
    // Navigation property
    public required Tournament Tournament { get; set; }
}
