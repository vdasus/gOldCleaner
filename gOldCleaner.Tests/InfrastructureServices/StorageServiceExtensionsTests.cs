using FluentAssertions;
using gOldCleaner.InfrastructureServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using Xunit;

namespace gOldCleaner.Tests.InfrastructureServices
{
    [Trait("Common", "Unit Test")]
    public class StorageServiceExtensionsTests
    {
        [Fact]
        public void ExceptionsNotThrown()
        {
            const string FILE = @"c:\myfile.txt";

            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {FILE, new MockFileData("Data") {Attributes = FileAttributes.Hidden}}
            });
            
            var files = Directory.EnumerateFiles(fileSystem.Path.GetPathRoot(FILE), "*.*", SearchOption.AllDirectories);

            Action sutNotSafe = () => { foreach (var file in files) { } };
            Action sut = () => { foreach (var file in files.SkipExceptions()) { } };

            sutNotSafe.Should().ThrowExactly<UnauthorizedAccessException>();

            sut.Should().NotThrow<UnauthorizedAccessException>();
            sut.Should().NotThrow();
        }
    }
}