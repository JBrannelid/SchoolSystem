using Microsoft.EntityFrameworkCore;
using School.Core.Interfaces;
using School.Data;
using School.Models;

namespace School.Core.Services;

// Service for managing Course-related operations
public class CourseService : ICourseService
{
    private readonly SchoolContext _context;

    // Constructor to initialize the service with DI with the database context
    public CourseService(SchoolContext context)
    {
        _context = context; // The context is injected when the service is created
    }

    // Get all courses from the database
    public async Task<IEnumerable<Course>> GetAllCoursesAsync()
    {
        return await _context.Courses
            .Include(c => c.CourseEnrollments)
            .OrderBy(c => c.CourseCode)
            .ToListAsync();  // Asynchronously retrieve and return the list of courses
    }

    // Get only active courses from the database
    public async Task<IEnumerable<Course>> GetActiveCourses()
    {
        return await _context.Courses
            .Include(c => c.CourseEnrollments)
            .Where(c => c.IsActive)  // Only fetch active courses (IsActive == true)
            .OrderBy(c => c.CourseCode)  // Order courses by course code
            .ToListAsync();
    }

    // Get a specific course by its ID
    public async Task<Course?> GetCourseByIdAsync(int id)
    {
        return await _context.Courses
            .Include(c => c.CourseEnrollments)
            .FirstOrDefaultAsync(c => c.CourseId == id);  // Find the first uniq courseID 
    }

    // Method to add a Course with a bool value to return true and save new course or false if anything went wrong or course code already excist 
    public async Task<bool> AddCourseAsync(Course course)
    {  
        try
        {
            // Check if course code already exsist 
            bool exists = await _context.Courses
                .AnyAsync(c => c.CourseCode == course.CourseCode);
            
            // Return false if course code exsist 
            if (exists)
                return false;

            // Return true if course code dosn't exsist and save the changes to the db
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false; // Return false if an error occurs
        }
    }

    // Update an existing course
    public async Task<bool> UpdateCourseAsync(Course course)
    {
        try
        {
            // Retrives the existing course from the database
            var existingCourse = await _context.Courses
                .FirstOrDefaultAsync(c => c.CourseId == course.CourseId);

            if (existingCourse == null) // Return null if the course dosen't excist
                return false;

            // If course code is being changed by the user, check if new course code already exists
            if (existingCourse.CourseCode != course.CourseCode)
            {
                // If the code is changing, check if the new code already exists
                var codeExists = await _context.Courses
                    .AnyAsync(c => c.CourseCode == course.CourseCode);

                if (codeExists) // If the new code exists, return false
                    return false;
            }

            // Update the course details
            existingCourse.CourseName = course.CourseName;
            existingCourse.CourseCode = course.CourseCode;
            existingCourse.IsActive = course.IsActive;

            await _context.SaveChangesAsync();  // Save the updated course to the database
            return true;  // Return true if the course was successfully updated
        }
        catch
        {
            return false;
        }
    }

    // Remove a course from the database (soft delete). Just set the CourseId to inactiv
    public async Task<bool> RemoveCourseAsync(int courseId)
    {
        try
        {
            var course = await _context.Courses
                .FirstOrDefaultAsync(c => c.CourseId == courseId);

            if (course == null)
                return false;

            // Soft delete: Mark the course as inactive by setting IsActive to false
            course.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    // Get grades for a specific student by their uniq PIN
    public async Task<IEnumerable<VwStudentCourseGrade>> GetStudentGradesAsync(string studentPin)
    {
        return await _context.VwStudentCourseGrades
            .Where(g => g.StudentPin == studentPin)
            .OrderBy(g => g.CourseName)
            .ToListAsync();
    }
}