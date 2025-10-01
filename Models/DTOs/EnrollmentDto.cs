using System.ComponentModel.DataAnnotations;

namespace studentManagementSystem.Models.DTOs
{
    public class EnrollmentDto
    {
        [Required]
        public int StudentId { get; set; }

        [Required]
        public int CourseId { get; set; }

        // Nullable grade assigned optionally
        [Range(0, 100)]
        public decimal? Mark { get; set; }

        // idk yet
        public string? CourseTitle { get; set; }
        public string? StudentName { get; set; }
    }
}
