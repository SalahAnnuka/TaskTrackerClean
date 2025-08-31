using TaskTrackerClean.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace TaskTrackerCleanService;

[TestFixture]
public class TaskReportServiceTest
{
    private TaskReportService _mockService;

    [SetUp]
    public void Setup()
    {
        var services = new ServiceCollection();

        // Register dependencies needed by TaskReportService
        services.AddTransient<TaskReportService>();
        // Add other dependencies like repositories, DbContext, etc.

        var provider = services.BuildServiceProvider();
        _mockService = provider.GetRequiredService<TaskReportService>();
    }

    [Test]
    public async Task Test()
    {
        var generatedReport = await _mockService.GenerateDailyReportAsync();
        Assert.IsNotNull(generatedReport);

        var latestReport = await _mockService.GetLatestReportAsync();
        Assert.IsNotNull(latestReport);

        Assert.AreEqual(generatedReport, latestReport);
    }
}