using Hangfire;

namespace TaskTrackerClean.Application.Services
{
    public class ReportSchedulerService
    {
        private readonly TaskReportService _taskReportService;
        private readonly IRecurringJobManager _recurringJobManager;

        public ReportSchedulerService(
            TaskReportService taskReportService,
            IRecurringJobManager recurringJobManager)
        {
            _taskReportService = taskReportService;
            _recurringJobManager = recurringJobManager;
        }

        public void ConfigureDailyReportJob()
        {
            _recurringJobManager.AddOrUpdate(
                recurringJobId: "DailyReportJob",
                methodCall: () => _taskReportService.GenerateDailyReportAsync(),
                cronExpression: "* * * * *",
                options: new RecurringJobOptions
                {
                    TimeZone = TimeZoneInfo.Utc,
                }
            );
        }
    }
}