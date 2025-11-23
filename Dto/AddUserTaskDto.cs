using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Dto
{
    public class AddUserTaskDto
    {
        [Required]
        public int TaskId { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public bool Admin { get; set; }
    }
}
