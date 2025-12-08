using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Dto
{
    public class EditTaskDto
    {
        public string? Name { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
    }
}
