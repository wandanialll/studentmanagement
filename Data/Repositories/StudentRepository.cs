
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using studentManagementSystem.Data;
using studentManagementSystem.Data.Repositories;
using studentManagementSystem.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace studentManagementSystem.Data.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<StudentRepository> _logger;

        public StudentRepository(ApplicationDbContext context, ILogger<StudentRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Student> GetByIdAsync(int id)
        {
            _logger.LogInformation("Querying student with ID {Id}", id);
            var student = await _context.Students
                .Include(s => s.StudentCourses)
                .ThenInclude(sc => sc.Course)
                .FirstOrDefaultAsync(s => s.StudentId == id);

            if (student == null)
            {
                _logger.LogWarning("Student not found for ID {Id}", id);
                throw new KeyNotFoundException("Student not found");
            }

            _logger.LogInformation("Found student: {Name}, ID {Id}", student.Name, student.StudentId);
            return student;
        }

        public async Task<IEnumerable<Student>> GetAllAsync()
        {
            _logger.LogInformation("Fetching all students");
            return await _context.Students
                .Include(s => s.StudentCourses)
                .ThenInclude(sc => sc.Course)
                .ToListAsync();
        }

        public async Task AddAsync(Student student)
        {
            _logger.LogInformation("Adding student: {Name}, {Email}", student.Name, student.Email);
            await _context.Students.AddAsync(student);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Student added: ID {Id}", student.StudentId);
        }

        public async Task UpdateAsync(Student student)
        {
            _logger.LogInformation("Updating student: ID {Id}", student.StudentId);
            _context.Students.Update(student);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Student updated: ID {Id}", student.StudentId);
        }

        public async Task DeleteAsync(int id)
        {
            _logger.LogInformation("Deleting student with ID {Id}", id);
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                _logger.LogWarning("Student not found for deletion: ID {Id}", id);
                throw new KeyNotFoundException("Student not found");
            }

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Student deleted: ID {Id}", id);
        }

        public async Task<StudentCourse> GetEnrollmentByIdAsync(int studentId, int courseId)
        {
            _logger.LogInformation("Querying enrollment for StudentId {StudentId}, CourseId {CourseId}", studentId, courseId);
            var enrollment = await _context.StudentCourses
                .Include(sc => sc.Student)
                .Include(sc => sc.Course)
                .FirstOrDefaultAsync(sc => sc.StudentId == studentId && sc.CourseId == courseId);

            if (enrollment == null)
            {
                _logger.LogWarning("Enrollment not found for StudentId {StudentId}, CourseId {CourseId}", studentId, courseId);
            }

            return enrollment;
        }

        public async Task<IEnumerable<StudentCourse>> GetEnrollmentsByStudentIdAsync(int studentId)
        {
            _logger.LogInformation("Querying enrollments for StudentId {StudentId}", studentId);
            var enrollments = await _context.StudentCourses
                .Include(sc => sc.Student)
                .Include(sc => sc.Course)
                .Where(sc => sc.StudentId == studentId)
                .ToListAsync();

            _logger.LogInformation("Found {Count} enrollments for StudentId {StudentId}", enrollments.Count, studentId);
            return enrollments;
        }

        public async Task<IEnumerable<StudentCourse>> GetEnrollmentsByCourseIdAsync(int courseId)
        {
            _logger.LogInformation("Querying enrollments for CourseId {CourseId}", courseId);
            var enrollments = await _context.StudentCourses
                .Include(sc => sc.Student)
                .Include(sc => sc.Course)
                .Where(sc => sc.CourseId == courseId)
                .ToListAsync();

            _logger.LogInformation("Found {Count} enrollments for CourseId {CourseId}", enrollments.Count, courseId);
            return enrollments;
        }

        public async Task DeleteEnrollmentAsync(int studentId, int courseId)
        {
            _logger.LogInformation("Deleting enrollment for StudentId {StudentId}, CourseId {CourseId}", studentId, courseId);
            var enrollment = await _context.StudentCourses
                .FirstOrDefaultAsync(sc => sc.StudentId == studentId && sc.CourseId == courseId);

            if (enrollment == null)
            {
                _logger.LogWarning("Enrollment not found for deletion: StudentId {StudentId}, CourseId {CourseId}", studentId, courseId);
                throw new KeyNotFoundException("Enrollment not found");
            }

            _context.StudentCourses.Remove(enrollment);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Enrollment deleted: StudentId {StudentId}, CourseId {CourseId}", studentId, courseId);
        }

        public async Task UpdateEnrollmentAsync(StudentCourse enrollment)
        {
            _logger.LogInformation("Updating enrollment for StudentId {StudentId}, CourseId {CourseId}", enrollment.StudentId, enrollment.CourseId);
            _context.StudentCourses.Update(enrollment);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Enrollment updated: StudentId {StudentId}, CourseId {CourseId}", enrollment.StudentId, enrollment.CourseId);
        }
    }

}