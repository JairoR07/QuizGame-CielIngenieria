using System.Text;
using System.Text.Json;
using QuizGame.Models.Api;

namespace QuizGame.Services
{
    public class GameApiService
    {
        private readonly HttpClient _httpClient;

        public GameApiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(configuration["ApiSettings:BaseUrl"]!);
        }
        public async Task<GameResponseDto?> StartGameAsync(string playerName)
        {
            var body = new
            {
                playerName
            };

            var content = new StringContent(
                JsonSerializer.Serialize(body),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync(
                "games/start",
                content);

            var json = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var apiResponse = JsonSerializer.Deserialize<ApiResponse<GameResponseDto>>(json, options);

            return apiResponse?.Data;
        }
        public async Task<QuestionResponseDto?> GetQuestionAsync(int gameId)
        {
            var response = await _httpClient.GetAsync(
                $"games/{gameId}/question");

            var json = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var apiResponse = JsonSerializer.Deserialize<
                ApiResponse<QuestionResponseDto>>(json, options);

            return apiResponse?.Data;
        }
        public async Task<AnswerResultDto?> AnswerQuestionAsync(int gameId, int answerOptionId)
        {
            var body = new
            {
                answerOptionId
            };

            var content = new StringContent(
                JsonSerializer.Serialize(body),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync(
                $"games/{gameId}/answer",
                content);

            var json = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var apiResponse = JsonSerializer.Deserialize<ApiResponse<AnswerResultDto>>(json, options);

            return apiResponse?.Data;
        }
        public async Task<AnswerResultDto?> WithdrawGameAsync(int gameId)
        {
            var response = await _httpClient.PostAsync(
                $"games/{gameId}/withdraw",
                null);

            var json = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var apiResponse = JsonSerializer.Deserialize<ApiResponse<AnswerResultDto>>(json, options);

            return apiResponse?.Data;
        }
    }
}
