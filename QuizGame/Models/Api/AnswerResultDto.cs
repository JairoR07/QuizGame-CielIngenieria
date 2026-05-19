namespace QuizGame.Models.Api
{
    public class AnswerResultDto
    {
        public bool IsCorrect { get; set; }

        public string Message { get; set; } = string.Empty;

        public int CurrentLevel { get; set; }

        public decimal AccumulatedPrize { get; set; }

        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Texto de la respuesta correcta (devuelto por la API cuando la respuesta es incorrecta).
        /// Si la API no lo devuelve, queda vacío.
        /// </summary>
        public string CorrectOptionText { get; set; } = string.Empty;

        /// <summary>
        /// Texto de la opción que eligió el jugador (se guarda en el controller antes de llamar la API).
        /// </summary>
        public string SelectedOptionText { get; set; } = string.Empty;
    }
}
