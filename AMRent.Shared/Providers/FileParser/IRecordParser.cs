namespace AMRent.Shared.Providers.FileParser
{
    public interface IRecordParser<T> where T : IFileRecord
    {
        T Parse(string line);
    }
}
