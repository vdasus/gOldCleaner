namespace gOldCleaner.InfrastructureServices
{
    public interface IInformerService : IInfrastructureService
    {
        void Inform(string message);
        void LogDebug(string messageToLog);
        void LogTrace(string messageToLog);
        void LogError(string messageToLog);
    }
}