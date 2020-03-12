using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.IO;

namespace gOldCleaner.InfrastructureServices
{
    public interface IStorageService
    {
        bool IsFileExists(string path);
        bool IsDirectoryExists(string path);

        IEnumerable<string> SafeEnumerateFiles(string folderItemFolderName, string searchPattern,
            SearchOption searchOption);
        DateTime GetLastWriteTimeUtc(string fileName);
        long GetFileSize(string fileName);
        string GetFileName(string fileName);
        Result DeleteFile(string path);
        Result SafeCleanEmptyFolders(string path);
    }
}