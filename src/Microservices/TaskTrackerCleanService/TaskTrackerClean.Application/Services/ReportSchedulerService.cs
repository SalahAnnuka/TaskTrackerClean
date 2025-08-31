using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            
        }
    }

}
