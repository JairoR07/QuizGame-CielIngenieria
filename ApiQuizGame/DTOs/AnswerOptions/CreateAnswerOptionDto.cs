using System.ComponentModel.DataAnnotations;

namespace ApiQuizGame.DTOs.AnswerOptions
{
    public class CreateAnswerOptionDto
    {
        [Required]
        [MaxLength(300)]
        public string Text { get; set; } = string.Empty;

        public bool IsCorrect { get; set; }
    }
}
