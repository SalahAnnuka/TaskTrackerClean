using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTrackerClean.Application.Dtos
{
    public class UserResponseDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public ICollection<TaskBriefResponseDto> Tasks { get; set; } = new List<TaskBriefResponseDto>();
        public int TasksNumber { get; set; }
        public int CompletedTasksNumber { get; set; }
        public int? ProjectId { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
