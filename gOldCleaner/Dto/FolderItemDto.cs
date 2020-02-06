namespace gOldCleaner.Dto
{
    public sealed class FolderItemDto
    {
        public string Description { get; set; }
        public string FolderPath { get; set; }
        public string DeleteAfter { get; set; }
        public string SearchPattern { get; set; }
        public bool IsDeleteEmptyFolders { get; set; }

        public FolderItemDto() { }

        public FolderItemDto(string description, string folderPath, string deleteAfter, string searchPattern, bool isDeleteEmptyFolders)
        {
            Description = description;
            FolderPath = folderPath;
            DeleteAfter = deleteAfter;
            SearchPattern = searchPattern;
            IsDeleteEmptyFolders = isDeleteEmptyFolders;
        }
    }
}