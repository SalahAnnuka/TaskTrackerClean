using TaskTrackerClean.Application.Interfaces;

namespace TaskTrackerClean.Application.Tests;

public class ApplicationTest
{
    ITaskService _mockTaskService;

    public ApplicationTest(ITaskService mockTaskService)
    {
        _mockTaskService = mockTaskService;
    }

    [Fact]
    public void Test1()
    {
        _mockTaskService.CreateOrUpdateAsync(10,"saka","descr",Priority.MEDIUM);
    }
}