using Microsoft.EntityFrameworkCore;
using School.Core.Interfaces;
using School.Data;
using School.Models;

namespace School.Core.Services;

// Service for managing Employee-related operations
public class EmployeeService : IEmployeeService
{
    private readonly SchoolContext _context;

    // Constructor to initialize the service with DI with the database context
    public EmployeeService(SchoolContext context)
    {
        _context = context; // The context is injected when the service is created
    }

    // Retrieve all active employees with their associated department
    public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
    {
        return await _context.Employees
            .Include(e => e.Fkdepartment)
            .Where(e => e.IsActive) // Only fetch active employees
            .ToListAsync(); // Convert the query from DB to a list
    }

    // Retrieve employees based on a specific department ID
    public async Task<IEnumerable<Employee>> GetEmployeesByDepartmentAsync(int departmentId)
    {
        return await _context.Employees
            .Include(e => e.Fkdepartment)
            .Where(e => e.FkdepartmentId == departmentId && e.IsActive)
            .ToListAsync();
    }

    // Retrieve employees based on their position
    public async Task<IEnumerable<Employee>> GetEmployeesByPositionAsync(string position)
    {
        return await _context.Employees
            .Include(e => e.Fkdepartment)
            .Where(e => e.Position == position && e.IsActive)
            .ToListAsync();
    }

    // Retrieve a single employee using their unique PIN
    public async Task<Employee?> GetEmployeeByPinAsync(string pin)
    {
        return await _context.Employees
            .Include(e => e.Fkdepartment)
            .FirstOrDefaultAsync(e => e.EmployeePin == pin && e.IsActive); // Find the first matching employee (every pin is uniq)
    }

    // Add a new employee to the database
    public async Task<bool> AddEmployeeAsync(Employee employee)
    {
        try
        {
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync(); // Save changes to the database
            return true;
        }
        catch
        {
            return false; // Return failure if an exception occurs
        }
    }

    public async Task<IEnumerable<Department>> GetDepartments()
    {
        return await _context.Departments
            .OrderBy(d => d.Name) // Order by name ASC
            .ToListAsync();  // Convert the query to a list
    }

    // Soft-delete an employee by setting IsActive to false. In that way could we save a employee records in the DB system
    public async Task<bool> RemoveEmployeeAsync(string employeePin)
    {
        try
        {
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.EmployeePin == employeePin);

            // Return false if no employee is found
            if (employee == null)
                return false;

            // Soft delete - set IsActive to false instead of removing
            employee.IsActive = false; // Mark the employee as inactive
            await _context.SaveChangesAsync(); // Save changes to the database
            return true;
        }
        catch
        {
            return false; // Return failure if an exception occurs
        }
    }
}