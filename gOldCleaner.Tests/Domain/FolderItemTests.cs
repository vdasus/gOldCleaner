using FluentAssertions;
using gOldCleaner.Domain;
using gOldCleaner.InfrastructureServices;
using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using Xunit;

namespace gOldCleaner.Tests.Domain
{
    public class FolderItemTests
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

            var isroot = new ItemsRoot(new StorageService(fileSystem, null));

            var obj = new FolderItem(isroot, "test", @"c:\temp\", TimeSpan.FromDays(1), "*.txt", true);

            var sut = obj.Cleanup();

            sut.IsSuccess.Should().BeTrue();
            fileSystem.AllFiles.Count().Should().Be(0);
            //ASAP it must work fileSystem.AllDirectories.Count().Should().Be(0);
        }
    }
}