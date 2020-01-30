using System;

namespace gOldCleaner.Domain
{
    public class FolderItem
    {
        public string Description { get; }
        public string FolderPath { get; }
        public TimeSpan DeleteAfter { get; }
        public string SearchPattern { get; }
        public bool IsDeleteEmptyFolders { get; }

        public FolderItem(string description, string folderPath, string searchPattern, TimeSpan deleteAfter, bool isDeleteEmptyFolders)
        {
            FolderPath = string.IsNullOrWhiteSpace(folderPath)
                ? throw new ArgumentNullException(nameof(folderPath))
                : folderPath;

            Description = description ?? throw new ArgumentNullException(nameof(description));

            SearchPattern = string.IsNullOrWhiteSpace(searchPattern)
                ? throw new ArgumentNullException(nameof(searchPattern))
                : searchPattern;

            DeleteAfter = deleteAfter;
            IsDeleteEmptyFolders = isDeleteEmptyFolders;
        }
    }
}
