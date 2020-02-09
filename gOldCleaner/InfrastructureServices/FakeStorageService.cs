using CSharpFunctionalExtensions;
using System;
using System.IO.Abstractions;
using NLog;

namespace gOldCleaner.InfrastructureServices
{
    public sealed class FakeStorageService : StorageService
    {
        public FakeStorageService(IFileSystem fs, ILogger logger) : base(fs, logger)
        {
        }

        public override Result CleanEmptyFolders(string path)
        {
            try
            {
                foreach (var directory in Fs.Directory.EnumerateDirectories(path))
                {
                    CleanEmptyFolders(directory);
                    if (Fs.Directory.GetFiles(directory).Length == 0 &&
                        Fs.Directory.GetDirectories(directory).Length == 0)
                    {
                        Logger.Info($"Directory.Delete({directory}, false)");
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

        public override Result DeleteFile(string path)
        {

            Logger.Info($"File.Delete({path});");

            return Result.Success();
        }
    }
}
