using AMRent.Shared.Providers.FileParser.Mopr.Models;

namespace AMRent.Shared.Providers.FileParser.Mopr.Parsers
{
    public class MoprTrailerParser : IRecordParser<MoprTrailer>
    {
        public MoprTrailer Parse(string line)
        {
            if (line.Length < 7)
                throw new FormatException($"Trailer line is too short: {line.Length} chars (expected at least 7)");

            try
            {
                return new MoprTrailer
                {
                    RecordCount = ParserHelpers.ParseInt(line.Substring(1, 6).Trim(), "RecordCount")
                };
            }
            catch (Exception ex)
            {
                throw new FormatException($"Error parsing trailer record: {ex.Message}", ex);
            }
        }
    }
}
