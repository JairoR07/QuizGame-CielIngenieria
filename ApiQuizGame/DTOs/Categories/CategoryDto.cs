namespace ApiQuizGame.DTOs.Categories
{
    public class CategoryDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int Level { get; set; }

        public decimal Prize { get; set; }
    }
}
