using Titanic.API.Requests;

namespace Titanic.API.Tests;

public class BenchmarkRequestTests : TitanicAPITest
{
    [Fact]
    public void GetBenchmarkScores_HasValidBenchmarkProperties()
    {
        // Arrange
        var request = new BenchmarkScoresRequest();
        var benchmarks = request.BlockingPerform(Api);

        // Act
        var benchmark = benchmarks[0];

        // Assert
        Assert.True(benchmark.Id > 0);
        Assert.True(benchmark.Score > 0);
        Assert.True(benchmark.Framerate > 0);
        Assert.NotNull(benchmark.Grade);
        Assert.NotNull(benchmark.Client);
    }

    [Fact]
    public void GetBenchmarkScores_WithPagination_ReturnsData()
    {
        // Arrange & Act
        var request = new BenchmarkScoresRequest(page: 2);
        var benchmarks = request.BlockingPerform(Api);

        // Assert
        Assert.NotNull(benchmarks);
    }
}
