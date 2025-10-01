using AutoMapper;
using studentManagementSystem.Data.Repositories;
using studentManagementSystem.Models;
using studentManagementSystem.Models.DTOs;

namespace studentManagementSystem.Services
{
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _courseRepository;

        public CourseService(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository;
        }

        public async Task<CourseDto> GetByIdAsync(int id)
        {
            var course = await _courseRepository.GetByIdAsync(id);
            return new CourseDto
            {
                CourseId = course.CourseId,
                Title = course.Title,
                Credits = course.Credits
            };
        }

        public async Task<IEnumerable<CourseDto>> GetAllAsync()
        {
            var courses = await _courseRepository.GetAllAsync();
            return courses.Select(c => new CourseDto
            {
                CourseId = c.CourseId,
                Title = c.Title,
                Credits = c.Credits
            });
        }

        public async Task AddAsync(CourseDto courseDto)
        {
            // Business rule: Check for duplicate title
            var existing = await _courseRepository.GetAllAsync();
            if (existing.Any(c => c.Title == courseDto.Title))
                throw new InvalidOperationException("Course title already exists");

            var course = new Course
            {
                Title = courseDto.Title,
                Credits = courseDto.Credits
            };
            await _courseRepository.AddAsync(course);
        }

        public async Task UpdateAsync(CourseDto courseDto)
        {
            // Business rule: Check for duplicate title (excluding current course)
            var existing = await _courseRepository.GetAllAsync();
            if (existing.Any(c => c.Title == courseDto.Title && c.CourseId != courseDto.CourseId))
                throw new InvalidOperationException("Course title already exists");

            var course = await _courseRepository.GetByIdAsync(courseDto.CourseId);
            course.Title = courseDto.Title;
            course.Credits = courseDto.Credits;
            await _courseRepository.UpdateAsync(course);
        }

        public async Task DeleteAsync(int id)
        {
            await _courseRepository.DeleteAsync(id);
        }
    }
}
