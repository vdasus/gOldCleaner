namespace gOldCleaner.Dto
{
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