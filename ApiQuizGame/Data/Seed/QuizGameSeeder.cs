using ApiQuizGame.Entities;

namespace ApiQuizGame.Data.Seed
{
    public class QuizGameSeeder
    {
        public static async Task SeedAsync(QuizGameDbContext context)
        {
            if (context.Categories.Any())
                return;

            var categories = new List<Category>
        {
            new() { Name = "Nivel Básico", Level = 1, Prize = 100000 },
            new() { Name = "Nivel Intermedio", Level = 2, Prize = 200000 },
            new() { Name = "Nivel Avanzado", Level = 3, Prize = 500000 },
            new() { Name = "Nivel Experto", Level = 4, Prize = 1000000 },
            new() { Name = "Nivel Maestro", Level = 5, Prize = 5000000 }
        };

            context.Categories.AddRange(categories);

            await context.SaveChangesAsync();
        }
    }
}
