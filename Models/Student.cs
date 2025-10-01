using System.ComponentModel.DataAnnotations;

namespace studentManagementSystem.Models
{
    public class Student
    {
        [Key]
        public int StudentId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = null!;

        //public ICollection<StudentCourse> StudentCourses { get; set; } = new List<StudentCourse>();
        public List<StudentCourse> StudentCourses { get; set; } = new List<StudentCourse>();
    }
}
