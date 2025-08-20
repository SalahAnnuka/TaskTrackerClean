using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTrackerClean.Application.Dtos
{
    public class TaskBriefResponseDto
    {
        public int Id { get; set; } = 0;
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string PriorityText { get; set; } = string.Empty;
        public string StatusText { get; set; } = string.Empty;
        public DateTime? StartDate { get; set; }
        public DateTime? DueDate { get; set; }
        public int? Duration { get; set; }
    }
}
