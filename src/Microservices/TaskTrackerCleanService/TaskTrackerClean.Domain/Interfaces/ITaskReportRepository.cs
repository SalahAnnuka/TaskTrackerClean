using TaskTrackerClean.Domain.Entities;

namespace TaskTrackerClean.Domain.Interfaces
{
    public interface ITaskReportRepository
    {
        public Task<IEnumerable<TaskReportEntity>> FindReportsAsync(DateTime timestamp);
        public Task<TaskReportEntity> CreateReportAsync();
        public Task<TaskReportEntity?> GetLatestReportAsync();
    }
}
