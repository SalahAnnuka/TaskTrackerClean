using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTrackerClean.Domain.Enums;

namespace TaskTrackerClean.Application.Dtos
{
    public class CreateTaskDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? PriorityText { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? DueDate { get; set; }
    }
}
