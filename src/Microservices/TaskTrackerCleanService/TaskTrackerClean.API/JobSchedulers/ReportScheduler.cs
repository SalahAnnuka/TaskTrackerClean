using Hangfire;
using TaskTrackerClean.Application.Interfaces;

namespace TaskTrackerClean.API.JobSchedulers
{
    public class ReportScheduler
    {
        private readonly ITaskReportService _taskReportService;
        private readonly IRecurringJobManager _recurringJobManager;

        public ReportScheduler(
            ITaskReportService taskReportService,
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
                cronExpression: Cron.Yearly,
                options: new RecurringJobOptions
                {
                    TimeZone = TimeZoneInfo.Utc,
                }
            );
        }
    }
}