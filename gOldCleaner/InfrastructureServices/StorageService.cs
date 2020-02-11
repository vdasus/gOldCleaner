using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;

namespace gOldCleaner.InfrastructureServices
{
    public class StorageService : IStorageService
    {
        protected readonly IFileSystem Fs;
        
        public StorageService(IFileSystem fs)
        {
            Fs = fs;
        }

        public bool IsFileExists(string path)
        {
            return Fs.File.Exists(path);
        }

        public bool IsDirectoryExists(string path)
        {
            return Fs.Directory.Exists(path);
        }

        public DateTime GetLastWriteTimeUtc(string fileName)
        {
            if (!Fs.File.Exists(fileName)) throw new FileNotFoundException($"[{fileName}] not found");
            return Fs.File.GetLastWriteTimeUtc(fileName);
        }

        public long GetFileSize(string fileName)
        {
            if (!Fs.File.Exists(fileName)) throw new FileNotFoundException($"[{fileName}] not found");
            return Fs.FileInfo.FromFileName(fileName).Length;
        }

        public string GetFileName(string fileName)
        {
            return Fs.Path.GetFileName(fileName);
        }

        public IEnumerable<string> EnumerateFiles(string folderItemFolderName, string searchPattern,
            SearchOption searchOption)
        {
            return Fs.Directory.EnumerateFiles(folderItemFolderName, searchPattern, searchOption).SkipExceptions();
        }

        public virtual Result CleanEmptyFolders(string path)
        {
            try
            {
                foreach (var directory in Fs.Directory.EnumerateDirectories(path).SkipExceptions())
                {
                    CleanEmptyFolders(directory);
                    if (Fs.Directory.GetFiles(directory).Length == 0 &&
                        Fs.Directory.GetDirectories(directory).Length == 0)
                    {
                        Fs.Directory.Delete(directory, false);
                    }
                }

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(ex.Message);
            }
        }

        public virtual Result DeleteFile(string path)
        {
            try
            {
                Fs.File.Delete(path);
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(ex.Message);
            }
        }
    }
}
