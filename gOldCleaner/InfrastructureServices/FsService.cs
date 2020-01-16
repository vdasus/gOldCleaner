using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using CSharpFunctionalExtensions;
using gOldCleaner.Domain;
using NLog;

namespace gOldCleaner.InfrastructureServices
{
    public class FsService: IStorageService
    {
        private readonly IFileSystem _fs;
        private readonly ILogger _logger;

        public FsService(IFileSystem fs, ILogger logger)
        {
            _fs = fs;
            _logger = logger;
        }

        public IEnumerable<StorageItem> GetFiles(string folderItemFolderName, string searchPattern)
        {
            var allfiles = Directory.GetFiles(folderItemFolderName, searchPattern, SearchOption.AllDirectories);
            foreach (var file in allfiles)
            {
                yield return new StorageItem(_fs.Path.GetFileName(file), file, _fs.File.GetLastWriteTimeUtc(file), _fs.FileInfo.FromFileName(file).Length);
            }
        }

        public Result CleanEmptyFolders(string path)
        {
            try
            {
                foreach (var directory in Directory.GetDirectories(path))
                {
                    CleanEmptyFolders(directory);
                    if (Directory.GetFiles(directory).Length == 0 &&
                        Directory.GetDirectories(directory).Length == 0)
                    {
                        Directory.Delete(directory, false);
                    }
                }

                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return Result.Failure(ex.Message);
            }
        }

        public Result DeleteFile(StorageItem file)
        {
            try
            {
                _fs.File.Delete(file.Path);
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return Result.Failure(ex.Message);
            }
        }
    }
}
