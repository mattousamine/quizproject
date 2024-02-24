namespace quiz.Models
{
    public class AdminPanelCount
    {
        public List<UserQuiz> Quizzes { get; set; }
        public int UserQuizCount { get; set; }
        public int CategoryCount { get; set; }
        public int QuizCount { get; set; }
        public int QuestionCount { get; set; }
        public int OptionCount { get; set; }
    }
}
