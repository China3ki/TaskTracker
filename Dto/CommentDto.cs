using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Dto
{
    public class CommentDto
    {
        [Required, MinLength(1)]
        public string Text { get; set; } = string.Empty;
        [Required]
        public int TaskId { get; set; }
    }
}
