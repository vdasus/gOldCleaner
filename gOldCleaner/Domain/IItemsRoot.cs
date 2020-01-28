using System;
using System.Collections.Generic;
using CSharpFunctionalExtensions;

namespace gOldCleaner.Domain
{
    public interface IItemsRoot
    {
        FolderItem CreateFolderItem(string description, string path, TimeSpan deleteAfter, string searchPattern,
            bool isDeleteEmptyFolders);

        Result CleanEmptyFolders(string path);

        FileItem CreateFileItem(string filePath);
        IEnumerable<FileItem> GetFileItems(string folderItemFolderName, string searchPattern);
    }
}