using CSharpFunctionalExtensions;
using NLog;
using System;
using System.IO;
using System.IO.Abstractions;

namespace gOldCleaner.InfrastructureServices
{
    public class StorageService: IStorageService
    {
        private readonly IFileSystem _fs;
        private readonly ILogger _logger;
        
        public StorageService(IFileSystem fs, ILogger logger)
        {
            _fs = fs;
            _logger = logger;
        }

        public DateTime GetLastWriteTimeUtc(string fileName)
        {
            if(!_fs.File.Exists(fileName)) throw new FileNotFoundException($"[{fileName}] not found");
            return _fs.File.GetLastWriteTimeUtc(fileName);
        }

        public long GetFileSize(string fileName)
        {
            if (!_fs.File.Exists(fileName)) throw new FileNotFoundException($"[{fileName}] not found");
            return _fs.FileInfo.FromFileName(fileName).Length;
        }

        public string GetFileName(string fileName)
        {
            if (!_fs.File.Exists(fileName)) throw new FileNotFoundException($"[{fileName}] not found");
            return _fs.Path.GetFileName(fileName);
        }

        public string[] GetFiles(string folderItemFolderName, string searchPattern, SearchOption searchOption)
        {
            return _fs.Directory.GetFiles(folderItemFolderName, searchPattern, searchOption);
        }

        public Result CleanEmptyFolders(string path)
        {
            try
            {
                foreach (var directory in _fs.Directory.GetDirectories(path))
                {
                    CleanEmptyFolders(directory);
                    if (_fs.Directory.GetFiles(directory).Length == 0 &&
                        _fs.Directory.GetDirectories(directory).Length == 0)
                    {
                        _fs.Directory.Delete(directory, false);
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

        public Result DeleteFile(string path)
        {
            try
            {
                _fs.File.Delete(path);
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
