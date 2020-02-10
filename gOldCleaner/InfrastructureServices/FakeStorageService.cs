using CSharpFunctionalExtensions;
using System;
using System.IO.Abstractions;
using NLog;

namespace gOldCleaner.InfrastructureServices
{
    public sealed class FakeStorageService : StorageService
    {
        private readonly ILogger _logger;

        public FakeStorageService(IFileSystem fs, ILogger logger) : base(fs)
        {
            _logger = logger;
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
                        _logger.Info($"Directory.Delete({directory})");
                    }
                }

                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger?.Error(ex);
                return Result.Failure(ex.Message);
            }
        }

        public override Result DeleteFile(string path)
        {

            _logger.Info($"File.Delete({path});");

            return Result.Success();
        }
    }
}
