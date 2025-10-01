using AutoMapper;
using Microsoft.EntityFrameworkCore;
using studentManagementSystem.Data;
using studentManagementSystem.Models;
using studentManagementSystem.Models.DTOs;

namespace studentManagementSystem.Services
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public EnrollmentService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task EnrollAsync(EnrollmentDto dto)
        {
            // Check student + course exist
            var student = await _context.Students.FindAsync(dto.StudentId);
            var course = await _context.Courses.FindAsync(dto.CourseId);

            if (student == null || course == null)
                throw new KeyNotFoundException("Invalid student or course.");

            // Prevent duplicate enrollment
            var exists = await _context.StudentCourses
                .AnyAsync(sc => sc.StudentId == dto.StudentId && sc.CourseId == dto.CourseId);

            if (exists)
                throw new InvalidOperationException("Student is already enrolled in this course.");

            // Optional business rule: Max 6 courses per student
            var currentEnrollments = await _context.StudentCourses
                .CountAsync(sc => sc.StudentId == dto.StudentId);

            if (currentEnrollments >= 6)
                throw new InvalidOperationException("Student has reached the maximum allowed enrollments (6).");

            var enrollment = _mapper.Map<StudentCourse>(dto);

            _context.StudentCourses.Add(enrollment);
            await _context.SaveChangesAsync();
        }

        public async Task UnenrollAsync(int studentId, int courseId)
        {
            var enrollment = await _context.StudentCourses
                .FirstOrDefaultAsync(sc => sc.StudentId == studentId && sc.CourseId == courseId);

            if (enrollment == null)
                throw new KeyNotFoundException("Enrollment not found.");

            _context.StudentCourses.Remove(enrollment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateGradeAsync(EnrollmentDto dto)
        {
            var enrollment = await _context.StudentCourses
                .FirstOrDefaultAsync(sc => sc.StudentId == dto.StudentId && sc.CourseId == dto.CourseId);

            if (enrollment == null)
                throw new KeyNotFoundException("Enrollment not found.");

            if (dto.Mark is < 0 or > 100)
                throw new InvalidOperationException("Grade must be between 0 and 100.");

            enrollment.Mark = (int?)dto.Mark;
            await _context.SaveChangesAsync();
        }
    }
}
