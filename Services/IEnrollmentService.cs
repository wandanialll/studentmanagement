using studentManagementSystem.Models.DTOs;

namespace studentManagementSystem.Services
{
    public interface IEnrollmentService
    {
        Task EnrollAsync(EnrollmentDto dto);
        Task UnenrollAsync(int studentId, int courseId);
        Task UpdateGradeAsync(EnrollmentDto dto);
    }
}
