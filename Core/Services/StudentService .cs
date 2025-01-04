using Microsoft.EntityFrameworkCore;
using School.Core.Interfaces;
using School.Data;
using School.Models;

namespace School.Core.Services;

// Service for managing student-related operations
public class StudentService : IStudentService
{
    private readonly SchoolContext _context;

    // Constructor to initialize the service with DI with the database context
    public StudentService(SchoolContext context)
    {
        _context = context; // The context is injected when the service is created
    }

    // Retrieve all active students, ordered by last name and first name
    public async Task<IEnumerable<Student>> GetAllStudentsAsync()
    {
        return await _context.Students
            .Include(s => s.Fkclass)
            .Where(s => s.IsActive)
            .OrderBy(s => s.LastName) // OrderBy Lastname
            .ThenBy(s => s.FirstName) // Then FirstName
            .ToListAsync(); // Execute the wuery and save the result from DB in a List<Student>
    }

    // Retrieve a student by their unique PIN
    public async Task<Student?> GetStudentByPinAsync(string pin)
    {
        try
        {   // Input cant be null
            if (string.IsNullOrEmpty(pin))
                throw new ArgumentException("Student PIN must not be empty");

            return await _context.Students
                .Include(s => s.Fkclass)
                .FirstOrDefaultAsync(s => s.StudentPin == pin && s.IsActive);
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to fetch student: " + ex.Message);
        }
    }

    // Add a new student to the database
    public async Task<bool> AddStudentAsync(Student student)
    {
        try
        {
            _context.Students.Add(student);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    // Update an existing student's details
    public async Task<bool> UpdateStudentAsync(Student student)
    {
        try
        {
            var existingStudent = await _context.Students
                .FirstOrDefaultAsync(s => s.StudentPin == student.StudentPin);

            if (existingStudent == null)
                return false;

            existingStudent.FirstName = student.FirstName;
            existingStudent.LastName = student.LastName;
            existingStudent.Gender = student.Gender;
            existingStudent.FkclassId = student.FkclassId;

            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    // Soft-delete a student by setting student IsActive to false.
    // In that sense are we keeping the student to the register but that student can't me enrolled to any courses ect. 
    public async Task<bool> RemoveStudentAsync(string pin)
    {
        try
        {
            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.StudentPin == pin);

            if (student == null)
                return false;

            // Soft delete - set IsActive to false instead of removing for saving the student records 
            student.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    // Retrieve all students in a specific class
    public async Task<IEnumerable<Student>> GetStudentsByClassAsync(int classId)
    {
        return await _context.Students
            .Include(s => s.Fkclass)
            .Where(s => s.FkclassId == classId && s.IsActive)
            .OrderBy(s => s.LastName)
            .ThenBy(s => s.FirstName)
            .ToListAsync();
    }

    // Search for students using a keyword in their name or PIN
    public async Task<IEnumerable<Student>> SearchStudentsAsync(string searchTerm)
    {
        var activeStudents = await _context.Students
            .Include(s => s.Fkclass)
            .Where(s => s.IsActive)
            .ToListAsync();

        var matchingStudents = activeStudents.Where(s =>
            s.FirstName.ToLower().Contains(searchTerm) ||
            s.LastName.ToLower().Contains(searchTerm) ||
            s.StudentPin.Contains(searchTerm))
            .OrderBy(s => s.LastName)
            .ToList();

        return matchingStudents;
    }

    // Retrieve all students of a specific gender
    public async Task<IEnumerable<Student>> GetStudentsByGenderAsync(string gender)
    {
        return await _context.Students
            .Include(s => s.Fkclass)
            .Where(s => s.Gender == gender && s.IsActive)
            .OrderBy(s => s.LastName)
            .ThenBy(s => s.FirstName)
            .ToListAsync();
    }

    // Retrieve students enrolled in a specific year
    public async Task<IEnumerable<Student>> GetStudentsByEnrollmentYearAsync(int year)
    {
        return await _context.Students
            .Include(s => s.Fkclass)
            .Where(s => s.EnrollmentDate.Year == year && s.IsActive)
            .OrderBy(s => s.LastName)
            .ThenBy(s => s.FirstName)
            .ToListAsync();
    }

    // Validate if a given PIN follows the correct format and doesn't already exist
    public async Task<bool> ValidateStudentPinAsync(string pin)
    {
        if (string.IsNullOrWhiteSpace(pin) || pin.Length != 13 || pin[8] != '-')
            return false;

        // Check if PIN already exists
        var exists = await _context.Students
            .AnyAsync(s => s.StudentPin == pin);

        if (exists)
            return false;

        // Validate date format (YYYYMMDD)
        if (!DateTime.TryParseExact(pin.Substring(0, 8),
            "yyyyMMdd",
            System.Globalization.CultureInfo.InvariantCulture,
            System.Globalization.DateTimeStyles.None,
            out _))
            return false;

        return true;
    }

    // Retrieve a list of all active classes, ordered by year and section
    public async Task<IEnumerable<Class>> GetClassesAsync()
    {
        return await _context.Classes
            .Where(c => c.IsActive)
            .OrderBy(c => c.Year)
            .ThenBy(c => c.Section)
            .ToListAsync();
    }
}