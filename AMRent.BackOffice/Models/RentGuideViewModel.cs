namespace AMRent.BackOffice.Models
{
    public class RentGuideViewModel
    {
        public List<RentGuideEdit> RentGuides { get; set; }
    }
    public class RentGuideEdit
    {
        public int LanguageId { get; set; }
        public IFormFile File { get; set; }
    }
}
