namespace AMRent.Data.Models.Database
{
    public class ProcessedFile
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public DateTime ProcessedAt { get; set; }
    }
}
