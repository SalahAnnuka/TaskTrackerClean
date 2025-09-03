using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTrackerClean.Application.Interfaces;
using TaskTrackerClean.Domain.Entities;
using TaskTrackerClean.Domain.Interfaces;

namespace TaskTrackerClean.Application.Services
{
    public class TaskReportService : ITaskReportService
    {
        private readonly ITaskReportRepository _taskReportRepository;

        public TaskReportService(ITaskReportRepository taskReportRepository)
        {
            _taskReportRepository = taskReportRepository;
        }

        public async Task<TaskReportEntity> GenerateDailyReportAsync()
        {
            var report = await _taskReportRepository.CreateReportAsync();
            return report;
        }
        public async Task<TaskReportEntity?> GetLatestReportAsync() {
            return await _taskReportRepository.GetLatestReportAsync();
        }

    }
}
