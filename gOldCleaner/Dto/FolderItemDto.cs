namespace gOldCleaner.Dto
{
    public sealed class FolderItemDto
    {
        public string Description { get; set; }
        public string FolderPath { get; set; }
        public string DeleteAfter { get; set; }
        public string SearchPattern { get; set; }
        public bool IsDeleteEmptyFolders { get; set; }
    }
}