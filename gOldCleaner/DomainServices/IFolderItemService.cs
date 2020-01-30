using CSharpFunctionalExtensions;
using gOldCleaner.Domain;

namespace gOldCleaner.DomainServices
{
    public interface IFolderItemService
    {
        Result Cleanup(FolderItem folder);
    }
}