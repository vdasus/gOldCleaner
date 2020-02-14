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
        }

        public Result Cleanup(FolderItem folder)
        {
            _informer?.LogDebug($"Processing {folder.Description} : {folder.FolderPath}...");

            Result.ErrorMessagesSeparator = "\n";

            var dateDeleteAfter = DateTime.UtcNow - folder.DeleteAfter;
            var result = Result.Ok();

            var searchPattern = folder.SearchPattern.Split(',');

            foreach (var oneSearchPattern in searchPattern)
            {
                try
                {
                    var files = _storage.EnumerateFiles(folder.FolderPath, oneSearchPattern,
                        SearchOption.AllDirectories);
                    foreach (var file in files)
                    {
                        if (_storage.GetLastWriteTimeUtc(file) <= dateDeleteAfter)
                        {
                            var isok = _storage.DeleteFile(file);
                            result = Result.Combine(result, isok);
                            _informer?.Inform(isok.IsSuccess ? "-" : "E");
                            _informer?.LogDebug(isok.IsSuccess ? $"deleted {file}" : isok.Error);

                        }
                        else
                        {
                            _informer?.Inform(".");
                            _informer?.LogTrace($"skipped {file}");
                        }
 }
                }
                catch(Exception ex)
                {
                    _informer?.Inform("E");
                    _informer?.LogError(ex.Message);
                }
            }

            _informer?.LogDebug($"Done {folder.Description} : {folder.FolderPath}");
            return result;
        }

        public Result DeleteEmptyFolders(FolderItem folder)
        {
            var result = Result.Ok();
            if (folder.IsDeleteEmptyFolders)
            {
                result = Result.Combine(result, _storage.CleanEmptyFolders(folder.FolderPath));
                _informer?.Inform("D");
                _informer?.LogDebug($"folder {folder.Description} : {folder.FolderPath} cleaned up");
            }

            return result;
        }

        public IEnumerable<FolderItem> MapFolders(List<FolderItemDto> folders)
        {
            return folders.Select(folder =>
                    new FolderItem(folder.Description, folder.FolderPath, folder.SearchPattern, ConvertStringToTimeSpan(folder.DeleteAfter), folder.IsDeleteEmptyFolders))
                .ToList();
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
    }
}