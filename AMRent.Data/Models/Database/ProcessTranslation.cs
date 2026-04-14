using System.ComponentModel.DataAnnotations;

namespace AMRent.Data.Models.Database
{
    public class ProcessTranslation : dCore.MultiLanguage.Models.Translation
    {
        public int ProcessId { get; set; }
        [StringLength(64)]
        public string Title { get; set; }
        [StringLength(256)]
        public string Text { get; set; }

        public Process? Process { get; set; }
    }
}
