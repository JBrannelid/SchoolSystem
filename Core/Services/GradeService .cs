using Microsoft.EntityFrameworkCore;
using School.Core.Interfaces;
using School.Data;
using School.Models;

namespace School.Core.Services;

// Service for managing grade-related operations
public class GradeService : IGradeService
{
    private readonly SchoolContext _context;

    // Constructor to initialize the service with DI with the database context
    public GradeService(SchoolContext context)
    {
        _context = context; // The context is injected when the service is created
    }

    public async Task<bool> AssignGradeAsync(string studentPin, int courseId, string teacherPin, string grade)
    {
        // Using var transaction to handle DB transaction in a safe way
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // Check if the grade exists in the system (A, B, C, D, E, F)
            var validGrade = await _context.GradeValues
                .FirstOrDefaultAsync(g => g.Grade.Trim() == grade.Trim());

            if (validGrade == null)
            {
                // If the grade is invalid, throw an exception
                throw new ArgumentException("Invalid grade");
            }

            // Look for an existing course enrollment for a student
            var enrollment = await _context.CourseEnrollments
                .FirstOrDefaultAsync(ce =>
                    ce.FkstudentPin == studentPin &&
                    ce.FkcourseId == courseId &&
                    ce.IsActive);

            // If no enrollment exists, create a new one with today's date
            if (enrollment == null)
            {
                enrollment = new CourseEnrollment
                {
                    FkstudentPin = studentPin,
                    FkcourseId = courseId,
                    FkteacherPin = teacherPin,
                    EnrollmentDate = DateOnly.FromDateTime(DateTime.Now),
                    IsActive = true
                };
                _context.CourseEnrollments.Add(enrollment);
            }

            // Update the enrollment with the new grade and date
            enrollment.Grade = grade;
            enrollment.GradeAssignedDate = DateTime.Now;

            // Save changes and commit the transaction
            await _context.SaveChangesAsync();
            await transaction.CommitAsync(); // Ensure the transaction is released
            return true;
        }
        catch
        {
            // If something goes wrong, roll back all changes and return a false value
            await transaction.RollbackAsync();
            return false;
        }
    }

    // Retrieve all possible grade values, sorted from highest to lowest (Order by descending)
    public async Task<IEnumerable<GradeValue>> GetAllGradeValuesAsync()
    {
        return await _context.GradeValues
            .OrderByDescending(g => g.GradeValue1) 
            .ToListAsync();
    }

    // Retrieve all grades for a specific student
    public async Task<IEnumerable<VwStudentCourseGrade>> GetStudentGradesAsync(string studentPin)
    {
        return await _context.VwStudentCourseGrades
            .Where(g => g.StudentPin == studentPin) // Filters by student PIN
            .ToListAsync();
    }

    // Retrieve detailed grade information for a student, sorted by course name
    public async Task<IEnumerable<VwStudentCourseGrade>> GetDetailedStudentInfoAsync(string studentPin)
    {
        return await _context.VwStudentCourseGrades
            .Where(g => g.StudentPin == studentPin)
            .OrderBy(g => g.CourseName) // Sorts by course name
            .ToListAsync();
    }
}
