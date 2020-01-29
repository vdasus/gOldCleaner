using DryIoc;
using gOldCleaner.Domain;
using gOldCleaner.Dto;
using gOldCleaner.InfrastructureServices;
using gOldCleaner.Properties;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;

namespace gOldCleaner
{
    class Program
    {
        private const string APP_NAME = "gOldCleaner";
        private const string APP_DESCRIPTION = "Cleanup old files";

        private static bool _isRun;
        private static bool _isPauseOnExit;

        static int Main(string[] args)
        {
            var logger = CompositionRoot.Container.Resolve<ILogger>();
            
            logger.Info(
                $"{APP_NAME} v{System.Reflection.Assembly.GetExecutingAssembly().GetName().Version} type -h or --help or -? to see usage");

            var help = false;
            var p = new OptionSet
            {
                {"r|run", "Go full prpocessing", v => _isRun = v != null},
                {"p|pause-on-exit", "Pause on exit", v => _isPauseOnExit = v != null},
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

                var folders = MapFolders(Settings.Default.FolderList.XmlDeserializeFromString<List<FolderItemDto>>());

                foreach (var folder in folders)
                {
                    folder.Cleanup()
                        .Tap(() => logger.Info($"{folder.FolderName} cleaned"))
                        .OnFailure(e =>
                        {
                            logger.Info(e);
                            logger.Error(e);
                        });
                }

                logger.Info("Done.");

                if (_isPauseOnExit) Console.ReadKey();

                return 0;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
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

        public static IEnumerable<FolderItem> MapFolders(List<FolderItemDto> folders)
        {
            var itemsRoot = CompositionRoot.Container.Resolve<IItemsRoot>();
            return folders.Select(folder =>
                    itemsRoot.CreateFolderItem(folder.Description, folder.FolderName,
                        ConvertStringToTimeSpan(folder.DeleteAfter), folder.SearchPattern, folder.IsDeleteEmptyFolders))
                .ToList();
        }

        public static TimeSpan ConvertStringToTimeSpan(string timespanString)
        {
            var data = Regex.Match(timespanString, @"(\d+)(\w)");
            var num = int.Parse(data.Groups[1].Value);
            var term = data.Groups[2].Value.ToUpperInvariant();

            switch (term)
            {
                case "D": return TimeSpan.FromDays(num);
                case "M": return TimeSpan.FromMinutes(num);
                case "H": return TimeSpan.FromHours(num);
                default: throw new ArgumentException($"Bad {nameof(FolderItem.DeleteAfter)} parameter {num}{term}");
            }
        }
    }
}
