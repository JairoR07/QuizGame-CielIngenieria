using ApiQuizGame.DTOs.Games;
using ApiQuizGame.DTOs.Questions;

namespace ApiQuizGame.Services
{
    public interface IGameService
    {
        Task<GameResponseDto> StartGameAsync(StartGameDto dto);
        Task<QuestionResponseDto> GetCurrentQuestionAsync(int gameId);
        Task<AnswerResultDto> AnswerQuestionAsync(int gameId, AnswerQuestionDto dto);
        Task<AnswerResultDto> WithdrawGameAsync(int gameId);
    }
}
