namespace ApiQuizGame.Entities
{
    public class Game
    {
        public int Id { get; set; }

        public string PlayerName { get; set; } = string.Empty;

        public int CurrentLevel { get; set; }

        public decimal AccumulatedPrize { get; set; }

        public string Status { get; set; } = "InProgress";

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public ICollection<GameRound> GameRounds { get; set; } = new List<GameRound>();
    }
}
