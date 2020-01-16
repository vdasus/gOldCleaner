using System;
using System.Collections.Generic;
using CSharpFunctionalExtensions;
using gOldCleaner.Domain;

namespace gOldCleaner.ApplicationServices
{
    public interface IApplicationService
    {
        Result<IEnumerable<string>> CleanupFolderReturnFailures(FolderItem folderItem);
        void Inform(IReadOnlyList<string> failuresList);
        TimeSpan ConvertStringToTimeSpan(string timespanString);
    }
}