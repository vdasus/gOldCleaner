using CSharpFunctionalExtensions;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;

namespace gOldCleaner.InfrastructureServices
{
    public class StorageService : IStorageService
    {
        protected readonly IFileSystem Fs;
        protected readonly ILogger Logger;

        public StorageService(IFileSystem fs, ILogger logger)
        {
            Fs = fs;
            Logger = logger;
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

        //ASAP bad solution, stops on UnauthorizedAccessException 
        public IEnumerable<string> EnumerateFiles(string folderItemFolderName, string searchPattern,
            SearchOption searchOption)
        {
            return Fs.Directory.EnumerateFiles(folderItemFolderName, searchPattern, searchOption).SafeWalk();
        }

        //ASAP bad solution, stops on UnauthorizedAccessException 
        public virtual Result CleanEmptyFolders(string path)
        {
            try
            {
                foreach (var directory in Fs.Directory.EnumerateDirectories(path).SafeWalk())
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
                Logger?.Error(ex);
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
                Logger?.Error(ex);
                return Result.Failure(ex.Message);
            }
        }
    }
}
