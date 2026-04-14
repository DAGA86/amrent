using System.Globalization;

namespace AMRent.Shared.Providers.FileParser
{
    public static class ParserHelpers
    {
        public static DateTime ParseDate(string input, string format, string fieldName)
        {
            if (!DateTime.TryParseExact(input, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
                throw new FormatException($"Invalid date format in '{fieldName}': '{input}' (expected format: {format})");

            return result;
        }

        public static int ParseInt(string input, string fieldName)
        {
            if (!int.TryParse(input, out var result))
                throw new FormatException($"Invalid integer format in '{fieldName}': '{input}'");

            return result;
        }
    }
}
