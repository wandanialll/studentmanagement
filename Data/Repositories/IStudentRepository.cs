using studentManagementSystem.Models;

namespace studentManagementSystem.Data.Repositories
{
    public interface IStudentRepository
    {
        Task<Student> GetByIdAsync(int id);
        Task<IEnumerable<Student>> GetAllAsync();
        Task AddAsync(Student student);
        Task UpdateAsync(Student student);
        Task DeleteAsync(int id);
        Task<StudentCourse> GetEnrollmentByIdAsync(int studentId, int courseId);
        Task<IEnumerable<StudentCourse>> GetEnrollmentsByStudentIdAsync(int studentId);
        Task<IEnumerable<StudentCourse>> GetEnrollmentsByCourseIdAsync(int courseId);
        Task DeleteEnrollmentAsync(int studentId, int courseId);
        Task UpdateEnrollmentAsync(StudentCourse enrollment);
    }
}
