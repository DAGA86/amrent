using AMRent.Shared.Providers.FileParser.Mopr.Models;

namespace AMRent.Shared.Providers.FileParser.Mopr.Parsers
{
    public class MoprHeaderParser : IRecordParser<MoprHeader>
    {
        public MoprHeader Parse(string line)
        {
            if (line.Length < 58)
                throw new FormatException($"Header line is too short: {line.Length} chars (expected at least 58)");

            try
            {
                return new MoprHeader
                {
                    FileName = line.Substring(2, 27).Trim(),
                    PreviousFileDate = line.Substring(30, 8).Trim(),
                    PreviousFileSequence = line.Substring(39, 3).Trim(),
                    CurrentFileDate = ParserHelpers.ParseDate(line.Substring(43, 8).Trim(), "yyyyMMdd", "CurrentFileDate"),
                    CurrentFileSequence = line.Substring(52, 3).Trim(),
                    SpecificationVersion = line.Substring(56, 2).Trim()
                };
            }
            catch (Exception ex)
            {
                throw new FormatException($"Error parsing header record: {ex.Message}", ex);
            }
        }
    }
}
