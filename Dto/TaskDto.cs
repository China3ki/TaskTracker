using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Dto
{
    public sealed class TaskDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [MinLength(1)]
        public string? Description { get; set; } = string.Empty;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? StatusId { get; set; }
    }
}
