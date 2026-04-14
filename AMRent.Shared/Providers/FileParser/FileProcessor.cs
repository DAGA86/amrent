namespace AMRent.Shared.Providers.FileParser
{
    public class FileProcessor
    {
        private readonly Dictionary<string, object> _parsers;

        public FileProcessor(Dictionary<string, object> parsers)
        {
            _parsers = parsers;
        }

        public List<IFileRecord> ProcessFile(string[] lines)
        {
            var records = new List<IFileRecord>();

            foreach (var line in lines)
            {
                var recordType = line.Substring(0, 1);
                if (_parsers.TryGetValue(recordType, out var parserObj))
                {
                    var method = parserObj.GetType().GetMethod("Parse");
                    var record = (IFileRecord)method.Invoke(parserObj, new object[] { line });
                    records.Add(record);
                }
            }

            return records;
        }
    }
}
