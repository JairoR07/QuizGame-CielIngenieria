namespace ApiQuizGame.Entities
{
    public class GameRound
    {
        public int Id { get; set; }

        public int GameId { get; set; }

        public Game Game { get; set; } = null!;

        public int QuestionId { get; set; }

        public Question Question { get; set; } = null!;

        public int? SelectedAnswerOptionId { get; set; }

        public AnswerOption? SelectedAnswerOption { get; set; }

        public int Level { get; set; }

        public decimal Prize { get; set; }

        public bool? IsCorrect { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? AnsweredAt { get; set; }
    }
}
