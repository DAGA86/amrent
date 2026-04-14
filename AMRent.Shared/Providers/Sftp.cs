using Microsoft.Extensions.Configuration;
using Renci.SshNet;

namespace AMRent.Shared.Providers
{
    public class Sftp : IDisposable
    {
        private readonly SftpClient _client;

        public Sftp(string hostname, int port, string username, string password)
        {
            _client = new SftpClient(hostname, port, username, password);
            _client.Connect();
        }

        public IEnumerable<string> ListFiles(string path)
        {
            return _client.ListDirectory(path)
                .Where(f => !f.IsDirectory && !f.Name.StartsWith("."))
                .Select(f => f.Name);
        }

        public string DownloadFile(string fileName, string localDir)
        {
            var localPath = Path.Combine(localDir, fileName);
            using var fs = File.Create(localPath);
            _client.DownloadFile($"Mopr/{fileName}", fs);
            return localPath;
        }

        public void DeleteFile(string path, string fileName)
        {
            string remoteFilePath = $"{path}/{fileName}";

            if (_client.Exists(remoteFilePath))
            {
                _client.DeleteFile(remoteFilePath);
            }
        }

        public void UploadFile(string localFilePath, string remotePath)
        {
            string fileName = Path.GetFileName(localFilePath);
            string remoteFilePath = $"{remotePath}/{fileName}";

            using var fs = File.OpenRead(localFilePath);
            _client.UploadFile(fs, remoteFilePath);
        }

        public void Dispose() => _client.Dispose();
    }
}