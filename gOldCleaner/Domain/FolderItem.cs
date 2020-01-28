using CSharpFunctionalExtensions;
using gOldCleaner.InfrastructureServices;
using System;
using System.Collections.Generic;

namespace gOldCleaner.Domain
{
    public class FolderItem
    {
        private readonly IInformer _informer;
        private readonly IItemsRoot _itemsFactory;

        public string Description { get; private set; }
        public string FolderName { get; private set; }
        public TimeSpan DeleteAfter { get; private set; }
        public string SearchPattern { get; private set; }
        public bool IsDeleteEmptyFolders { get; private set; }

        public IEnumerable<FileItem> Files { get; }

        private FolderItem() { }

        internal FolderItem(IItemsRoot itemsRoot, IInformer informer, string description, string path, TimeSpan deleteAfter, string searchPattern, bool isDeleteEmptyFolders)
        {
            _itemsFactory = itemsRoot;
            _informer = informer;
            Description = description ?? throw new ArgumentNullException(nameof(description));
            FolderName = path ?? throw new ArgumentNullException(nameof(path));
            DeleteAfter = deleteAfter;
            SearchPattern = searchPattern;
            IsDeleteEmptyFolders = isDeleteEmptyFolders;

            Files = _itemsFactory.GetFileItems(path, searchPattern);
        }

        public void Cleanup()
        {
            var dateDeleteAfter = DateTime.UtcNow - DeleteAfter;

            foreach (var file in Files)
            {
                if (file.LastWriteTimeUtc > dateDeleteAfter) continue;

                var result = file.Delete()
                    .OnFailure(_informer.Inform);
                
                if(result.IsSuccess) 
                    _informer.Inform($"{file.Path} deleted");
            }

            if (IsDeleteEmptyFolders)
                _itemsFactory.CleanEmptyFolders(FolderName)
                    .OnFailure(_informer.Inform);
        }
    }
}
