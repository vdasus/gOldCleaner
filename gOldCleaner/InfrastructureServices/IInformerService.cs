namespace gOldCleaner.InfrastructureServices
{
    public interface IInformerService
    {
        void Inform(string message);
        void LogDebug(string messageToLog);
        void LogTrace(string messageToLog);
        void LogError(string messageToLog);
    }
}