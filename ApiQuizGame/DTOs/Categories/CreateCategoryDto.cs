using System.ComponentModel.DataAnnotations;

namespace ApiQuizGame.DTOs.Categories
{
    public class CreateCategoryDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Range(1, 5)]
        public int Level { get; set; }

        [Range(1, 100000000)]
        public decimal Prize { get; set; }
    }
}
