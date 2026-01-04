using Titanic.API.Requests;
using Xunit;

namespace Titanic.API.Tests
{
    /// <summary>
    /// Tests for server-related API requests.
    /// </summary>
    public class ServerRequestTests : TitanicAPITest
    {
        [Fact]
        public void GetServerStats_ReturnsValidStats()
        {
            // Arrange
            var request = new ServerStatsRequest();

            // Act
            var stats = request.BlockingPerform(Api);

            // Assert
            Assert.NotNull(stats);
            Assert.True(stats.Uptime >= 0);
            Assert.True(stats.TotalUsers >= 0);
            Assert.True(stats.TotalScores >= 0);
            Assert.True(stats.OnlineUsers >= 0);
        }
    }
}
