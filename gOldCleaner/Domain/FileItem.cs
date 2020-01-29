using System;
using CSharpFunctionalExtensions;
using gOldCleaner.InfrastructureServices;

namespace gOldCleaner.Domain
{
    public class FileItem
    {
        private readonly IStorageService _storage;
        public string Path { get; }
        public string Name => _storage.GetFileName(Path);
        public DateTime LastWriteTimeUtc => _storage.GetLastWriteTimeUtc(Path);
        public long Size => _storage.GetFileSize(Path);

        public FileItem(IStorageService storage, string filePath)
        {
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
            Path = filePath ?? throw new ArgumentNullException(nameof(filePath));
        }

        public Result Delete()
        {
            return _storage.DeleteFile(Path);
        }
    }
}
