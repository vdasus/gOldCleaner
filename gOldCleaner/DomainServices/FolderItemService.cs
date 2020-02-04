using System;
using System.IO;
using System.Linq;
using CSharpFunctionalExtensions;
using gOldCleaner.Domain;
using gOldCleaner.InfrastructureServices;

namespace gOldCleaner.DomainServices
{
    public class FolderItemService: IFolderItemService
    {
        private readonly IStorageService _storage;

        public FolderItemService(IStorageService storage)
        {
            _storage = storage;
        }

        public Result Cleanup(FolderItem folder)
        {
            Result.ErrorMessagesSeparator = "\n";

            var dateDeleteAfter = DateTime.UtcNow - folder.DeleteAfter;
            var result = Result.Ok();

            var searchPattern = folder.SearchPattern.Split(',');
            foreach (var oneSearchPattern in searchPattern)
            {
                var files = _storage.EnumerateFiles(folder.FolderPath, oneSearchPattern, SearchOption.AllDirectories);

                result = files.Where(file => _storage.GetLastWriteTimeUtc(file) <= dateDeleteAfter)
                    .Aggregate(result, (current, file) => Result.Combine(current, _storage.DeleteFile(file)));
            }

            //TODO move to separate method
            if (folder.IsDeleteEmptyFolders)
                result = Result.Combine(result, _storage.CleanEmptyFolders(folder.FolderPath));

            return result;
        }
    }
}