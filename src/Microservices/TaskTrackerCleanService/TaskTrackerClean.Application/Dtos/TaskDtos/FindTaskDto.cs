using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTrackerClean.Domain.Enums;

namespace TaskTrackerClean.Application.Dtos
{
    public class FindTaskDto
    {
        public int Id { get; set; } = 0;
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? PriorityText { get; set; }
        public string? StatusText { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? DueDate { get; set; }
    }
}
