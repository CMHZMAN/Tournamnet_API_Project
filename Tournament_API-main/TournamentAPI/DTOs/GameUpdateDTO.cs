using System.ComponentModel.DataAnnotations;

namespace TournamentAPI.DTOs;

public class GameUpdateDTO
{
    [Required(ErrorMessage = "Title is required")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 100 characters")]
    public required string Title { get; set; }

    [DataType(DataType.DateTime)]
    [CustomValidation(typeof(GameUpdateDTO), nameof(ValidateTime))]
    public DateTime Time { get; set; }

    public static ValidationResult? ValidateTime(DateTime time, ValidationContext context)
    {
        if (time <= DateTime.Now)
        {
            return new ValidationResult("Time cannot be in the past");
        }
        return ValidationResult.Success;
    }
}
