using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTrackerClean.Application.Dtos;

namespace TaskTrackerClean.Application.Dtos
{
    public class CreateProjectDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
    }
}
