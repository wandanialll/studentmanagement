using studentManagementSystem.Models.DTOs;

namespace studentManagementSystem.Services
{
    public interface ICourseService
    {
        Task<CourseDto> GetByIdAsync(int id);
        Task<IEnumerable<CourseDto>> GetAllAsync();
        Task AddAsync(CourseDto courseDto);
        Task UpdateAsync(CourseDto courseDto);
        Task DeleteAsync(int id);
    }
}
