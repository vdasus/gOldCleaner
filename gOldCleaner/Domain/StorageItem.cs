using System;

namespace gOldCleaner.Domain
{
    public class StorageItem
    {
        public string Name { get; private set; }
        public string Path { get; private set; }
        public DateTime LastWriteTimeUtc { get; private set; }
        public long Size { get; private set; }

        public StorageItem(string fileName, string filePath, DateTime lastWriteTimeUtc, long size)
        {
            Name = fileName ?? throw new ArgumentNullException(nameof(fileName));
            Path = filePath ?? throw new ArgumentNullException(nameof(filePath));
            LastWriteTimeUtc = lastWriteTimeUtc;
            Size = size;
        }
    }
}
