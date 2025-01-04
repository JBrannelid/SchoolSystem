using School.Models;
using System.Threading.Tasks;

namespace School.Core.Interfaces;

// Interface for course-related operations.

public interface ICourseService
{
    Task<IEnumerable<Course>> GetAllCoursesAsync(); // Returns an async task all courses

    Task<IEnumerable<Course>> GetActiveCourses(); // Returns an async task with active courses

    Task<Course?> GetCourseByIdAsync(int id); // Take an Id and returns the course value if found, or null if not found

    Task<bool> AddCourseAsync(Course course); // Return a bool value 

    Task<bool> UpdateCourseAsync(Course course); // Return a bool value 

    Task<bool> RemoveCourseAsync(int courseId); // Return a bool value 

    Task<IEnumerable<VwStudentCourseGrade>> GetStudentGradesAsync(string studentPin); // Returns an async task with grade data (VwStudentCourseGrade) from a student PIN

}