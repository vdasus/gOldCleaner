using FluentAssertions;
using gOldCleaner.InfrastructureServices;
using Moq;
using NLog;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using Xunit;

namespace gOldCleaner.Tests.InfrastructureServices
{
    [Trait("Common", "Unit Test")]
    public class FakeStorageServiceTests
    {
        [Fact]
        public void CleanEmptyFolders_DeletNotInvoked()
        {
            const string FILE = @"c:\myfile.txt";

            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { FILE, new MockFileData("Data") },
                {$@"{Path.GetPathRoot(FILE)}temp\", new MockDirectoryData() },
                {$@"{Path.GetPathRoot(FILE)}temp2\", new MockDirectoryData() }
            });

            var logger = new Mock<ILogger>();

            var obj = new FakeStorageService(fileSystem, logger.Object);

            var sut = obj.CleanEmptyFolders(Path.GetPathRoot(FILE));

            sut.IsSuccess.Should().BeTrue();

            fileSystem.File.Exists(FILE).Should().BeTrue();
            fileSystem.AllFiles.Count().Should().Be(1);
            fileSystem.AllDirectories.Count().Should().Be(3);

            logger.Verify(x => x.Info(It.IsRegex("temp.")), Times.Exactly(2));
        }
        
        [Fact]
        public void DeleteFile_DeletNotInvoked()
        {
            const string FILE = @"c:\myfile.txt";

            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { FILE, new MockFileData("Data") }
            });

            var logger = new Mock<ILogger>();

            var obj = new FakeStorageService(fileSystem, logger.Object);

            var sut = obj.DeleteFile(FILE);

            sut.IsSuccess.Should().BeTrue();

            fileSystem.File.Exists(FILE).Should().BeTrue();
            fileSystem.AllFiles.Count().Should().Be(1);
            fileSystem.AllDirectories.Count().Should().Be(1);

            logger.Verify(x => x.Info($"File.Delete({FILE});"), Times.Once);
        }
    }
}