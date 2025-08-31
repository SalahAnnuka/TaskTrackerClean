using Hangfire;

namespace TaskTrackerClean.Application.Services
{
    public class ReportSchedulerService
    {
        private readonly TaskReportService _taskReportService;

        public ReportSchedulerService(TaskReportService taskReportService)
        {
            _taskReportService = taskReportService;
        }

        public void ConfigureDailyReportJob()
        {
            RecurringJob.AddOrUpdate(
                recurringJobId: "DailyReportJob",
                methodCall: () => _taskReportService.GenerateDailyReportAsync(),
                cronExpression: "0 0 * * *",
                options: new RecurringJobOptions
                {
                    TimeZone = TimeZoneInfo.Utc,
                }
            );
        }
    }

}
