namespace ApiQuizGame.Entities
{
    public class Category
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int Level { get; set; }

        public decimal Prize { get; set; }

        public ICollection<Question> Questions { get; set; } = new List<Question>();
    }
}
