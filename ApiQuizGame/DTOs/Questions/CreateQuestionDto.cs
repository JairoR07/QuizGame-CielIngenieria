using System.ComponentModel.DataAnnotations;
using ApiQuizGame.DTOs.AnswerOptions;

namespace ApiQuizGame.DTOs.Questions
{
    public class CreateQuestionDto
    {
        [Required]
        [MaxLength(500)]
        public string Statement { get; set; } = string.Empty;

        [Required]
        public int CategoryId { get; set; }

        [Required]
        [MinLength(4)]
        [MaxLength(4)]
        public List<CreateAnswerOptionDto> Options { get; set; } = new();
    }


}
