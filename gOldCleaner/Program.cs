using DryIoc;
using gOldCleaner.DomainServices;
using gOldCleaner.Dto;
using gOldCleaner.InfrastructureServices;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Configuration;

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

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("gOldCleaner.exe.json");

            var config = builder.Build();

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

                CompositionRoot.BuildStorage(_isTestRun);
                
                var fiSvc = CompositionRoot.Container.Resolve<IFolderItemService>();

                var folders = fiSvc.MapFolders(config.GetSection("FolderList").Get<List<FolderItemDto>>());
                
                foreach (var folder in folders)
                {
                    fiSvc.Cleanup(folder);
                    fiSvc.DeleteEmptyFolders(folder);
                }

                _logger.Info("Done.");

                if (_isPauseOnExit) Console.ReadKey();

                return 0;
            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);
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
    }
}
