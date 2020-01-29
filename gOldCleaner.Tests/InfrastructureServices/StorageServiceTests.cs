using System;
using FluentAssertions;
using gOldCleaner.InfrastructureServices;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using Xunit;

namespace gOldCleaner.Tests.InfrastructureServices
{
    [Trait("Common", "Unit Test")]
    public class StorageServiceTests
    {
        [Fact]
        public void GetLastWriteTimeUtc()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"c:\myfile.txt", new MockFileData("Testing is meh.") }
            });

            var obj = new StorageService(fileSystem, null);
            var sut = obj.GetLastWriteTimeUtc(@"c:\myfile.txt");
            sut.ToString("yyyy-MM-dd HH:mm:ss").Should().Be("2010-01-03 20:00:00");
        }
        
        //TODO review functional
        [Fact]
        public void GetLastWriteTimeUtcWhenNotFound()
        {
            var obj = new StorageService(new FileSystem(), null);
            var sut = obj.GetLastWriteTimeUtc(@"525DE403-8D8D-4E96-B8E1-7270F434D129.unk");
            sut.ToString("yyyy-MM-dd HH:mm:ss").Should().Be("1601-01-01 00:00:00");
        }
        
        [Fact]
        public void GetLastWriteTimeUtcWhenNull()
        {
            var obj = new StorageService(new FileSystem(), null);
            Action sut = () => obj.GetLastWriteTimeUtc(null);
            sut.Should().ThrowExactly<ArgumentNullException>().WithMessage("Value cannot be null.*");
        }
        
        [Fact]
        public void GetFileSize()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"c:\myfile.txt", new MockFileData("Testing is meh.") }
            });

            var obj = new StorageService(fileSystem, null);
            var sut = obj.GetFileSize(@"c:\myfile.txt");
            sut.Should().Be(15L);
        }
        
        [Fact]
        public void GetFileName()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"c:\myfile.txt", new MockFileData("Testing is meh.") }
            });

            var obj = new StorageService(fileSystem, null);
            var sut = obj.GetFileName(@"c:\myfile.txt");
            sut.Should().Be("myfile.txt");
        }
        
        [Fact]
        public void GetFiles()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"c:\myfile.txt", new MockFileData("Testing is meh.") },
                { @"c:\myfile1.tt", new MockFileData("Testing is meh1.") },
                { @"c:\myfile2.txt", new MockFileData("Testing is meh2.") },
                { @"c:\test\myfile.txt", new MockFileData("Testing is meh.") }
            });

            var obj = new StorageService(fileSystem, null);
            var sut = obj.GetFiles(@"c:\", "*.txt", SearchOption.AllDirectories);
            sut.Length.Should().Be(3);
        }

        [Fact]
        public void CleanEmptyFolders()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"c:\myfile.txt", new MockFileData("Testing is meh.") },
                { @"c:\test2\", new MockDirectoryData() },
                { @"c:\test\myfile.txt", new MockFileData("Testing is meh.") }
            });

            var obj = new StorageService(fileSystem, null);

            var sut = obj.CleanEmptyFolders(@"c:\");
            
            sut.IsSuccess.Should().BeTrue();
            fileSystem.AllDirectories.Count().Should().Be(2);
            fileSystem.AllFiles.Count().Should().Be(2);
        }

        [Fact]
        public void DeleteFile()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"c:\myfile.txt", new MockFileData("Testing is meh.") },
                { @"c:\myfile1.tt", new MockFileData("Testing is meh1.") }
            });

            var obj = new StorageService(fileSystem, null);
            var sut = obj.DeleteFile(@"c:\myfile.txt");
            
            sut.IsSuccess.Should().BeTrue();
            fileSystem.AllFiles.Count().Should().Be(1);
        }
    }
}