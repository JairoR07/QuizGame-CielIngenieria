using ApiQuizGame.Data;
using ApiQuizGame.DTOs.Questions;
using ApiQuizGame.DTOs.Shared;
using ApiQuizGame.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiQuizGame.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuestionsController : ControllerBase
    {
        private readonly QuizGameDbContext _context;

        public QuestionsController(QuizGameDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Creates a new question with four answer options.
        /// </summary>
        /// <param name="dto">Question information.</param>
        /// <returns>Created question identifier.</returns>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<object>>> CreateQuestion(CreateQuestionDto dto)
        {
            if (dto.Options.Count != 4)
            {
                return BadRequest(
                    ApiResponse<string>.Fail(
                        "The question must contain exactly 4 options."));
            }

            if (dto.Options.Count(o => o.IsCorrect) != 1)
            {
                return BadRequest(
                    ApiResponse<string>.Fail(
                        "The question must contain exactly one correct answer."));
            }

            var categoryExists = await _context.Categories
                .AnyAsync(c => c.Id == dto.CategoryId);

            if (!categoryExists)
            {
                return BadRequest(
                    ApiResponse<string>.Fail(
                        "Category does not exist."));
            }

            var question = new Question
            {
                Statement = dto.Statement,
                CategoryId = dto.CategoryId,
                AnswerOptions = dto.Options
                    .Select(o => new AnswerOption
                    {
                        Text = o.Text,
                        IsCorrect = o.IsCorrect
                    })
                    .ToList()
            };

            _context.Questions.Add(question);

            await _context.SaveChangesAsync();

            return Ok(ApiResponse<object>.Ok(
                new
                {
                    questionId = question.Id
                },
                "Question created successfully"));
        }
        /// <summary>
        /// Retrieves all questions with category information.
        /// </summary>
        /// <returns>List of questions.</returns>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<object>>>> GetQuestions()
        {
            var questions = await _context.Questions
                .Include(q => q.Category)
                .Include(q => q.AnswerOptions)
                .Select(q => new
                {
                    q.Id,
                    q.Statement,
                    Category = q.Category.Name,
                    q.Category.Level,
                    Options = q.AnswerOptions.Select(a => new
                    {
                        a.Id,
                        a.Text,
                        a.IsCorrect
                    })
                })
                .ToListAsync();

            return Ok(ApiResponse<IEnumerable<object>>.Ok(questions, "Questions retrieved successfully"));
        }
        /// <summary>
        /// Retrieves a question by identifier.
        /// </summary>
        /// <param name="id">Question identifier.</param>
        /// <returns>Question information.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> GetQuestion(int id)
        {
            var question = await _context.Questions
                .Include(q => q.Category)
                .Include(q => q.AnswerOptions)
                .Where(q => q.Id == id)
                .Select(q => new
                {
                    q.Id,
                    q.Statement,
                    Category = q.Category.Name,
                    q.Category.Level,
                    Options = q.AnswerOptions.Select(a => new
                    {
                        a.Id,
                        a.Text,
                        a.IsCorrect
                    })
                })
                .FirstOrDefaultAsync();

            if (question == null)
            {
                return NotFound(
                    ApiResponse<string>.Fail(
                        "Question not found"));
            }

            return Ok(ApiResponse<object>.Ok(question, "Question retrieved successfully"));
        }
        /// <summary>
        /// Deletes a question.
        /// </summary>
        /// <param name="id">Question identifier.</param>
        /// <returns>Delete result.</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<string>>> DeleteQuestion(int id)
        {
            var question = await _context.Questions.Include(q => q.AnswerOptions).FirstOrDefaultAsync(q => q.Id == id);

            if (question == null)
            {
                return NotFound(ApiResponse<string>.Fail("Question not found"));
            }

            // Verificar si está en uso en alguna partida
            var isInUse = await _context.GameRounds.AnyAsync(gr => gr.QuestionId == id);

            if (isInUse)
            {
                return BadRequest(ApiResponse<string>.Fail(
                    "No se puede eliminar la pregunta porque está siendo usada en una partida activa."));
            }

            _context.AnswerOptions.RemoveRange(question.AnswerOptions);
            _context.Questions.Remove(question);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse<string>.Ok(null, "Question deleted successfully"));
        }
    }
}
