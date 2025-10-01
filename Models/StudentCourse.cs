using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace studentManagementSystem.Models
{
    public class StudentCourse
    {
        [Key, Column(Order =0)]
        public int StudentId { get; set; }

        [Key, Column(Order =1)]
        public int CourseId {  get; set; }

        [Range(0,100, ErrorMessage ="Marks must be within 100")]
        public int? Mark { get; set; }

        public Student Student { get; set; } = null!;
        public Course Course { get; set; } = null!;
    }
}
