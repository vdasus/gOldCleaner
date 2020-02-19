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
        private readonly IInformer _informer;

        public FolderItemService(IStorageService storage, IInformer informer = null)
        {
            _storage = storage;
            _informer = informer;
            Result.ErrorMessagesSeparator = "\n";
        }

        public Result Cleanup(FolderItem folder)
        {
            _informer?.LogDebug($"Processing [{folder.Description}] : {folder.FolderPath}...");

            var dateDeleteAfter = DateTime.UtcNow - folder.DeleteAfter;
            var result = Result.Success();

            var searchPatterns = folder.SearchPattern.Split(',');

            result = searchPatterns.Aggregate(result,
                (current, searchPattern) => 
                    Result.Combine(current, CleanFolderBySearchPattern(folder, searchPattern, dateDeleteAfter)));

            _informer?.LogDebug($"Done [{folder.Description}] : {folder.FolderPath}");

            return result;
        }

        public Result DeleteEmptyFolders(FolderItem folder)
        {
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
                if(!_storage.IsDirectoryExists(folder.FolderPath)) throw new DirectoryNotFoundException($"{folder.FolderPath} is not exists.");

                result.Add(new FolderItem(folder.Description, folder.FolderPath, folder.SearchPattern, ConvertStringToTimeSpan(folder.DeleteAfter), folder.IsDeleteEmptyFolders));
            }

            return result;
        }

        #region privates

        private Result CleanFolderBySearchPattern(FolderItem folder, string oneSearchPattern, DateTime dateDeleteAfter)
        {
            var result = Result.Success();

            try
            {
                var files = _storage.SafeEnumerateFiles(folder.FolderPath, oneSearchPattern,
                    SearchOption.AllDirectories);

                result = files.Aggregate(result,
                    (current, file) => Result.Combine(current, CleanOneFile(dateDeleteAfter, file)));
            }
            catch (Exception ex)
            {
                _informer?.Inform("E");
                _informer?.LogError(ex.Message);
                return Result.Failure(result.Error);
            }

            return result;
        }

        private Result CleanOneFile(DateTime dateDeleteAfter, string file)
        {
            var result = Result.Success();

            var lastwritedate = _storage.GetLastWriteTimeUtc(file);
            if (lastwritedate <= dateDeleteAfter)
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

            switch (term)
            {
                case "D": return TimeSpan.FromDays(num);
                case "H": return TimeSpan.FromHours(num);
                case "M": return TimeSpan.FromMinutes(num);
                default: throw new ArgumentException(errorString);
            }
        }

        #endregion
    }
}