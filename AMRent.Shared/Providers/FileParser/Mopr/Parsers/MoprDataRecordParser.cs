using AMRent.Shared.Providers.FileParser.Mopr.Models;

namespace AMRent.Shared.Providers.FileParser.Mopr.Parsers
{
    public class MoprDataRecordParser : IRecordParser<MoprDataRecord>
    {
        public MoprDataRecord Parse(string line)
        {
            if (line.Length < 129)
                throw new FormatException($"Data record line is too short: {line.Length} chars (expected at least 129)");

            try
            {
                return new MoprDataRecord
                {
                    ManufacturerCode = line.Substring(1, 5).Trim(),
                    EquipmentNumber = line.Substring(6, 13).Trim(),
                    ContractLicencePlate = line.Substring(19, 15).Trim(),
                    TariffClass = line.Substring(34, 1).Trim(),
                    ExitTollCode = line.Substring(35, 4).Trim(),
                    ExitTollName = line.Substring(39, 18).Trim(),
                    ExitDate = ParserHelpers.ParseDate(line.Substring(57, 14).Trim(), "yyyyMMddHHmmss", "ExitDate"),
                    EntryTollCode = line.Substring(71, 4).Trim(),
                    EntryTollName = line.Substring(75, 18).Trim(),
                    EntryDate = ((line.Substring(93, 14) == "00000000000000") ? null : ParserHelpers.ParseDate(line.Substring(93, 14).Trim(), "yyyyMMddHHmmss", "EntryDate")),
                    TariffInCents = ParserHelpers.ParseInt(line.Substring(107, 6).Trim(), "TariffInCents"),
                    TransactionCode = line.Substring(113, 12).Trim(),
                    VatCode = line.Substring(125, 2).Trim(),
                    ServiceCode = line.Substring(127, 2).Trim()
                };
            }
            catch (Exception ex)
            {
                throw new FormatException($"Error parsing data record: {ex.Message}", ex);
            }
        }
    }
}
