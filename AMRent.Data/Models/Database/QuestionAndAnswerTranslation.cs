using System.ComponentModel.DataAnnotations;

namespace AMRent.Data.Models.Database
{
    public class QuestionAndAnswerTranslation : dCore.MultiLanguage.Models.Translation
    {
        public int QuestionAndAnswerId { get; set; }
        [StringLength(256)]
        public string Question { get; set; }
        public string Answer { get; set; }

        public QuestionAndAnswer? QuestionAndAnswer { get; set; }
    }
}
