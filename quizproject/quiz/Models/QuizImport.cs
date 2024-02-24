namespace quiz.Models
{
    public class QuizImport
    {
        public string Category { get; set; }
        public string Quiz { get; set; }
        public string Questions { get; set; }
        public string Option1 { get; set; }
        public string Option2 { get; set; }
        public string Option3 { get; set; }
        public string Option4 { get; set; }
        public string Option5 { get; set; }
        public string CorrectAnswer { get; set; }
        public string FeedbackText { get; set; }
        public int Level { get; set; }
    }

}
