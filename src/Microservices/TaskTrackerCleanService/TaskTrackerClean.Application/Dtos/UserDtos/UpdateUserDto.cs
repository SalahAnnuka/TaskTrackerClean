namespace TaskTrackerClean.Application.Dtos
{
    public class UpdateUserDto
    {
        public int Id { get; set; } = 0;
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}
