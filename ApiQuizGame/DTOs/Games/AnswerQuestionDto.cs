using System.ComponentModel.DataAnnotations;

namespace ApiQuizGame.DTOs.Games
{
    public class AnswerQuestionDto
    {
        [Required]
        public int AnswerOptionId { get; set; }
    }
}
