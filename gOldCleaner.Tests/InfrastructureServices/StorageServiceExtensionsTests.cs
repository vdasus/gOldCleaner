using FluentAssertions;
using gOldCleaner.InfrastructureServices;
using System;
using System.IO;
using Xunit;

namespace gOldCleaner.Tests.InfrastructureServices
{
    [Trait("Common", "Unit Test")]
    public class StorageServiceExtensionsTests
    {
        [Fact]
        //TODO Bad test, try to solve with abstract fs permissions
        public void ExceptionsNotThrown()
        {
            var files = Directory.EnumerateFiles(@"C:\Windows\System32", "*.*", SearchOption.AllDirectories).SkipExceptions();

            Action sut = () => { foreach (var file in files) { } };

            sut.Should().NotThrow();
        }
    }
}