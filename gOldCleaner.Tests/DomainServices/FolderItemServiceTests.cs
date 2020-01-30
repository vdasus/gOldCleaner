using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using FluentAssertions;
using gOldCleaner.Domain;
using gOldCleaner.DomainServices;
using gOldCleaner.InfrastructureServices;
using Xunit;

namespace gOldCleaner.Tests.DomainServices
{
    public class FolderItemServiceTests
    {
        [Trait("Common", "Unit Test")]
        [Fact]
        public void Cleanup()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"c:\temp\myfile.txt", new MockFileData("Testing is meh.") },
                { @"c:\temp\test\myfile.txt", new MockFileData("Testing is meh.") }
            });

            var fiSvc = new FolderItemService(new StorageService(fileSystem, null), null);

            var data = new FolderItem("test", @"c:\temp\", "*.txt", TimeSpan.FromDays(1), true);

            var sut = fiSvc.Cleanup(data);

            sut.IsSuccess.Should().BeTrue();
            fileSystem.AllFiles.Count().Should().Be(0);
            fileSystem.AllDirectories.Count().Should().Be(2);
        }
    }
}