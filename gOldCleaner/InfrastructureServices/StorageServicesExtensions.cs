using System.Collections.Generic;

namespace gOldCleaner.InfrastructureServices
{
    public static class StorageServicesExtensions
    {
        public static IEnumerable<T> SafeWalk<T>(this IEnumerable<T> source)
        {
            using (var enumerator = source.GetEnumerator())
            {
                bool? hasCurrent = null;

                do
                {
                    try
                    {
                        hasCurrent = enumerator.MoveNext();
                    }
                    catch
                    {
                        hasCurrent = null; // we're not sure
                    }

                    if (hasCurrent ?? false) // if not sure, do not return value
                        yield return enumerator.Current;

                } while (hasCurrent ?? true); // if not sure, continue walking
            }
        }
    }
}