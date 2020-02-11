using AutoFixture;
using FluentAssertions;
using gOldCleaner.InfrastructureServices;
using Xunit;

namespace gOldCleaner.Tests.InfrastructureServices
{
    [Trait("Common", "Unit Test")]
    public class XmlTests
    {
        private readonly Fixture _fixture;

        public XmlTests()
        {
            _fixture = new Fixture();
        }

        [Fact]
        public void SerializeToXmlStringIndented()
        {

            var (obj, xml) = CreateTestData();

            var sut = obj.SerializeToXmlStringIndented<TestData>();

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
            var id = _fixture.Create<int>();
            var name = _fixture.Create<string>();
            var testData = new TestData {Id = id, Name = name};
            var xml = $@"<?xml version=""1.0"" encoding=""utf-16""?>
<TestData xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
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