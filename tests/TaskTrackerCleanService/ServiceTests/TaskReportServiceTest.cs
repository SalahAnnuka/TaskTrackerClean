using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using TaskTrackerClean.Application.Services;

namespace TaskTrackerCleanService;

[TestFixture]
public class TaskReportServiceTest
{
    private TaskReportService _service;

    [SetUp]
    public void Setup()
    {

        // Use the shared ServiceProvider from TestSetup
        _service = TestSetup.ServiceProvider.GetRequiredService<TaskReportService>();
    }

    [Test]
    public async Task Test()
    {
        var generatedReport = await _service.GenerateDailyReportAsync();
        Assert.IsNotNull(generatedReport);

        var latestReport = await _service.GetLatestReportAsync();
        Assert.IsNotNull(latestReport);

        Assert.AreEqual(generatedReport, latestReport);
    }
}