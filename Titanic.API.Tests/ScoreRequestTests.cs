using Titanic.API.Models;
using Titanic.API.Requests;
using Xunit;

namespace Titanic.API.Tests
{
    /// <summary>
    /// Tests for score-related API requests.
    /// </summary>
    public class ScoreRequestTests : TitanicAPITest
    {
        [Fact]
        public void GetPerformanceRecords_ReturnsRecordsForAllModes()
        {
            // Arrange
            var request = new GetPerformanceRecordsRequest();

            // Act
            var records = request.BlockingPerform(Api);

            // Assert
            Assert.NotNull(records);
            Assert.NotNull(records.Osu);
            Assert.NotNull(records.Taiko);
            Assert.NotNull(records.Ctb);
            Assert.NotNull(records.Mania);
        }

        [Fact]
        public void GetPerformanceRecords_OsuRecord_HasValidData()
        {
            // Arrange
            var request = new GetPerformanceRecordsRequest();

            // Act
            var records = request.BlockingPerform(Api);

            // Assert
            Assert.NotNull(records.Osu);
            Assert.True(records.Osu.Id > 0);
            Assert.True(records.Osu.PP > 0);
            Assert.False(string.IsNullOrEmpty(records.Osu.Grade));
        }

        [Fact]
        public void GetScoreRecords_ReturnsRecordsForAllModes()
        {
            // Arrange
            var request = new GetScoreRecordsRequest();

            // Act
            var records = request.BlockingPerform(Api);

            // Assert
            Assert.NotNull(records);
            Assert.NotNull(records.Osu);
            Assert.NotNull(records.Taiko);
            Assert.NotNull(records.Ctb);
            Assert.NotNull(records.Mania);
        }

        [Fact]
        public void GetScoreRecords_OsuRecord_HasValidData()
        {
            // Arrange
            var request = new GetScoreRecordsRequest();

            // Act
            var records = request.BlockingPerform(Api);

            // Assert
            Assert.NotNull(records.Osu);
            Assert.True(records.Osu.Id > 0);
            Assert.True(records.Osu.TotalScore > 0);
            Assert.False(string.IsNullOrEmpty(records.Osu.Grade));
        }

        [Fact]
        public void GetScoreById_ValidId_ReturnsScore()
        {
            // Arrange: Get a known score ID from performance records
            var recordsRequest = new GetPerformanceRecordsRequest();
            var records = recordsRequest.BlockingPerform(Api);
            var scoreId = records.Osu.Id;

            var request = new GetScoreByIdRequest(scoreId);

            // Act
            var score = request.BlockingPerform(Api);

            // Assert
            Assert.NotNull(score);
            Assert.Equal(scoreId, score.Id);
        }

        [Fact]
        public void GetScoreById_HasExpectedProperties()
        {
            // Arrange
            var recordsRequest = new GetPerformanceRecordsRequest();
            var records = recordsRequest.BlockingPerform(Api);
            var scoreId = records.Osu.Id;

            var request = new GetScoreByIdRequest(scoreId);

            // Act
            var score = request.BlockingPerform(Api);

            // Assert
            Assert.NotNull(score);
            Assert.True(score.UserId > 0);
            Assert.True(score.Mode >= 0 && score.Mode <= 3);
            Assert.NotNull(score.Beatmap);
            Assert.NotNull(score.User);
        }

        [Fact]
        public void GetScoreById_IncludesBeatmapInfo()
        {
            // Arrange
            var recordsRequest = new GetPerformanceRecordsRequest();
            var records = recordsRequest.BlockingPerform(Api);
            var scoreId = records.Osu.Id;

            var request = new GetScoreByIdRequest(scoreId);

            // Act
            var score = request.BlockingPerform(Api);

            // Assert
            Assert.NotNull(score.Beatmap);
            Assert.True(score.Beatmap.Id > 0);
            Assert.False(string.IsNullOrEmpty(score.Beatmap.Version));
        }

        [Fact]
        public void GetScoreById_IncludesUserInfo()
        {
            // Arrange
            var recordsRequest = new GetPerformanceRecordsRequest();
            var records = recordsRequest.BlockingPerform(Api);
            var scoreId = records.Osu.Id;

            var request = new GetScoreByIdRequest(scoreId);

            // Act
            var score = request.BlockingPerform(Api);

            // Assert
            Assert.NotNull(score.User);
            Assert.True(score.User.Id > 0);
            Assert.False(string.IsNullOrEmpty(score.User.Name));
        }
    }
}
