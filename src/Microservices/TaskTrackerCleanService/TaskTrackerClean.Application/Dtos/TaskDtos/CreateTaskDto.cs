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
