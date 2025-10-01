using System.ComponentModel.DataAnnotations;

namespace studentManagementSystem.Models.DTOs
{
    public class CourseDto
    {
        public int CourseId { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [Range(0, 4)]
        public int Credits { get; set; }
    }
}
