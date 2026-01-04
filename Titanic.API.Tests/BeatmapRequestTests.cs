using System;
using Titanic.API.Models;
using Titanic.API.Requests;
using Xunit;

namespace Titanic.API.Tests
{
    /// <summary>
    /// Tests for beatmap-related API requests.
    /// </summary>
    public class BeatmapRequestTests : TitanicAPITest
    {
        [Fact]
        public void GetBeatmap_ValidId_ReturnsBeatmap()
        {
            // Arrange
            var request = new BeatmapRequest(75);

            // Act
            var beatmap = request.BlockingPerform(Api);

            // Assert
            Assert.NotNull(beatmap);
            Assert.Equal(75, beatmap.Id);
            Assert.False(string.IsNullOrEmpty(beatmap.Version));
        }

        [Fact]
        public void GetBeatmap_HasExpectedProperties()
        {
            // Arrange
            var request = new BeatmapRequest(75);

            // Act
            var beatmap = request.BlockingPerform(Api);

            // Assert
            Assert.NotNull(beatmap);
            Assert.True(beatmap.SetId > 0);
            Assert.True(beatmap.Mode >= 0 && beatmap.Mode <= 3);
            Assert.False(string.IsNullOrEmpty(beatmap.MD5));
            Assert.False(string.IsNullOrEmpty(beatmap.Filename));
        }

        [Fact]
        public void BeatmapLookup_ByChecksum_ReturnsBeatmap()
        {
            // Arrange: First get a beatmap to obtain its checksum
            var getRequest = new BeatmapRequest(75);
            var beatmap = getRequest.BlockingPerform(Api);

            // Now create a lookup request using the MD5 checksum
            var lookupRequest = new BeatmapLookupRequest(beatmap.MD5);

            // Act
            var lookedUpBeatmap = lookupRequest.BlockingPerform(Api);

            // Assert
            Assert.NotNull(lookedUpBeatmap);
            Assert.Equal(beatmap.Id, lookedUpBeatmap.Id);
            Assert.Equal(beatmap.MD5, lookedUpBeatmap.MD5);
        }

        [Fact]
        public void GetBeatmapSet_ValidId_ReturnsBeatmapSet()
        {
            // Arrange
            var beatmapRequest = new BeatmapRequest(75);
            var beatmap = beatmapRequest.BlockingPerform(Api);
            var request = new BeatmapSetRequest(beatmap.SetId);

            // Act
            var beatmapSet = request.BlockingPerform(Api);

            // Assert
            Assert.NotNull(beatmapSet);
            Assert.Equal(beatmap.SetId, beatmapSet.Id);
            Assert.NotNull(beatmapSet.Beatmaps);
            Assert.NotEmpty(beatmapSet.Beatmaps);
        }

        [Fact]
        public void GetBeatmapSet_ContainsExpectedBeatmap()
        {
            // Arrange
            var beatmapRequest = new BeatmapRequest(75);
            var beatmap = beatmapRequest.BlockingPerform(Api);
            var setRequest = new BeatmapSetRequest(beatmap.SetId);

            // Act
            var beatmapSet = setRequest.BlockingPerform(Api);

            // Assert
            Assert.NotNull(beatmapSet);
            Assert.Contains(beatmapSet.Beatmaps, b => b.Id == beatmap.Id);
        }
    }
}
