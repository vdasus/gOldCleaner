using System;
using AutoFixture;
using FluentAssertions;
using gOldCleaner.Domain;
using Xunit;

namespace gOldCleaner.Tests.Domain
{
    [Trait("Common", "Unit Test")]
    public class FolderItemTests
    {
        [Fact]
        public void ValidFolderItem()
        {
            var fixture = new Fixture();
            Action sut = () => { fixture.Create<FolderItem>(); };
            sut.Should().NotThrow();
        }
        
        [Theory]
        [InlineData("1", "", "")]
        [InlineData("", "1", "")]
        [InlineData("", "", "1")]
        [InlineData(null, "", "")]
        [InlineData("", null, "")]
        [InlineData("", "", null)]
        public void InvalidFolderItem(string description, string folderPath, string searchPattern)
        {
            Action sut = () => new FolderItem(description, folderPath, searchPattern, TimeSpan.MinValue, true);

            sut.Should().ThrowExactly<ArgumentNullException>().WithMessage("Value cannot be null*");

        }
    }
}