using CSharpFunctionalExtensions;
using System;
using System.IO;

namespace gOldCleaner.InfrastructureServices
{
    public interface IStorageService
    {
        bool IsFileExists(string path);
        bool IsDirectoryExists(string path);

        string[] GetFiles(string folderItemFolderName, string searchPattern,
            SearchOption searchOption);
        DateTime GetLastWriteTimeUtc(string filename);
        long GetFileSize(string fileName);
        string GetFileName(string fileName);
        Result DeleteFile(string path);
        Result CleanEmptyFolders(string path);
    }
}