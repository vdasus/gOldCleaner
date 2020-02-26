using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;

namespace gOldCleaner.Domain
{
    public sealed class FolderItem: ValueObject
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

        #region Overrides of ValueObject

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Description;
            yield return FolderPath;
            yield return DeleteAfter;
            yield return SearchPattern;
            yield return IsDeleteEmptyFolders;
        }

        #endregion
    }
}
