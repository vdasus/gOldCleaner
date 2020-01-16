using gOldCleaner.Domain;
using System.Collections.Generic;
using CSharpFunctionalExtensions;

namespace gOldCleaner.InfrastructureServices
{
    public interface IStorageService
    {
        IEnumerable<StorageItem> GetFiles(string folderItemFolderName, string searchPattern);
        Result DeleteFile(StorageItem file);
        Result CleanEmptyFolders(string path);
    }
}