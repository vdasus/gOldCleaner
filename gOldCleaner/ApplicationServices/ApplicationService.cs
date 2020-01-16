using CSharpFunctionalExtensions;
using gOldCleaner.Domain;
using gOldCleaner.InfrastructureServices;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace gOldCleaner.ApplicationServices
{
    public class ApplicationService : IApplicationService
    {
        private readonly IStorageService _storage;
        private readonly ILogger _logger;

        public ApplicationService(IStorageService storage, ILogger logger)
        {
            _storage = storage;
            _logger = logger;
        }

        public Result<IEnumerable<string>> CleanupFolderReturnFailures(FolderItem folderItem)
        {
            if (folderItem == null) return Result.Failure<IEnumerable<string>>($"{nameof(folderItem)} can't be null");

            var failuresList = new List<string>();

            var dateUtcNow = DateTime.UtcNow;

            try
            {
                _logger.Debug($"Processing {folderItem.FolderName}");

                foreach (var file in _storage.GetFiles(folderItem.FolderName, folderItem.SearchPattern))
                {
                    var dateDeleteAfter = DateTime.UtcNow - folderItem.DeleteAfter;

                    if (file.LastWriteTimeUtc > dateDeleteAfter) continue;

                    _logger.Trace($"Try to delete {file.Name} ...");

                    _storage.DeleteFile(file)
                        .OnFailure(e => failuresList.Add(e));

                    _logger.Debug($"File {file.Path} deleted");
                }

                if (folderItem.IsDeleteEmptyFolders)
                    _storage.CleanEmptyFolders(folderItem.FolderName);

                _logger.Debug("Done.");
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return Result.Failure<IEnumerable<string>>(ex.Message);
            }

            return Result.Success(failuresList.AsEnumerable());
        }

        public TimeSpan ConvertStringToTimeSpan(string timespanString)
        {
            var data = Regex.Match(timespanString, @"(\d+)(\w)");
            var num = int.Parse(data.Groups[1].Value);
            var term = data.Groups[2].Value.ToUpperInvariant();

            switch (term)
            {
                case "D": return TimeSpan.FromDays(num);
                case "M": return TimeSpan.FromMinutes(num);
                case "H": return TimeSpan.FromHours(num);
                default: throw new ArgumentException($"Bad {nameof(FolderItem.DeleteAfter)} parameter {num}{term}");
            }
        }

        public void Inform(IReadOnlyList<string> failuresList)
        {
            if (failuresList.Count != 0)
            {
                Console.WriteLine("Failures:");
                foreach (var failure in failuresList) Console.WriteLine($"\t{failure}");
            }
            else
            {
                Console.WriteLine("All successfully done.");
            }
        }
    }
}
