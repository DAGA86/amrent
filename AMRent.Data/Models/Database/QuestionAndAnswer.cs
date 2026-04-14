namespace AMRent.Data.Models.Database
{
    public class QuestionAndAnswer
    {
        public int Id { get; set; }

        public ICollection<QuestionAndAnswerTranslation>? Translations { get; set; } = new List<QuestionAndAnswerTranslation>();
    }
}
