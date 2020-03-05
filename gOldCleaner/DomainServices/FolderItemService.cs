using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;
using gOldCleaner.Domain;
using gOldCleaner.Dto;
using gOldCleaner.InfrastructureServices;

namespace gOldCleaner.DomainServices
{
    public sealed class FolderItemService : IFolderItemService
    {
        private readonly IStorageService _storage;
        private readonly IInformerService _informer;

        public FolderItemService(IStorageService storage, IInformerService informer = null)
        {
            _storage = storage;
            _informer = informer;
            Result.ErrorMessagesSeparator = "\n";
        }

        public Result Cleanup(FolderItem folder)
        {
            if (folder is null) return Result.Failure($"{nameof(folder)} can't be null");

            _informer?.LogDebug($"Processing [{folder.Description}] : {folder.FolderPath}...");

            var dateDeleteBefore = GetCorrectDeleteBefore(folder.DeleteAfter);
            var result = Result.Success();

            var searchPatterns = folder.SearchPattern.Split(',');

            result = searchPatterns.Aggregate(result,
                (current, searchPattern) => 
                    Result.Combine(current, CleanFolderBySearchPattern(folder, searchPattern, dateDeleteBefore)));

            _informer?.LogDebug($"Done [{folder.Description}] : {folder.FolderPath}");

            return result;
        }

        public Result DeleteEmptyFolders(FolderItem folder)
        {
            if (folder is null) return Result.Failure($"{nameof(folder)} can't be null");

            var result = Result.Success();
            if (!folder.IsDeleteEmptyFolders) return result;

            result = _storage.SafeCleanEmptyFolders(folder.FolderPath);

            _informer?.Inform("D");
            _informer?.LogDebug($"folder [{folder.Description}] : {folder.FolderPath} cleaned up from empty folders");

            return result;
        }

        public IEnumerable<FolderItem> MapFolders(List<FolderItemDto> folders)
        {
            var result = new List<FolderItem>();

            foreach (var folder in folders)
            {
                if (!_storage.IsDirectoryExists(folder.FolderPath))
                    throw new DirectoryNotFoundException($"{folder.FolderPath} is not exists.");

                result.Add(new FolderItem(folder.Description, folder.FolderPath, folder.SearchPattern,
                    ConvertStringToTimeSpan(folder.DeleteAfter), folder.IsDeleteEmptyFolders));
            }

            return result;
        }

        #region privates

        private Result CleanFolderBySearchPattern(FolderItem folder, string oneSearchPattern, DateTime dateDeleteBefore)
        {
            if (folder is null) return Result.Failure($"{nameof(folder)} can't be null");

            var result = Result.Success();

            try
            {
                var files = _storage.SafeEnumerateFiles(folder.FolderPath, oneSearchPattern,
                    SearchOption.AllDirectories);

                result = files.Aggregate(result,
                    (current, file) => Result.Combine(current, CleanOneFile(dateDeleteBefore, file)));
            }
            catch (Exception ex)
            {
                _informer?.Inform("E");
                _informer?.LogError(ex.Message);
                return Result.Failure(result.Error);
            }

            return result;
        }

        private Result CleanOneFile(DateTime dateDeleteBefore, string file)
        {
            var result = Result.Success();

            var lastwritedate = _storage.GetLastWriteTimeUtc(file);
            if (lastwritedate <= dateDeleteBefore)
            {
                result = _storage.DeleteFile(file);
                _informer?.Inform(result.IsSuccess ? "-" : "E");
                _informer?.LogDebug(result.IsSuccess ? $"deleted {file}" : $"{result.Error} (LW:{lastwritedate})");
            }
            else
            {
                _informer?.Inform(".");
                _informer?.LogTrace($"skipped {file}");
            }

            return result;
        }

        private TimeSpan ConvertStringToTimeSpan(string timespanString)
        {
            var errorString = $"Bad {nameof(FolderItem.DeleteAfter)} parameter {timespanString}. \\d+(D|H|M) only allowed";

            var data = Regex.Match(timespanString, @"^(\d+)(\w)$");
            if(data.Groups.Count<2) throw new ArgumentException(errorString);

            var num = int.Parse(data.Groups[1]?.Value);
            var term = data.Groups[2].Value.ToUpperInvariant();

            return term switch
            {
                "D" => TimeSpan.FromDays(num),
                "H" => TimeSpan.FromHours(num),
                "M" => TimeSpan.FromMinutes(num),
                _ => throw new ArgumentException(errorString)
            };
        }

        private DateTime GetCorrectDeleteBefore(TimeSpan folderDeleteAfter)
        {
            try
            {
                return DateTime.UtcNow - folderDeleteAfter;
            }
            catch
            {
                _informer?.LogError($"Bad {nameof(folderDeleteAfter)}={folderDeleteAfter}, set to {DateTime.MinValue}");
                return DateTime.MinValue;
            }
        }

        #endregion
    }
}