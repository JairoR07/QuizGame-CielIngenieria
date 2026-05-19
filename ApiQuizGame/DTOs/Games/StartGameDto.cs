using System.ComponentModel.DataAnnotations;

namespace ApiQuizGame.DTOs.Games
{
    public class StartGameDto
    {
        [Required]
        [MaxLength(100)]
        public string PlayerName { get; set; } = string.Empty;
    }
}
