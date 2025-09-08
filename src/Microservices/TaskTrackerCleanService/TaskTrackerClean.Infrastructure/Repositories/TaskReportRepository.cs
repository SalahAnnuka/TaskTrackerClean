
using MongoDB.Driver;
using TaskTrackerClean.Domain.Data;
using TaskTrackerClean.Domain.Entities;
using TaskTrackerClean.Domain.Interfaces;
using TaskTrackerClean.Domain.Enums;

namespace TaskTrackerClean.Infrastructure.Repositories
{
    public class TaskReportRepository : ITaskReportRepository
    {
        private readonly MongoDbService _mongoDbService;
        private readonly ITaskRepository _taskRepository;

        public TaskReportRepository(MongoDbService mongoDbService, ITaskRepository taskRepository) {
            _mongoDbService = mongoDbService;
            _taskRepository = taskRepository;
        }


        public async Task<TaskReportEntity> CreateReportAsync()
        {
            var tasks = await _taskRepository.FindAsync();

            var totalTasks = tasks.Count();
            var finishedTasks = tasks.Where(t => t.Status == Status.COMPLETE).Count();

            var report = new TaskReportEntity()
            {
                TotalTasks = totalTasks,
                FinishedTasks = finishedTasks,
                UnfinishedTasks = totalTasks - finishedTasks,
                OverdueTasks = tasks.Where(t => t.DueDate < DateTime.UtcNow
                && t.Status != Status.COMPLETE).Count(),
                FinishedPercentage = (double)finishedTasks / totalTasks,
                Timestamp = DateTime.UtcNow
            };
            await _mongoDbService.TaskReports.InsertOneAsync(report);
            return report;
        }



        public async Task<IEnumerable<TaskReportEntity>> FindReportsAsync(DateTime timestamp)
        {
            try
            {

                DateTime start = timestamp.Date;       
                DateTime end = start.AddDays(1); 

                var filter = Builders<TaskReportEntity>.Filter.And(
                    Builders<TaskReportEntity>.Filter.Gte(doc => doc.Timestamp, start),
                    Builders<TaskReportEntity>.Filter.Lt(doc => doc.Timestamp, end)
                );
                var reports = await _mongoDbService.TaskReports
                    .Find(filter)
                    .ToListAsync();

                return reports;
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving reports.", ex);
            }
        }



        public async Task<TaskReportEntity?> GetLatestReportAsync()
        {
            try
            {

                var newestItem = await _mongoDbService.TaskReports
                    .Find(FilterDefinition<TaskReportEntity>.Empty)
                    .SortByDescending(doc => doc.Timestamp)  
                    .FirstOrDefaultAsync();

                return newestItem;
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving reports.", ex);
            }
        }
    }
}
