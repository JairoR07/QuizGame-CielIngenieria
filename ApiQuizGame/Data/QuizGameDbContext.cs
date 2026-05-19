using ApiQuizGame.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApiQuizGame.Data
{
    public class QuizGameDbContext : DbContext
    //QuizGameDbContext hereda funcionalidades de Entity Framework
    {
        public QuizGameDbContext(DbContextOptions<QuizGameDbContext> options)
        : base(options)
        {
        }

        public DbSet<Category> Categories => Set<Category>();

        public DbSet<Question> Questions => Set<Question>();

        public DbSet<AnswerOption> AnswerOptions => Set<AnswerOption>();

        public DbSet<Game> Games => Set<Game>();

        public DbSet<GameRound> GameRounds => Set<GameRound>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>()
                .Property(c => c.Prize)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Game>()
                .Property(g => g.AccumulatedPrize)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<GameRound>()
                .Property(gr => gr.Prize)
                .HasColumnType("decimal(18,2)");
        }
    }
}
