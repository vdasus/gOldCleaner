using AutoFixture;
using FluentAssertions;
using gOldCleaner.InfrastructureServices;
using Xunit;

namespace gOldCleaner.Tests.InfrastructureServices
{
    [Trait("Common", "Unit Test")]
    public class XmlTests
    {
        [Fact]
        public void SerializeToXmlStringIndented()
        {

            var (obj, xml) = CreateTestData();

            var sut = obj.SerializeToPlainXmlString<TestData>();

            sut.Should().Be(xml);
        }

        [Fact]
        public void XmlDeserializeFromString()
        {
            var (obj, xml) = CreateTestData();

            var sut = xml.XmlDeserializeFromString<TestData>();

            sut.Should().BeEquivalentTo(obj);
        }

        private (TestData testData, string xml) CreateTestData()
        {
            var fixture = new Fixture();

            var id = fixture.Create<int>();
            var name = fixture.Create<string>();
            var testData = new TestData {Id = id, Name = name};
            var xml = $@"<TestData>
  <Id>{id}</Id>
  <Name>{name}</Name>
</TestData>";
            return (testData, xml);
        }
    }

    public class TestData
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public TestData() { }
    }
}