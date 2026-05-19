using System.Text;
using System.Text.Json;
using ApiQuizGame.Data;
using ApiQuizGame.Entities;

namespace ApiQuizGame.Services
{
    public class AiQuestionService
    {
        private readonly HttpClient _httpClient;
        private readonly QuizGameDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AiQuestionService> _logger;

        public AiQuestionService(
            HttpClient httpClient,
            QuizGameDbContext context,
            IConfiguration configuration,
            ILogger<AiQuestionService> logger)
        {
            _httpClient = httpClient;
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<Question?> GenerateAndSaveQuestionAsync(int categoryId, int level)
        {
            try
            {
                var apiKey = _configuration["GeminiSettings:ApiKey"];
                var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={apiKey}";

                var levelDescription = level switch
                {
                    1 => "muy fácil, para cualquier persona",
                    2 => "fácil, conocimiento general básico",
                    3 => "moderada, requiere algo de cultura general",
                    4 => "difícil, requiere buen nivel de cultura general",
                    5 => "muy difícil, solo expertos la responderían",
                    _ => "moderada"
                };

                var prompt = $@"Genera una pregunta de cultura general de dificultad {levelDescription}.
La pregunta debe ser en español, interesante y educativa.
Responde ÚNICAMENTE con un JSON válido con exactamente esta estructura, sin texto adicional, sin markdown:
{{
  ""statement"": ""¿Texto de la pregunta?"",
  ""options"": [
    {{ ""text"": ""Opción correcta"", ""isCorrect"": true }},
    {{ ""text"": ""Opción incorrecta 1"", ""isCorrect"": false }},
    {{ ""text"": ""Opción incorrecta 2"", ""isCorrect"": false }},
    {{ ""text"": ""Opción incorrecta 3"", ""isCorrect"": false }}
  ]
}}
Las opciones deben estar en orden aleatorio. Solo una opción puede tener isCorrect: true.";

                var requestBody = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new { text = prompt }
                            }
                        }
                    },
                    generationConfig = new
                    {
                        temperature = 0.8,
                        maxOutputTokens = 500
                    }
                };

                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(url, content);

                if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                {
                    _logger.LogWarning("Gemini rate limit, esperando 60 segundos...");
                    await Task.Delay(60000);
                    response = await _httpClient.PostAsync(url, content);
                }

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Gemini API error: {StatusCode}", response.StatusCode);
                    return null;
                }

                var responseJson = await response.Content.ReadAsStringAsync();
                var geminiResponse = JsonSerializer.Deserialize<JsonElement>(responseJson);

                var generatedText = geminiResponse
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString();

                if (string.IsNullOrEmpty(generatedText))
                    return null;

                // Limpiar posible markdown
                generatedText = generatedText
                    .Replace("```json", "")
                    .Replace("```", "")
                    .Trim();

                var questionData = JsonSerializer.Deserialize<JsonElement>(generatedText);

                var statement = questionData.GetProperty("statement").GetString();
                var options = questionData.GetProperty("options");

                if (string.IsNullOrEmpty(statement))
                    return null;

                var question = new Question
                {
                    Statement = statement,
                    CategoryId = categoryId,
                    AnswerOptions = new List<AnswerOption>()
                };

                foreach (var option in options.EnumerateArray())
                {
                    question.AnswerOptions.Add(new AnswerOption
                    {
                        Text = option.GetProperty("text").GetString() ?? "",
                        IsCorrect = option.GetProperty("isCorrect").GetBoolean()
                    });
                }

                _context.Questions.Add(question);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Pregunta generada por IA y guardada. Id: {Id}, Nivel: {Level}", question.Id, level);

                return question;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generando pregunta con IA para nivel {Level}", level);
                return null;
            }
        }

        public async Task<List<Question>> GenerateFullGameAsync()
        {
            var questions = new List<Question>();

            for (int level = 1; level <= 5; level++)
            {
                var category = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions
                    .FirstOrDefaultAsync(_context.Categories, c => c.Level == level);

                if (category == null)
                {
                    _logger.LogWarning("No existe categoría para nivel {Level}", level);
                    continue;
                }

                var question = await GenerateAndSaveQuestionAsync(category.Id, level);

                if (question != null)
                    questions.Add(question);

                // Pequeña pausa para no saturar la API
                await Task.Delay(300);
            }

            return questions;
        }
    }
}
