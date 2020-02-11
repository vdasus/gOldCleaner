using AutoFixture;
using FluentAssertions;
using gOldCleaner.Domain;
using gOldCleaner.DomainServices;
using gOldCleaner.Dto;
using gOldCleaner.InfrastructureServices;
using Moq;
using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
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

            var informer = new Mock<IInformer>();
            var fiSvc = new FolderItemService(new StorageService(fileSystem), informer.Object);

            var sut = fiSvc.Cleanup(data);

            informer.Verify(x=>x.Inform("-"), Times.Exactly(3));
        }

        [Fact]
        public void MapFolders()
        {
            var fixture = new Fixture();
            var data = fixture.Build<FolderItemDto>().With(x => x.DeleteAfter, "5d").CreateMany(5).ToList();

            var obj = new FolderItemService(new Mock<IStorageService>().Object);
            var sut = obj.MapFolders(data).ToList();

            sut.Count.Should().Be(5);
            sut[0].Should().BeEquivalentTo(data[0], x=>x.Excluding(t=>t.DeleteAfter));
            sut[0].DeleteAfter.Should().Be(TimeSpan.FromDays(5));
            sut[4].Should().BeEquivalentTo(data[4], x=>x.Excluding(t=>t.DeleteAfter));
            sut[4].DeleteAfter.Should().Be(TimeSpan.FromDays(5));
        }

        [Theory]
        [InlineData("5d", 5*60*24)]
        [InlineData("6h", 6*60)]
        [InlineData("7m", 7)]
        public void ConvertStringToTimeSpan(string str, double rez)
        {
            var obj = new FolderItemService(new Mock<IStorageService>().Object);
            var sut = obj.ConvertStringToTimeSpan(str);
            sut.Should().Be(TimeSpan.FromMinutes(rez));
        }
        
        [Theory]
        [InlineData("5char_not_D_H_M")]
        [InlineData("Not_a_digit_first")]
        [InlineData("5u")]
        public void ConvertStringToTimeSpanInvalid(string data)
        {
            var obj = new FolderItemService(new Mock<IStorageService>().Object);
            Action sut = () => obj.ConvertStringToTimeSpan(data);

            sut.Should().ThrowExactly<ArgumentException>().WithMessage("Bad DeleteAfter parameter*");
        }
    }
}