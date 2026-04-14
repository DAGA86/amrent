namespace AMRent.Shared.Providers.FileParser.Mopr.Models
{
    public class MoprTrailer : IFileRecord
    {
        public string RecordType => "F";
        public int RecordCount { get; set; }
    }
}
