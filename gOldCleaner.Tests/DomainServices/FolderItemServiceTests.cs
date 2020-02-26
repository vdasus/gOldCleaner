using AutoFixture;
using FluentAssertions;
using FsCheck.Xunit;
using gOldCleaner.Domain;
using gOldCleaner.Dto;
using gOldCleaner.InfrastructureServices;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using gOldCleaner.DomainServices;
using Xunit;

namespace gOldCleaner.Tests.DomainServices
{
    [Trait("Common", "Unit Test")]
    public class FolderItemServiceTests
    {
        [Fact]
        public void Cleanup()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"c:\temp\myfile.txt", new MockFileData("Data") },
                { @"c:\temp\test\test\myfile.txt", new MockFileData("Data") },
                { @"c:\temp\test\myfile.txt", new MockFileData("Data") },
                { @"c:\temp\test\myfile.log", new MockFileData("Data") },
                { @"c:\temp\myfile.log", new MockFileData("Data") }
            });

            var data = new FolderItem("test", @"c:\temp\", "*.txt", TimeSpan.FromDays(1), true);

            var fiSvc = new FolderItemService(new StorageService(fileSystem));

            var sut = fiSvc.Cleanup(data);

            sut.IsSuccess.Should().BeTrue();
            fileSystem.AllFiles.Count().Should().Be(2);
            fileSystem.AllDirectories.Count().Should().Be(4);
        }

        [Property(Arbitrary = new[] { typeof(NonNullOrEmptyStringArbitraries) })]
        public void CleanupWhenValid(FolderItem item)
        {
            var fiSvc = new FolderItemService(new Mock<IStorageService>().Object);

            Action sut = () => fiSvc.Cleanup(item);
            sut.Should().NotThrow();
        }

        [Fact]
        public void DeleteEmptyFolders()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"c:\temp\myfile.txt", new MockFileData("Data") },
                { @"c:\temp\testemty", new MockDirectoryData() },
                { @"c:\temp\testemty2", new MockDirectoryData() },
                { @"c:\temp\test\myfile.txt", new MockFileData("Data") }
            });

            var data = new FolderItem("test", @"c:\temp\", "*.txt", TimeSpan.FromDays(1), true);

            var fiSvc = new FolderItemService(new StorageService(fileSystem));

            var sut = fiSvc.DeleteEmptyFolders(data);

            sut.IsSuccess.Should().BeTrue();
            fileSystem.AllFiles.Count().Should().Be(2);
            fileSystem.AllDirectories.Count().Should().Be(3);
        }

        [Property(Arbitrary = new[] { typeof(NonNullOrEmptyStringArbitraries) })]
        public void DeleteEmptyFoldersWhenValid(FolderItem item)
        {
            var fiSvc = new FolderItemService(new Mock<IStorageService>().Object);

            Action sut = () => fiSvc.DeleteEmptyFolders(item);
            sut.Should().NotThrow();
        }

        [Fact]
        public void CleanupInform()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"c:\temp\myfile.txt", new MockFileData("Data") },
                { @"c:\temp\test\test\myfile.txt", new MockFileData("Data")},
                { @"c:\temp\test\myfile.txt", new MockFileData("Data") },
                { @"c:\temp\test\myfile.log", new MockFileData("Data") },
                { @"c:\temp\myfile.log", new MockFileData("Data") }
            });

            var data = new FolderItem("test", @"c:\temp\", "*.txt", TimeSpan.FromDays(1), true);

            var informer = new Mock<IInformerService>();
            var fiSvc = new FolderItemService(new StorageService(fileSystem), informer.Object);

            var sut = fiSvc.Cleanup(data);

            informer.Verify(x => x.Inform("-"), Times.Exactly(3));
        }

        [Theory]
        [InlineData("5d", 5 * 60 * 24)]
        [InlineData("6h", 6 * 60)]
        [InlineData("7m", 7)]
        public void MapFoldersValid(string timespan, double rez)
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"c:\temp\myfile.txt", new MockFileData("Data") }
            });

            var fixture = new Fixture();
            var data = fixture.Build<FolderItemDto>()
                .With(x => x.DeleteAfter, timespan)
                .With(x=>x.FolderPath, @"c:\temp\")
                .CreateMany(5).ToList();

            var obj = new FolderItemService(new StorageService(fileSystem));
            var sut = obj.MapFolders(data).ToList();

            sut.Count.Should().Be(5);
            sut[0].Should().BeEquivalentTo(data[0], x => x.Excluding(t => t.DeleteAfter));
            sut[0].DeleteAfter.Should().Be(TimeSpan.FromMinutes(rez));
            sut[4].Should().BeEquivalentTo(data[4], x => x.Excluding(t => t.DeleteAfter));
            sut[4].DeleteAfter.Should().Be(TimeSpan.FromMinutes(rez));
        }
        
        [Theory]
        [InlineData("5char_not_D_H_M")]
        [InlineData("Not_a_digit_first")]
        [InlineData("5u")]
        public void MapFoldersInvalid(string timespan)
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"c:\temp\myfile.txt", new MockFileData("Data") }
            });

            var fixture = new Fixture();
            var data = fixture.Build<FolderItemDto>()
                .With(x => x.DeleteAfter, timespan)
                .With(x => x.FolderPath, @"c:\temp\")
                .CreateMany(5).ToList();

            var obj = new FolderItemService(new StorageService(fileSystem));
            Action sut = () => obj.MapFolders(data);

            sut.Should().ThrowExactly<ArgumentException>().WithMessage($"Bad DeleteAfter parameter {timespan}*");
        }

        [Fact]
        public void MapFoldersWhenDirNotFound()
        {
            var fixture = new Fixture();
            var data = fixture.Build<FolderItemDto>()
                .With(x => x.DeleteAfter, "30d")
                .With(x=>x.FolderPath, "c:\\TEMP\\nonexistingfolderBEE34A23-572A-4C0E-BA9B-FF0A9671ACF7")
                .CreateMany(5).ToList();

            var obj = new FolderItemService(Mock.Of<IStorageService>());
            Action sut = () => obj.MapFolders(data);

            sut.Should().ThrowExactly<DirectoryNotFoundException>().WithMessage(@"c:\TEMP\nonexistingfolderBEE34A23-572A-4C0E-BA9B-FF0A9671ACF7 is not exists.");
        }
        
        [Fact]
        public void MapFoldersWhenBadFolderInConfig()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"c:\temp\myfile.txt", new MockFileData("Data") }
            });

            var fixture = new Fixture();
            var data = fixture.Build<FolderItemDto>()
                .With(x => x.DeleteAfter, "30d")
                .With(x=>x.FolderPath, "c:\temp")
                .CreateMany(5).ToList();

            var obj = new FolderItemService(new StorageService(fileSystem));
            Action sut = () => obj.MapFolders(data);

            sut.Should().ThrowExactly<DirectoryNotFoundException>().WithMessage(@"c:	emp is not exists.");
        }

    }
}