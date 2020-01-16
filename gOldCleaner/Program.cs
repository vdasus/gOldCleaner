using CSharpFunctionalExtensions;
using DryIoc;
using gOldCleaner.ApplicationServices;
using gOldCleaner.Domain;
using gOldCleaner.InfrastructureServices;
using gOldCleaner.Properties;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace gOldCleaner
{
    class Program
    {
        private const string APP_NAME = "gOldCleaner";
        private const string APP_DESCRIPTION = "Cleanup old files";

        private static bool _isRun;
        private static IApplicationService _app;

        static int Main(string[] args)
        {
            var logger = CompositionRoot.Container.Resolve<ILogger>();
            _app = CompositionRoot.Container.Resolve<IApplicationService>();

            logger.Info(
                $"{APP_NAME} v{System.Reflection.Assembly.GetExecutingAssembly().GetName().Version} type -h or --help or -? to see usage");

            var help = false;
            var p = new OptionSet
            {
                {"r|run", "Go full prpocessing", v => _isRun = v != null},
                {"h|?|help", "show this message and exit.", v => help = v != null}
            };
            var failuresList = new List<string>();
            try
            {
                p.Parse(args);

                if (help)
                {
                    ShowHelp(p);
                    return 0;
                }

                if (!_isRun) return 0;

                var folders = GetFoldersFromSettings();

                foreach (var folder in folders)
                {
                    _app.CleanupFolderReturnFailures(folder)
                        .Tap(x => failuresList.AddRange(x))
                        .OnFailure(e => logger.Error(e));
                }

                _app.Inform(failuresList);

                logger.Info("Done.");

                return 0;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return 99;
            }
        }

        private static IEnumerable<FolderItem> GetFoldersFromSettings()
        {
            var folders = Settings.Default.FolderList.XmlDeserializeFromString<List<FolderItemDto>>();

            return folders.Select(folder =>
                    new FolderItem(folder.Description, folder.FolderName,
                        _app.ConvertStringToTimeSpan(folder.DeleteAfter), folder.SearchPattern, folder.IsDeleteEmptyFolders))
                .ToList();
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
