using AutoMapper;
using studentManagementSystem.Models;
using studentManagementSystem.Models.DTOs;

namespace studentManagementSystem.Services.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Student ↔ StudentDto
            CreateMap<Student, StudentDto>().ReverseMap();

            // Course ↔ CourseDto
            CreateMap<Course, CourseDto>().ReverseMap();

            // StudentCourse ↔ EnrollmentDto
            CreateMap<StudentCourse, EnrollmentDto>().ReverseMap();
        }
    }
}
