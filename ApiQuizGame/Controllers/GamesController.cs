using ApiQuizGame.DTOs.Categories;
using ApiQuizGame.DTOs.Games;
using ApiQuizGame.DTOs.Questions;
using ApiQuizGame.DTOs.Shared;
using ApiQuizGame.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ApiQuizGame.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GamesController : ControllerBase
    {
        private readonly IGameService _gameService;

        public GamesController(IGameService gameService)
        {
            _gameService = gameService;
        }
        /// <summary>
        /// Starts a new game for a player.
        /// </summary>
        /// <param name="dto">Player information.</param>
        /// <returns>Created game information.</returns>
        [HttpPost("start")]
        public async Task<ActionResult<ApiResponse<GameResponseDto>>> StartGame(StartGameDto dto)
        {
            var result = await _gameService.StartGameAsync(dto);

            return Ok(ApiResponse<GameResponseDto>.Ok(result, "Game started successfully"));
        }
        /// <summary>
        /// Retrieves a random question for the current game level.
        /// </summary>
        /// <param name="gameId">Game identifier.</param>
        /// <returns>Question information.</returns>
        [HttpGet("{gameId}/question")]
        public async Task<ActionResult<ApiResponse<QuestionResponseDto>>> GetCurrentQuestion(int gameId)
        {
            var question = await _gameService.GetCurrentQuestionAsync(gameId);

            return Ok(ApiResponse<QuestionResponseDto>.Ok(question, "Question retrieved successfully"));
        }
        /// <summary>
        /// Answers the current game question.
        /// </summary>
        /// <param name="gameId">Game identifier.</param>
        /// <param name="dto">Selected answer.</param>
        /// <returns>Answer result.</returns>
        [HttpPost("{gameId}/answer")]
        public async Task<ActionResult<ApiResponse<AnswerResultDto>>> AnswerQuestion(int gameId, AnswerQuestionDto dto)
        {
            var result = await _gameService.AnswerQuestionAsync(gameId, dto);

            return Ok(ApiResponse<AnswerResultDto>.Ok(result, result.Message));
        }
        /// <summary>
        /// Withdraws the player from the current game.
        /// </summary>
        /// <param name="gameId">Game identifier.</param>
        /// <returns>Withdraw result.</returns>
        [HttpPost("{gameId}/withdraw")]
        public async Task<ActionResult<ApiResponse<IEnumerable<CategoryDto>>>> WithdrawGame(int gameId)
        {
            var result = await _gameService.WithdrawGameAsync(gameId);

            return Ok(ApiResponse<AnswerResultDto>.Ok(result, "Player withdrew from the game"));
        }
    }
}
