using System;
using System.IO;
using System.Linq;
using CSharpFunctionalExtensions;
using gOldCleaner.Domain;
using gOldCleaner.InfrastructureServices;
using NLog;

namespace gOldCleaner.DomainServices
{
    public class FolderItemService: IFolderItemService
    {
        private readonly IStorageService _storage;
        private readonly ILogger _logger;
        
        public FolderItemService(IStorageService storage, ILogger logger)
        {
            _storage = storage;
            _logger = logger;
        }

        public Result Cleanup(FolderItem folder)
        {
            Result.ErrorMessagesSeparator = "\n";

            var dateDeleteAfter = DateTime.UtcNow - folder.DeleteAfter;
            var result = Result.Ok();

            var files = _storage.GetFiles(folder.FolderPath, folder.SearchPattern, SearchOption.AllDirectories);

            result = files.Where(file => _storage.GetLastWriteTimeUtc(file) <= dateDeleteAfter)
                .Aggregate(result, (current, file) => Result.Combine(current, _storage.DeleteFile(file)));

            if (folder.IsDeleteEmptyFolders)
                result = Result.Combine(result, _storage.CleanEmptyFolders(folder.FolderPath));

            return result;
        }
    }
}