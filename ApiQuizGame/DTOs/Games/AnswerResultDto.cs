namespace ApiQuizGame.DTOs.Games
{
    public class AnswerResultDto
    {
        public bool IsCorrect { get; set; }

        public string Message { get; set; } = string.Empty;

        public int CurrentLevel { get; set; }

        public decimal AccumulatedPrize { get; set; }

        public string Status { get; set; } = string.Empty;
        public string CorrectOptionText { get; set; } = string.Empty;
    }
}
