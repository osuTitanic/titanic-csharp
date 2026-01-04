using System;
using Titanic.API.Models;
using Titanic.API.Requests;
using Xunit;

namespace Titanic.API.Tests
{
    /// <summary>
    /// Tests for user-related API requests.
    /// </summary>
    public class UserRequestTests : TitanicAPITest
    {
        [Fact]
        public void GetUser_ValidUserId_ReturnsUser()
        {
            // Arrange
            var request = new GetUserRequest(1);

            // Act
            var user = request.BlockingPerform(Api);

            // Assert
            Assert.NotNull(user);
            Assert.Equal(1, user.Id);
            Assert.False(string.IsNullOrEmpty(user.Name));
            Assert.False(string.IsNullOrEmpty(user.Country));
        }

        [Fact]
        public void GetUser_HasExpectedProperties()
        {
            // Arrange
            var request = new GetUserRequest(1);

            // Act
            var user = request.BlockingPerform(Api);

            // Assert
            Assert.NotNull(user);
            Assert.True(user.CreatedAt > DateTime.MinValue);
            Assert.True(user.LatestActivity > DateTime.MinValue);
        }

        [Fact]
        public void UserLookup_ByUsername_ReturnsUser()
        {
            // Arrange
            var getRequest = new GetUserRequest(1);
            var user = getRequest.BlockingPerform(Api);
            
            var lookupRequest = new UserLookupRequest(user.Name);

            // Act
            var lookedUpUser = lookupRequest.BlockingPerform(Api);

            // Assert
            Assert.NotNull(lookedUpUser);
            Assert.Equal(user.Id, lookedUpUser.Id);
            Assert.Equal(user.Name, lookedUpUser.Name);
        }

        [Fact]
        public void GetUserAchievements_ValidUserId_ReturnsAchievements()
        {
            // Arrange
            var request = new GetUserAchievementsRequest(1);

            // Act
            var achievements = request.BlockingPerform(Api);

            // Assert
            Assert.NotNull(achievements); // User may or may not have achievements, but the list should not be null
        }

        [Fact]
        public void GetUserFriends_ValidUserId_ReturnsFriendsList()
        {
            // Arrange
            var request = new GetUserFriendsRequest(1);

            // Act
            var friends = request.BlockingPerform(Api);

            // Assert
            Assert.NotNull(friends); // User may or may not have friends, but the list should not be null
        }
    }
}
