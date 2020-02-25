using System;
using AutoFixture;
using FluentAssertions;
using FsCheck;
using FsCheck.Xunit;
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

        //TODO just playing with PBT, remove later
        [Trait("Common", "PBT")]
        [Property]
        public Property CheckProperty(NonEmptyString description, NonEmptyString folderPath, NonEmptyString searchPattern)
        {
            Func<bool> sut = () =>
            {
                try
                {
                    new FolderItem(description.Get, folderPath.Get, searchPattern.Get, TimeSpan.MinValue, true);
                    return true;
                }
                catch
                {
                    return false;
                }
            };
            
            return sut.When(!string.IsNullOrWhiteSpace(description.Get) && !string.IsNullOrWhiteSpace(folderPath.Get) && !string.IsNullOrWhiteSpace(searchPattern.Get))
                .Classify(description.Get.Contains("a"), "Contains [a]")
                .Classify(description.Get.Contains(" "), "Contains [ ]");
        }
    }
}