using AutoMapper;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Logging;
using studentManagementSystem.Data.Repositories;
using studentManagementSystem.Models;
using studentManagementSystem.Models.DTOs;
using static studentManagementSystem.DebugConstants;

namespace studentManagementSystem.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly ILogger<StudentService> _logger;


        public StudentService(IStudentRepository studentRepository, ICourseRepository courseRepository, ILogger<StudentService> logger)
        {
            _studentRepository = studentRepository;
            _courseRepository = courseRepository;
            _logger = logger;
        }

        public async Task<StudentDto> GetByIdAsync(int id)
        {
            var student = await _studentRepository.GetByIdAsync(id);
            return new StudentDto
            {
                StudentId = student.StudentId,
                Name = student.Name,
                Email = student.Email
            };
        }

        public async Task<IEnumerable<StudentDto>> GetAllAsync()
        {
            var students = await _studentRepository.GetAllAsync();
            return students.Select(s => new StudentDto
            {
                StudentId = s.StudentId,
                Name = s.Name,
                Email = s.Email
            });
        }

        public async Task<IEnumerable<Student>> GetAllWithEnrollmentsAsync()
        {
            _logger.LogInformation("Fetching all students with enrollments");
            return await _studentRepository.GetAllAsync();
        }

        public async Task<EnrollmentDto> GetEnrollmentByIdAsync(int studentId, int courseId)
        {
            _logger.LogInformation("Fetching enrollment for StudentId {StudentId}, CourseId {CourseId}", studentId, courseId);
            var enrollment = await _studentRepository.GetEnrollmentByIdAsync(studentId, courseId);
            if (enrollment == null)
            {
                _logger.LogWarning("Enrollment not found for StudentId {StudentId}, CourseId {CourseId}", studentId, courseId);
                throw new InvalidOperationException(DebugConstants.EnrollmentNotFound);
            }

            var student = await _studentRepository.GetByIdAsync(studentId);
            var course = await _courseRepository.GetByIdAsync(courseId);

            return new EnrollmentDto
            {
                StudentId = enrollment.StudentId,
                CourseId = enrollment.CourseId,
                Mark = enrollment.Mark,
                StudentName = student.Name,
                CourseTitle = course.Title
            };
        }

        public async Task<IEnumerable<EnrollmentDto>> GetEnrollmentsByStudentIdAsync(int studentId)
        {
            _logger.LogInformation("Fetching enrollments for StudentId {StudentId}", studentId);
            if (studentId <= 0)
            {
                _logger.LogWarning("Invalid StudentId: {StudentId}", studentId);
                throw new ArgumentException("Please provide a valid student ID.");
            }

            var student = await _studentRepository.GetByIdAsync(studentId); // Ensure student exists
            var enrollments = await _studentRepository.GetEnrollmentsByStudentIdAsync(studentId);
            var enrollmentDtos = enrollments.Select(e => new EnrollmentDto
            {
                StudentId = e.StudentId,
                CourseId = e.CourseId,
                Mark = e.Mark,
                StudentName = e.Student?.Name,
                CourseTitle = e.Course?.Title
            }).ToList();

            _logger.LogInformation("Found {Count} enrollments for StudentId {StudentId}", enrollmentDtos.Count, studentId);
            return enrollmentDtos;
        }

        public async Task AddAsync(StudentDto studentDto)
        {
            // Business rule: Check for duplicate email
            var existing = await _studentRepository.GetAllAsync();
            if (existing.Any(s => s.Email == studentDto.Email))
                throw new InvalidOperationException("Email already exists");

            var student = new Student
            {
                Name = studentDto.Name,
                Email = studentDto.Email
            };
            await _studentRepository.AddAsync(student);
        }

        public async Task UpdateAsync(StudentDto studentDto)
        {
            // Business rule: Check for duplicate email (excluding current student)
            var existing = await _studentRepository.GetAllAsync();
            if (existing.Any(s => s.Email == studentDto.Email && s.StudentId != studentDto.StudentId))
                throw new InvalidOperationException("Email already exists");

            var student = await _studentRepository.GetByIdAsync(studentDto.StudentId);
            student.Name = studentDto.Name;
            student.Email = studentDto.Email;
            await _studentRepository.UpdateAsync(student);
        }

        public async Task DeleteAsync(int id)
        {
            await _studentRepository.DeleteAsync(id);
        }

        public async Task EnrollAsync(EnrollmentDto enrollmentDto)
        {
            _logger.LogInformation("Attempting to enroll student {StudentId} in course {CourseId}", enrollmentDto.StudentId, enrollmentDto.CourseId);

            // Validate StudentId
            if (enrollmentDto.StudentId <= 0)
            {
                _logger.LogWarning("Invalid StudentId: {StudentId}", enrollmentDto.StudentId);
                throw new ArgumentException("Invalid student ID.");
            }

            // Validate CourseId
            if (enrollmentDto.CourseId <= 0)
            {
                _logger.LogWarning("Invalid CourseId: {CourseId}", enrollmentDto.CourseId);
                throw new ArgumentException("Invalid course ID.");
            }

            Student student;
            try
            {
                student = await _studentRepository.GetByIdAsync(enrollmentDto.StudentId);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError("Student not found: {StudentId}", enrollmentDto.StudentId);
                throw new InvalidOperationException(DebugConstants.StudentNotFound, ex);
            }

            if (student.StudentCourses.Any(sc => sc.CourseId == enrollmentDto.CourseId))
            {
                _logger.LogWarning("Student {StudentId} already enrolled in course {CourseId}", enrollmentDto.StudentId, enrollmentDto.CourseId);
                throw new InvalidOperationException(DebugConstants.AlreadyEnrolled);
            }

            if (student.StudentCourses.Count >= 5)
            {
                _logger.LogWarning("Student {StudentId} cannot enroll in more than 5 courses", enrollmentDto.StudentId);
                throw new InvalidOperationException(DebugConstants.MaxEnrollments);
            }

            Course course;
            try
            {
                course = await _courseRepository.GetByIdAsync(enrollmentDto.CourseId);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError("Course not found: {CourseId}", enrollmentDto.CourseId);
                throw new InvalidOperationException(DebugConstants.CourseNotFound, ex);
            }

            var enrollment = new StudentCourse
            {
                StudentId = enrollmentDto.StudentId,
                CourseId = enrollmentDto.CourseId,
                Mark = (int?)enrollmentDto.Mark
            };
            student.StudentCourses.Add(enrollment);
            await _studentRepository.UpdateAsync(student);
            _logger.LogInformation("Enrollment successful for student {StudentId} in course {CourseId}", enrollmentDto.StudentId, enrollmentDto.CourseId);
        }

        public async Task DeleteEnrollmentAsync(int studentId, int courseId)
        {
            _logger.LogInformation("Attempting to delete enrollment for StudentId {StudentId}, CourseId {CourseId}", studentId, courseId);

            if (studentId <= 0)
            {
                _logger.LogWarning("Invalid StudentId: {StudentId}", studentId);
                throw new ArgumentException("Please provide a valid student ID.");
            }

            if (courseId <= 0)
            {
                _logger.LogWarning("Invalid CourseId: {CourseId}", courseId);
                throw new ArgumentException("Please provide a valid course ID.");
            }

            var enrollment = await _studentRepository.GetEnrollmentByIdAsync(studentId, courseId);
            if (enrollment == null)
            {
                _logger.LogWarning("Enrollment not found for StudentId {StudentId}, CourseId {CourseId}", studentId, courseId);
                throw new InvalidOperationException(DebugConstants.EnrollmentNotFound);
            }

            await _studentRepository.DeleteEnrollmentAsync(studentId, courseId);
            _logger.LogInformation("Enrollment deleted successfully for StudentId {StudentId}, CourseId {CourseId}", studentId, courseId);
        }

        public async Task EditEnrollmentAsync(EnrollmentDto enrollmentDto)
        {
            _logger.LogInformation("Attempting to edit enrollment for StudentId {StudentId}, CourseId {CourseId}", enrollmentDto.StudentId, enrollmentDto.CourseId);

            if (enrollmentDto.StudentId <= 0)
            {
                _logger.LogWarning("Invalid StudentId: {StudentId}", enrollmentDto.StudentId);
                throw new ArgumentException("Please provide a valid student ID.");
            }

            if (enrollmentDto.CourseId <= 0)
            {
                _logger.LogWarning("Invalid CourseId: {CourseId}", enrollmentDto.CourseId);
                throw new ArgumentException("Please provide a valid course ID.");
            }

            var enrollment = await _studentRepository.GetEnrollmentByIdAsync(enrollmentDto.StudentId, enrollmentDto.CourseId);
            if (enrollment == null)
            {
                _logger.LogWarning("Enrollment not found for StudentId {StudentId}, CourseId {CourseId}", enrollmentDto.StudentId, enrollmentDto.CourseId);
                throw new InvalidOperationException(DebugConstants.EnrollmentNotFound);
            }

            enrollment.Mark = (int?)enrollmentDto.Mark;
            await _studentRepository.UpdateEnrollmentAsync(enrollment);
            _logger.LogInformation("Enrollment updated successfully for StudentId {StudentId}, CourseId {CourseId}", enrollmentDto.StudentId, enrollmentDto.CourseId);
        }
    }
}
