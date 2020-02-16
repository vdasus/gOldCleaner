using FluentAssertions;
using gOldCleaner.InfrastructureServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using Xunit;

namespace gOldCleaner.Tests.InfrastructureServices
{
    [Trait("Common", "Unit Test")]
    public class StorageServiceTests
    {
        [Fact]
        public void IsFileExists()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"c:\myfile.txt", new MockFileData("Data") }
            });

            var obj = new StorageService(fileSystem);
            var sut = obj.IsFileExists(@"c:\myfile.txt");
            var sutNotExists = obj.IsFileExists(@"c:\525DE403-8D8D-4E96-B8E1-7270F434D129.txt");

            sut.Should().BeTrue();
            sutNotExists.Should().BeFalse();
        }

        [Fact]
        public void IsDirectoryExists()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"c:\temp\myfile.txt", new MockFileData("Data") }
            });

            var obj = new StorageService(fileSystem);
            var sut = obj.IsDirectoryExists(@"c:\temp");
            var sutDirNotExists = obj.IsDirectoryExists(@"c:\525DE403-8D8D-4E96-B8E1-7270F434D129");

            sut.Should().BeTrue();
            sutDirNotExists.Should().BeFalse();
        }

        [Fact]
        public void GetLastWriteTimeUtc()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {
                    @"c:\myfile.txt",
                    new MockFileData("Data") { LastWriteTime = DateTime.Parse("2020-02-11 19:52").ToUniversalTime() }
                }
            });

            var obj = new StorageService(fileSystem);
            var sut = obj.GetLastWriteTimeUtc(@"c:\myfile.txt");

            sut.Should().Be(DateTime.Parse("2020-02-11 19:52").ToUniversalTime());
        }

        [Theory]
        [InlineData(null, "[] not found")]
        [InlineData(@"525DE403-8D8D-4E96-B8E1-7270F434D129.unk", "[525DE403-8D8D-4E96-B8E1-7270F434D129.unk] not found")]
        public void GetLastWriteTimeUtcWhenNotFoundOrNull(string path, string err)
        {
            var obj = new StorageService(new MockFileSystem());
            Action sut = () => obj.GetLastWriteTimeUtc(path);

            sut.Should().ThrowExactly<FileNotFoundException>().WithMessage(err);
        }

        [Fact]
        public void GetFileSize()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"c:\myfile.txt", new MockFileData("Data") }
            });

            var obj = new StorageService(fileSystem);
            var sut = obj.GetFileSize(@"c:\myfile.txt");

            sut.Should().Be(4L);
        }

        [Theory]
        [InlineData(null, "[] not found")]
        [InlineData(@"525DE403-8D8D-4E96-B8E1-7270F434D129.unk", "[525DE403-8D8D-4E96-B8E1-7270F434D129.unk] not found")]
        public void GetFileSizeWhenNotFoundOrNull(string path, string err)
        {
            var obj = new StorageService(new MockFileSystem());
            Action sut = () => obj.GetFileSize(path);

            sut.Should().ThrowExactly<FileNotFoundException>().WithMessage(err);
        }

        [Theory]
        [InlineData("", "")]
        [InlineData(null, null)]
        [InlineData(@"c:\myfile.txt", "myfile.txt")]
        public void GetFileName(string data, string result)
        {
            var obj = new StorageService(new MockFileSystem());
            var sut = obj.GetFileName(data);

            sut.Should().Be(result);
        }

        [Fact]
        public void GetFiles()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"c:\myfile.txt", new MockFileData("Data") },
                { @"c:\myfile1.tt", new MockFileData("Data1.") },
                { @"c:\myfile2.txt", new MockFileData("Data2.") },
                { @"c:\test\myfile.txt", new MockFileData("Data") }
            });

            var obj = new StorageService(fileSystem);
            var sut = obj.SafeEnumerateFiles(@"c:\", "*.txt", SearchOption.AllDirectories).ToList();

            sut.Count.Should().Be(3);
        }

        [Fact]
        public void CleanEmptyFolders()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"c:\myfile.txt", new MockFileData("Data") },
                { @"c:\test2\", new MockDirectoryData() },
                { @"c:\test3\", new MockDirectoryData() },
                { @"c:\test\myfile.txt", new MockFileData("Data") }
            });

            var obj = new StorageService(fileSystem);

            var sut = obj.SafeCleanEmptyFolders(@"c:\");

            sut.IsSuccess.Should().BeTrue();
            fileSystem.AllDirectories.Count().Should().Be(2);
            fileSystem.AllFiles.Count().Should().Be(2);
        }

        [Fact]
        public void DeleteFile()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"c:\myfile.txt", new MockFileData("Data") },
                { @"c:\myfile1.tt", new MockFileData("Data1.") }
            });

            var obj = new StorageService(fileSystem);
            var sut = obj.DeleteFile(@"c:\myfile.txt");

            sut.IsSuccess.Should().BeTrue();
            fileSystem.AllFiles.Count().Should().Be(1);
            fileSystem.File.Exists(@"c:\myfile.txt").Should().BeFalse();
        }
    }
}