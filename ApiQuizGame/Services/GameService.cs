using ApiQuizGame.Data;
using ApiQuizGame.DTOs.Games;
using ApiQuizGame.DTOs.Questions;
using ApiQuizGame.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApiQuizGame.Services
{
    public class GameService : IGameService
    {
        private readonly QuizGameDbContext _context;
        private readonly ILogger<GameService> _logger;
        private readonly AiQuestionService _aiQuestionService;

        public GameService(
            QuizGameDbContext context,
            ILogger<GameService> logger,
            AiQuestionService aiQuestionService)
        {
            _context = context;
            _logger = logger;
            _aiQuestionService = aiQuestionService;
        }

        public async Task<GameResponseDto> StartGameAsync(StartGameDto dto)
        {
            var game = new Game
            {
                PlayerName = dto.PlayerName,
                CurrentLevel = 1,
                AccumulatedPrize = 0,
                Status = "InProgress",
                StartDate = DateTime.UtcNow
            };

            _context.Games.Add(game);
            await _context.SaveChangesAsync();

            return new GameResponseDto
            {
                Id = game.Id,
                PlayerName = game.PlayerName,
                CurrentLevel = game.CurrentLevel,
                AccumulatedPrize = game.AccumulatedPrize,
                Status = game.Status,
                StartDate = game.StartDate
            };
        }

        public async Task<QuestionResponseDto> GetCurrentQuestionAsync(int gameId)
        {
            var game = await _context.Games
                .Include(g => g.GameRounds)
                .FirstOrDefaultAsync(g => g.Id == gameId);

            if (game == null)
                throw new Exception("Game not found");

            if (game.Status != "InProgress")
                throw new Exception("Game is not active");

            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Level == game.CurrentLevel);

            if (category == null)
            {
                game.Status = "ConfigurationError";
                game.EndDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                throw new Exception("No existe una categoría configurada para el nivel actual.");
            }

            var usedQuestionIds = game.GameRounds
                .Select(gr => gr.QuestionId)
                .ToList();

            var question = await _context.Questions
                .Include(q => q.AnswerOptions)
                .Where(q => q.CategoryId == category.Id && !usedQuestionIds.Contains(q.Id))
                .OrderBy(q => Guid.NewGuid())
                .FirstOrDefaultAsync();

            // Si no hay preguntas disponibles, generamos una con IA
            if (question == null)
            {
                _logger.LogInformation("No hay preguntas para nivel {Level}. Generando con IA...", game.CurrentLevel);

                question = await _aiQuestionService.GenerateAndSaveQuestionAsync(category.Id, game.CurrentLevel);

                if (question == null)
                {
                    game.Status = "ConfigurationError";
                    game.EndDate = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                    throw new Exception("No hay preguntas disponibles y no se pudo generar una con IA.");
                }

                // Recargar con las opciones
                question = await _context.Questions
                    .Include(q => q.AnswerOptions)
                    .FirstOrDefaultAsync(q => q.Id == question.Id);
            }

            var round = new GameRound
            {
                GameId = game.Id,
                QuestionId = question!.Id,
                Level = game.CurrentLevel,
                Prize = category.Prize,
                CreatedAt = DateTime.UtcNow
            };

            _context.GameRounds.Add(round);
            await _context.SaveChangesAsync();

            return new QuestionResponseDto
            {
                QuestionId = question.Id,
                Statement = question.Statement,
                Level = category.Level,
                Prize = category.Prize,
                Options = question.AnswerOptions
                    .Select(a => new QuestionOptionDto { Id = a.Id, Text = a.Text })
                    .ToList()
            };
        }

        public async Task<AnswerResultDto> AnswerQuestionAsync(int gameId, AnswerQuestionDto dto)
        {
            var game = await _context.Games
                .Include(g => g.GameRounds)
                .FirstOrDefaultAsync(g => g.Id == gameId);

            if (game == null) throw new Exception("Game not found");
            if (game.Status != "InProgress") throw new Exception("Game is not active");

            var currentRound = game.GameRounds
                .OrderByDescending(gr => gr.CreatedAt)
                .FirstOrDefault();

            if (currentRound == null) throw new Exception("No active round");

            var answer = await _context.AnswerOptions
                .FirstOrDefaultAsync(a => a.Id == dto.AnswerOptionId);

            if (answer == null) throw new Exception("Answer not found");

            currentRound.SelectedAnswerOptionId = answer.Id;
            currentRound.AnsweredAt = DateTime.UtcNow;
            currentRound.IsCorrect = answer.IsCorrect;

            var correctAnswer = await _context.AnswerOptions
                .FirstOrDefaultAsync(a => a.QuestionId == answer.QuestionId && a.IsCorrect);

            if (!answer.IsCorrect)
            {
                var lostPrize = game.AccumulatedPrize;
                game.Status = "Lost";
                game.AccumulatedPrize = 0;
                game.EndDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return new AnswerResultDto
                {
                    IsCorrect = false,
                    Message = "Respuesta incorrecta. Juego terminado.",
                    CurrentLevel = game.CurrentLevel,
                    AccumulatedPrize = lostPrize,
                    Status = game.Status,
                    CorrectOptionText = correctAnswer?.Text ?? string.Empty
                };
            }

            game.AccumulatedPrize += currentRound.Prize;

            if (game.CurrentLevel == 5)
            {
                game.Status = "Winner";
                game.EndDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return new AnswerResultDto
                {
                    IsCorrect = true,
                    Message = "¡Felicitaciones! Has ganado el juego.",
                    CurrentLevel = game.CurrentLevel,
                    AccumulatedPrize = game.AccumulatedPrize,
                    Status = game.Status
                };
            }

            game.CurrentLevel++;
            await _context.SaveChangesAsync();

            return new AnswerResultDto
            {
                IsCorrect = true,
                Message = "Respuesta correcta.",
                CurrentLevel = game.CurrentLevel,
                AccumulatedPrize = game.AccumulatedPrize,
                Status = game.Status
            };
        }

        public async Task<AnswerResultDto> WithdrawGameAsync(int gameId)
        {
            var game = await _context.Games
                .FirstOrDefaultAsync(g => g.Id == gameId);

            if (game == null) throw new Exception("Game not found");
            if (game.Status != "InProgress") throw new Exception("Game is not active");

            game.Status = "Withdrawn";
            game.EndDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return new AnswerResultDto
            {
                IsCorrect = true,
                Message = "Conservas tu premio acumulado",
                CurrentLevel = game.CurrentLevel - 1,
                AccumulatedPrize = game.AccumulatedPrize,
                Status = game.Status
            };
        }
    }
}
