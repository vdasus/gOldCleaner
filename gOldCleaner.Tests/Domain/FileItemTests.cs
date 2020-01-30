using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using FluentAssertions;
using gOldCleaner.Domain;
using gOldCleaner.InfrastructureServices;
using Xunit;

namespace gOldCleaner.Tests.Domain
{
    [Trait("Common", "Unit Test")]
    public class FileItemTests
    {
        [Fact]
        public void Delete()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"c:\myfile.txt", new MockFileData("Testing is meh.") }
            });

            var obj = new FileItem(new StorageService(fileSystem, null), @"c:\myfile.txt");
            var sut = obj.Delete();
            sut.IsSuccess.Should().BeTrue();
            fileSystem.AllFiles.Count().Should().Be(0);
        }
        
        [Fact]
        public void GetName()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"c:\myfile.txt", new MockFileData("Testing is meh.") }
            });

            var obj = new FileItem(new StorageService(fileSystem, null), @"c:\myfile.txt");
            var sut = obj.Name;
            sut.Should().Be("myfile.txt");
        }
        
        [Fact]
        public void GetLastWriteTimeUtc()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"c:\myfile.txt", new MockFileData("Testing is meh.") }
            });

            var obj = new FileItem(new StorageService(fileSystem, null), @"c:\myfile.txt");
            var sut = obj.LastWriteTimeUtc;
            sut.Should().Be(DateTime.Parse("2010-01-03 20:00:00"));
        }
        
        [Fact]
        public void GetSize()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"c:\myfile.txt", new MockFileData("Testing is meh.") }
            });

            var obj = new FileItem(new StorageService(fileSystem, null), @"c:\myfile.txt");
            var sut = obj.Size;
            sut.Should().Be(15L);
        }
    }
}