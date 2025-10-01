using studentManagementSystem.Models;
using studentManagementSystem.Models.DTOs;

namespace studentManagementSystem.Services
{
    public interface IStudentService
    {
        Task<StudentDto> GetByIdAsync(int id);
        Task<IEnumerable<StudentDto>> GetAllAsync();
        Task<IEnumerable<Student>> GetAllWithEnrollmentsAsync();
        Task AddAsync(StudentDto studentDto);
        Task UpdateAsync(StudentDto studentDto);
        Task DeleteAsync(int id);
        Task EnrollAsync(EnrollmentDto enrollmentDto);
        Task<EnrollmentDto> GetEnrollmentByIdAsync(int studentId, int courseId);
        Task<IEnumerable<EnrollmentDto>> GetEnrollmentsByStudentIdAsync(int studentId);
        Task DeleteEnrollmentAsync(int studentId, int courseId);
        Task EditEnrollmentAsync(EnrollmentDto enrollmentDto);
    }
}
