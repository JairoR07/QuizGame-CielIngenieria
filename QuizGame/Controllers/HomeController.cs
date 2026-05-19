using Microsoft.AspNetCore.Mvc;
using QuizGame.Services;

namespace QuizGame.Controllers
{
    public class HomeController : Controller
    {
        private readonly GameApiService _gameApiService;

        public HomeController(GameApiService gameApiService)
        {
            _gameApiService = gameApiService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> StartGame(string playerName)
        {
            var game = await _gameApiService.StartGameAsync(playerName);

            if (game == null)
            {
                return RedirectToAction("Index");
            }

            HttpContext.Session.SetInt32("GameId", game.Id);
            HttpContext.Session.SetString("PlayerName", game.PlayerName);
            HttpContext.Session.SetString("AccumulatedPrize", game.AccumulatedPrize.ToString("N0"));

            return RedirectToAction("Question", new { gameId = game.Id });
        }

        public async Task<IActionResult> Question(int gameId)
        {
            var question = await _gameApiService.GetQuestionAsync(gameId);

            if (question == null)
            {
                return RedirectToAction("Index");
            }

            ViewBag.GameId = gameId;

            return View(question);
        }

        [HttpPost]
        public async Task<IActionResult> AnswerQuestion(int gameId, int answerOptionId, string answerOptionText)
        {
            var result = await _gameApiService.AnswerQuestionAsync(gameId, answerOptionId);

            if (result == null)
                return RedirectToAction("Index");

            result.SelectedOptionText = answerOptionText ?? string.Empty;

            HttpContext.Session.SetString("AccumulatedPrize", result.AccumulatedPrize.ToString("N0"));

            return View("Result", result);
        }

        [HttpPost]
        public async Task<IActionResult> WithdrawGame(int gameId)
        {
            var result = await _gameApiService.WithdrawGameAsync(gameId);

            if (result == null)
            {
                return RedirectToAction("Index");
            }

            return View("Result", result);
        }
    }
}
