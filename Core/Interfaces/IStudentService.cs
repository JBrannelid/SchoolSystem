using School.Models;
using System.Net.NetworkInformation;

namespace School.Core.Interfaces;

// Interface for student-related operations
public interface IStudentService
{
    Task<IEnumerable<Student>> GetAllStudentsAsync();  // Gets a list of all students

    Task<Student?> GetStudentByPinAsync(string pin); // Gets a student by their unique PIN. Returns null if not found

    Task<bool> AddStudentAsync(Student student); // Adds a new student. Returns true or false 

    Task<bool> UpdateStudentAsync(Student student); // Updates an existing student's information. Returns true or false

    Task<bool> RemoveStudentAsync(string pin); // Removes a student by their PIN. Returns true or false 

    Task<IEnumerable<Student>> GetStudentsByClassAsync(int classId); // Gets a list of students in a specific class

    Task<IEnumerable<Student>> SearchStudentsAsync(string searchTerm); // Searches for students by a search term (name, PIN, etc.)

    Task<IEnumerable<Student>> GetStudentsByGenderAsync(string gender); // Gets a list of students by gender

    Task<IEnumerable<Student>> GetStudentsByEnrollmentYearAsync(int year); // Gets a list of students enrolled in a specific year

    Task<bool> ValidateStudentPinAsync(string pin); // Validates if a student PIN exists in the system

    Task<IEnumerable<Class>> GetClassesAsync(); // Gets a list of all classes
}