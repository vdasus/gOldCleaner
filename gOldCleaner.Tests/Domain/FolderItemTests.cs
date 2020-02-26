using AutoFixture;
using FluentAssertions;
using FsCheck.Xunit;
using gOldCleaner.Domain;
using System;
using Xunit;

namespace gOldCleaner.Tests.Domain
{
    [Trait("Common", "Unit Test")]
    public class FolderItemTests
    {
        [Fact]
        public void Should_Create_Valid_FolderItem()
        {
            var fixture = new Fixture();
            Action sut = () => { fixture.Create<FolderItem>(); };
            sut.Should().NotThrow();
        }
        
        [Theory]
        [InlineData("", "", "")]
        [InlineData("1", "", "")]
        [InlineData("", "1", "")]
        [InlineData("", "", "1")]
        [InlineData(null, null, null)]
        [InlineData(null, "", "")]
        [InlineData("", null, "")]
        [InlineData("", "", null)]
        public void Throw_On_Creating_FolderItem_With_Invalid_Parameters(string description, string folderPath, string searchPattern)
        {
            Action sut = () => new FolderItem(description, folderPath, searchPattern, TimeSpan.MinValue, true);

            sut.Should().ThrowExactly<ArgumentNullException>().WithMessage("Value cannot be null*");

        }

        [Trait("Common", "PBT")]
        [Property(Arbitrary = new[] { typeof(NonNullOrEmptyStringArbitraries) })]
        public void Create_Random_FolderItems(string description, string folderPath, string searchPattern, TimeSpan timeSpan, bool isDelete)
        {
            var sut = new FolderItem(description, folderPath, searchPattern, timeSpan, isDelete);
            sut.Description.Should().Be(description);
        }
    }
}