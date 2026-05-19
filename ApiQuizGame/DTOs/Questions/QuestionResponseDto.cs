namespace ApiQuizGame.DTOs.Questions
{
    public class QuestionResponseDto
    {
        public int QuestionId { get; set; }
        public string Statement { get; set; } = string.Empty;
        public int Level { get; set; }
        public decimal Prize { get; set; }
        public List<QuestionOptionDto> Options { get; set; } = new();
    }
}
