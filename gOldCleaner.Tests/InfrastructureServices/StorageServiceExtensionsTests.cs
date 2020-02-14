using FluentAssertions;
using gOldCleaner.InfrastructureServices;
using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace gOldCleaner.Tests.InfrastructureServices
{
    [Trait("Common", "Unit Test")]
    public class StorageServiceExtensionsTests
    {
        [Fact]
        public void ExceptionsNotThrown()
        {
            var files = new GetTestEnumerable();

            Action sutNotSafe = () => { foreach (var file in files) { } };
            Action sut = () => { foreach (var file in files.SkipExceptions()) { } };

            sutNotSafe.Should().ThrowExactly<UnauthorizedAccessException>();

            sut.Should().NotThrow<UnauthorizedAccessException>();
            sut.Should().NotThrow();
        }

        private class GetTestEnumerable: IEnumerable<string>
        {
            #region Implementation of IEnumerable

            public IEnumerator<string> GetEnumerator()
            {
                yield return "1";
                throw new UnauthorizedAccessException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #endregion
        }
    }
}