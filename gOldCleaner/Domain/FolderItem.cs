using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace gOldCleaner.Domain
{
    public class FolderItem
    {
        private readonly IItemsRoot _itemsFactory;

        public string Description { get; private set; }
        public string FolderName { get; private set; }
        public TimeSpan DeleteAfter { get; private set; }
        public string SearchPattern { get; private set; }
        public bool IsDeleteEmptyFolders { get; private set; }

        public IEnumerable<FileItem> Files { get; }

        private FolderItem() { }

        public FolderItem(IItemsRoot itemsRoot, string description, string path, TimeSpan deleteAfter, string searchPattern, bool isDeleteEmptyFolders)
        {
            _itemsFactory = itemsRoot;
            Description = description ?? throw new ArgumentNullException(nameof(description));
            FolderName = path ?? throw new ArgumentNullException(nameof(path));
            DeleteAfter = deleteAfter;
            SearchPattern = searchPattern;
            IsDeleteEmptyFolders = isDeleteEmptyFolders;

            Files = _itemsFactory.GetFileItems(path, searchPattern);
        }

        public Result Cleanup()
        {
            var result = Result.Success();
            Result.ErrorMessagesSeparator = "\n";

            var dateDeleteAfter = DateTime.UtcNow - DeleteAfter;

            result = Files.Where(file => file.LastWriteTimeUtc <= dateDeleteAfter)
                .Aggregate(result, (current, file) => Result.Combine(current, file.Delete()));

            if (IsDeleteEmptyFolders)
                result = Result.Combine(result, _itemsFactory.CleanEmptyFolders(FolderName));

            return result;
        }
    }
}
