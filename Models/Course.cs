using System.ComponentModel.DataAnnotations;

namespace studentManagementSystem.Models
{
    public class Course
    {
        [Key]
        public int CourseId { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = null!;

        [Range(0, 4)]
        public int Credits { get; set; }

        //public ICollection<StudentCourse> StudentCourses { get; set; } = new List<StudentCourse>();
        public List<StudentCourse> StudentCourses { get; set; } = new List<StudentCourse>();
    }
}
