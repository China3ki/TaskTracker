using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Dto
{
    public class TaskUserDto
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public int TaskId { get; set; }
        public bool Admin { get; set; } = false;
    }
}
