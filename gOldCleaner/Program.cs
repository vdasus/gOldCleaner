using CSharpFunctionalExtensions;
using DryIoc;
using gOldCleaner.DomainServices;
using gOldCleaner.Dto;
using gOldCleaner.InfrastructureServices;
using NLog;
using System;
using System.Collections.Generic;
using static gOldCleaner.Properties.Settings;

namespace gOldCleaner
{
    internal sealed class Program
    {
        private const string APP_NAME = "gOldCleaner";
        private const string APP_DESCRIPTION = "Cleanup old files";

        private static bool _isRun;
        private static bool _isPauseOnExit;
        
        private static ILogger _logger;
        private static bool _isTestRun;

        static int Main(string[] args)
        {
            _logger = CompositionRoot.Container.Resolve<ILogger>();
            
            _logger.Info(
                $"{APP_NAME} v{System.Reflection.Assembly.GetExecutingAssembly().GetName().Version} type -h or --help or -? to see usage");

            var help = false;
            var p = new OptionSet
            {
                {"r|run", "Go full prpocessing", v => _isRun = v != null},
                {"p|pause-on-exit", "Pause on exit", v => _isPauseOnExit = v != null},
                {"t|test-run", "Just log, no real file processing", v => _isTestRun = v != null},
                {"h|?|help", "show this message and exit.", v => help = v != null}
            };
            
            try
            {
                p.Parse(args);

                if (help)
                {
                    ShowHelp(p);
                    return 0;
                }

                if (!_isRun) return 0;

                RegisterStorage(_isTestRun);
                
                var fiSvc = CompositionRoot.Container.Resolve<IFolderItemService>();
                var folders = fiSvc.MapFolders(Default.FolderList.XmlDeserializeFromString<List<FolderItemDto>>());
                
                foreach (var folder in folders)
                {
                    _logger.Trace($"Processing {folder.FolderPath}...");
                    fiSvc.Cleanup(folder)
                        .Tap(() => _logger.Info($"{folder.FolderPath} cleaned"))
                        .OnFailure(e =>  { _logger.Error(e); });
                    
                    fiSvc.DeleteEmptyFolders(folder)
                        .Tap(() => _logger.Info($"Empty folders in {folder.FolderPath} cleaned"))
                        .OnFailure(e => { _logger.Error(e); });
                }

                _logger.Info("Done.");

                if (_isPauseOnExit) Console.ReadKey();

                return 0;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return 99;
            }
        }

        private static void ShowHelp(OptionSet p)
        {
            Console.WriteLine($"Usage: {APP_NAME} [OPTIONS]+");
            Console.WriteLine(APP_DESCRIPTION);
            Console.WriteLine(string.Empty);
            Console.WriteLine("Options:");
            p.WriteOptionDescriptions(Console.Out);
        }

        private static void RegisterStorage(bool isFake)
        {
            if (isFake)
                CompositionRoot.Container.Register<IStorageService, FakeStorageService>(Reuse.Singleton);
            else
                CompositionRoot.Container.Register<IStorageService, StorageService>(Reuse.Singleton);
        }
    }
}
