using System.Collections.Generic;
using CSharpFunctionalExtensions;
using gOldCleaner.Domain;
using gOldCleaner.Dto;

namespace gOldCleaner.DomainServices
{
    public interface IFolderItemService : IDomainService
    {
        Result Cleanup(FolderItem folder);
        Result DeleteEmptyFolders(FolderItem folder);
        IEnumerable<FolderItem> MapFolders(List<FolderItemDto> folders);
    }
}