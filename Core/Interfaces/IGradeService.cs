using Microsoft.VisualBasic;
using School.Core.Services;
using School.Models;

namespace School.Core.Interfaces;

// Interface for grade-related operations
public interface IGradeService
{
    Task<bool> AssignGradeAsync(string studentPin, int courseId, string teacherPin, string grade); // Assigns a grade to a student (return a bool)

    Task<IEnumerable<GradeValue>> GetAllGradeValuesAsync(); // Gets a list of all grade values (A, B, C, etc.)

    Task<IEnumerable<VwStudentCourseGrade>> GetStudentGradesAsync(string studentPin); // Gets all grades assigned to a specific student

    Task<IEnumerable<VwStudentCourseGrade>> GetDetailedStudentInfoAsync(string studentPin); // Retrieves detailed information about a student's grades and teachers
}