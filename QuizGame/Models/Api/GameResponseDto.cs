namespace QuizGame.Models.Api
{
    public class GameResponseDto
    {
        public int Id { get; set; }

        public string PlayerName { get; set; } = string.Empty;

        public int CurrentLevel { get; set; }

        public decimal AccumulatedPrize { get; set; }

        public string Status { get; set; } = string.Empty;

        public DateTime StartDate { get; set; }
    }
}
