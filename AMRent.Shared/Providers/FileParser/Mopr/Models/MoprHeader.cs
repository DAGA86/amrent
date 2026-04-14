namespace AMRent.Shared.Providers.FileParser.Mopr.Models
{
    public class MoprHeader : IFileRecord
    {
        public string RecordType => "H";
        public string FileName { get; set; }
        public string PreviousFileDate { get; set; }
        public string PreviousFileSequence { get; set; }
        public DateTime CurrentFileDate { get; set; }
        public string CurrentFileSequence { get; set; }
        public string SpecificationVersion { get; set; }
    }
}
