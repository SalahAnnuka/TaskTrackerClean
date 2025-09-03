using TaskTrackerClean.Domain.Entities;

namespace TaskTrackerClean.Application.Interfaces
{
    public interface ITaskReportService
    {
        public Task<TaskReportEntity> GenerateDailyReportAsync();
        public Task<TaskReportEntity?> GetLatestReportAsync();
    }
}
