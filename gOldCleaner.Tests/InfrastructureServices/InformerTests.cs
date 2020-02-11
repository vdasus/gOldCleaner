using gOldCleaner.InfrastructureServices;
using Moq;
using Xunit;
using NLog;

namespace gOldCleaner.Tests.InfrastructureServices
{
    [Trait("Common", "Unit Test")]
    public class InformerTests
    {
        [Fact]
        public void Debug()
        {
            var logger = new Mock<ILogger>();
            var obj = new Informer(logger.Object);
            obj.LogDebug("test debug");
            logger.Verify(x=>x.Debug("test debug"), Times.Once);
        }
        
        [Fact]
        public void Trace()
        {
            var logger = new Mock<ILogger>();
            var obj = new Informer(logger.Object);
            obj.LogTrace("test trace");
            logger.Verify(x=>x.Trace("test trace"), Times.Once);
        }
        
        [Fact]
        public void Error()
        {
            var logger = new Mock<ILogger>();
            var obj = new Informer(logger.Object);
            obj.LogError("test error");
            logger.Verify(x=>x.Error("test error"), Times.Once);
        }
    }
}