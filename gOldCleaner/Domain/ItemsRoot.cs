﻿using CSharpFunctionalExtensions;
using gOldCleaner.InfrastructureServices;
using System;
using System.Collections.Generic;
using System.IO;

namespace gOldCleaner.Domain
{
    public class ItemsRoot: IItemsRoot
    {
        private readonly IStorageService _storage;
        
        public ItemsRoot(IStorageService storage)
        {
            _storage = storage;
        }

        public FolderItem CreateFolderItem(string description, string path, TimeSpan deleteAfter, string searchPattern,
            bool isDeleteEmptyFolders)
        {
            return new FolderItem(this, description, path, deleteAfter, searchPattern,
                isDeleteEmptyFolders);
        }

        public Result CleanEmptyFolders(string path)
        {
            return _storage.CleanEmptyFolders(path);
        }

        public FileItem CreateFileItem(string filePath)
        {
            return new FileItem(_storage, filePath);
        }

        public IEnumerable<FileItem> GetFileItems(string folderItemFolderName, string searchPattern)
        {
            var allfiles = _storage.GetFiles(folderItemFolderName, searchPattern, SearchOption.AllDirectories);
            foreach (var file in allfiles)
            {
                yield return CreateFileItem(file);
            }
        }
    }
}