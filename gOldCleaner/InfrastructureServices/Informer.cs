﻿using System;
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

        #region Implementation of IInformer

        public void Inform(string message)
        {
            Console.Write(message);
        }

        public void LogDebug(string messageToLog)
        {
            if (messageToLog != null) _logger?.Debug(messageToLog);
        }

        public void LogTrace(string messageToLog)
        {
            if (messageToLog != null) _logger?.Trace(messageToLog);
        }

        public void LogError(string messageToLog)
        {
            if (messageToLog != null) _logger?.Error(messageToLog);
        }

        #endregion
    }
}