using System;

namespace gOldCleaner.Domain
{
    public class FolderItem
    {
        public string Description { get; private set; }
        public string FolderName { get; private set; }
        public TimeSpan DeleteAfter { get; private set; }
        public string SearchPattern { get; private set; }
        public bool IsDeleteEmptyFolders { get; private set; }


        private FolderItem() { }

        public FolderItem(string description, string folderName, TimeSpan deleteAfter, string searchPattern, bool isDeleteEmptyFolders)
        {
            Description = description ?? throw new ArgumentNullException(nameof(description));
            FolderName = folderName ?? throw new ArgumentNullException(nameof(folderName));
            DeleteAfter = deleteAfter;
            SearchPattern = searchPattern;
            IsDeleteEmptyFolders = isDeleteEmptyFolders;
        }
    }

    public class FolderItemDto
    {
        public string Description { get; set; }
        public string FolderName { get; set; }
        public string DeleteAfter { get; set; }
        public string SearchPattern { get; set; }
        public bool IsDeleteEmptyFolders { get; set; }

        public FolderItemDto() { }

        public FolderItemDto(string description, string folderName, string deleteAfter, string searchPattern, bool isDeleteEmptyFolders)
        {
            Description = description;
            FolderName = folderName;
            DeleteAfter = deleteAfter;
            SearchPattern = searchPattern;
            IsDeleteEmptyFolders = isDeleteEmptyFolders;
        }
    }
}
