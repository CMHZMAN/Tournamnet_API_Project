namespace TournamentAPI.Models;

public class Tournament
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public int MaxPlayers { get; set; }
    public DateTime Date { get; set; }
    
    // Navigation property
    public ICollection<Game> Games { get; set; } = new List<Game>();
}
