using System;

namespace gOldCleaner.InfrastructureServices
{
    public class Informer : IInformer
    {
        #region Implementation of IInformer

        public void Inform(string message)
        {
            Console.Write(message);
        }

        #endregion
    }
}