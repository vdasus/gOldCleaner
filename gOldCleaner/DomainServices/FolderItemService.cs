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
    public sealed class FolderItemService: IFolderItemService
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

        public IEnumerable<FolderItem> MapFolders(List<FolderItemDto> folders)
        {
            return folders.Select(folder =>
                    new FolderItem(folder.Description, folder.FolderPath, folder.SearchPattern, ConvertStringToTimeSpan(folder.DeleteAfter), folder.IsDeleteEmptyFolders))
                .ToList();
        }

        public TimeSpan ConvertStringToTimeSpan(string timespanString)
        {
            var data = Regex.Match(timespanString, @"(\d+)(\w)");
            var num = int.Parse(data.Groups[1].Value);
            var term = data.Groups[2].Value.ToUpperInvariant();

            switch (term)
            {
                case "D": return TimeSpan.FromDays(num);
                case "H": return TimeSpan.FromHours(num);
                case "M": return TimeSpan.FromMinutes(num);
                default: throw new ArgumentException($"Bad {nameof(FolderItem.DeleteAfter)} parameter {num}{term}");
            }
        }
    }
}