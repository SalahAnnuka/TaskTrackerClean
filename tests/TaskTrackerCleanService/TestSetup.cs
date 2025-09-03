using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.Extensions.DependencyInjection;
using TaskTrackerClean.Application.Interfaces;
using TaskTrackerClean.Application.Services;
using TaskTrackerClean.Domain.Interfaces;
using TaskTrackerClean.Infrastructure.Repositories;

[SetUpFixture]
public class TestSetup
{
    public static ServiceProvider ServiceProvider { get; private set; }

    [OneTimeSetUp]
    public void GlobalSetup()
    {
        var services = new ServiceCollection();

        services.AddTransient<ITaskRepository, TaskRepository>();
        services.AddTransient<IUserRepository, UserRepository>();
        services.AddTransient<IProjectRepository, ProjectRepository>();
        services.AddTransient<ITaskReportRepository, TaskReportRepository>();

        services.AddTransient<ITaskService, TaskService>();
        services.AddTransient<IUserService, UserService>();
        services.AddTransient<IProjectService, ProjectService>();
        services.AddTransient<TaskReportService>();
        services.AddTransient<ReportSchedulerService>();
        //services.AddTransient<TaskTrackerClean.Domain.Data.MongoDbService>();

        ServiceProvider = services.BuildServiceProvider();
    }

    [OneTimeTearDown]
    public void GlobalTeardown()
    {
        ServiceProvider?.Dispose();
    }

}