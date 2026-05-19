namespace ApiQuizGame.Entities
{
    public class Question
    {
        public int Id { get; set; }

        public string Statement { get; set; } = string.Empty;

        public int CategoryId { get; set; }

        public Category Category { get; set; } = null!;

        public ICollection<AnswerOption> AnswerOptions { get; set; } = new List<AnswerOption>();
    }
}
