using System;
using System.Collections.Generic;
using CSharpFunctionalExtensions;
using gOldCleaner.Domain;
using gOldCleaner.Dto;

namespace gOldCleaner.DomainServices
{
    public interface IFolderItemService
    {
        Result Cleanup(FolderItem folder);
        IEnumerable<FolderItem> MapFolders(List<FolderItemDto> folders);
        TimeSpan ConvertStringToTimeSpan(string timespanString);
    }
}