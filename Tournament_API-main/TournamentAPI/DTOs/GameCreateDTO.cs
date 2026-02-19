using System.ComponentModel.DataAnnotations;

namespace TournamentAPI.DTOs;

public class GameCreateDTO
{
    [Required(ErrorMessage = "Title is required")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 100 characters")]
    public required string Title { get; set; }

    [DataType(DataType.DateTime)]
    [CustomValidation(typeof(GameCreateDTO), nameof(ValidateTime))]
    public DateTime Time { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "TournamentId must be valid")]
    public int TournamentId { get; set; }

    public static ValidationResult? ValidateTime(object? value, ValidationContext context)
    {
        if (value == null)
        {
            return new ValidationResult("Time is required");
        }

        DateTime time;
        if (value is DateTime dt)
        {
            time = dt;
        }
        else if (value is DateTimeOffset dto)
        {
            time = dto.UtcDateTime;
        }
        else if (!DateTime.TryParse(value.ToString(), out time))
        {
            return new ValidationResult("Invalid time format");
        }

        if (time.ToUniversalTime() <= DateTime.UtcNow)
        {
            return new ValidationResult("Time cannot be in the past");
        }
        return ValidationResult.Success;
    }
}
