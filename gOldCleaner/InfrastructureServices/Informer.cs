using System;
using NLog;

namespace gOldCleaner.InfrastructureServices
{
    public class Informer : IInformer
    {
        private readonly ILogger _logger;

        public Informer(ILogger logger)
        {
            _logger = logger;
        }

        public void Inform(string failuresList)
        {
            Console.WriteLine($"\n{failuresList}");
            _logger.Debug(failuresList);
        }
    }
}
