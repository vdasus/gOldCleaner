using System.IO.Abstractions;
using DryIoc;
using FluentAssertions;
using gOldCleaner.DomainServices;
using gOldCleaner.InfrastructureServices;
using NLog;
using Xunit;

namespace gOldCleaner.Tests
{
    [Trait("Common", "Unit Test")]
    public class CompositionRootTests
    {
        [Fact]
        public void Valid()
        {
            CompositionRoot.BuildStorage(true);

            var sutFs = CompositionRoot.Container.Resolve<IFileSystem>();
            var sutI = CompositionRoot.Container.Resolve<IInformer>();
            var sutL = CompositionRoot.Container.Resolve<ILogger>();
            var sutFis = CompositionRoot.Container.Resolve<IFolderItemService>();

            sutFs.Should().BeOfType<FileSystem>();
            sutI.Should().BeOfType<Informer>();
            sutL.Should().BeOfType<NLog.Logger>();
            sutFis.Should().BeOfType<FolderItemService>();
        }
    }
}