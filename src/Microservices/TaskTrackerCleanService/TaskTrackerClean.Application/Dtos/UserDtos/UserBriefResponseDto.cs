using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTrackerClean.Application.Dtos
{
    public class UserBriefResponseDto
    {
        public int Id { get; set; } = 0;
        public string? Username { get; set; }
        public string? Email { get; set; }
    }
}
