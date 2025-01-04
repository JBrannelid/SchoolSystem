using Microsoft.EntityFrameworkCore;
using School.Core.Interfaces;
using School.Data;
using School.Models;

namespace School.Core.Services;

// Service for managing Department-related operations
public class DepartmentService : IDepartmentService
{
    private readonly SchoolContext _context;

    // Constructor that uses DI to provide the database context
    public DepartmentService(SchoolContext context)
    {
        _context = context; // The context is injected when the service is created
    }

    // Get a list of all departments
    public async Task<IEnumerable<Department>> GetAllDepartmentsAsync()
    {
        // Retrives all departments from the database
        return await _context.Departments.ToListAsync();
    }

    // Get the number of teachers in each department. We use a view insteed of writing a SQL-query 
    public async Task<IEnumerable<VwTeachersByDepartment>> GetTeacherCountByDepartmentAsync()
    {
        return await _context.VwTeachersByDepartments.ToListAsync();
    }

    // Get average salary and other stats for each department. We use a view insteed of writing a SQL-query 
    public async Task<IEnumerable<VwAvgSalaryByDepartment>> GetDepartmentSalaryStatsAsync()
    {
        return await _context.VwAvgSalaryByDepartments.ToListAsync();
    }

    // Get the total salary paid by a specific department
    public async Task<decimal> GetDepartmentTotalSalaryAsync(int departmentId)
    {
        // Get all active employees in department
        var employees = await _context.Employees
            .Where(e => e.FkdepartmentId == departmentId && e.IsActive)
            .ToListAsync();

        decimal totalSalary = 0;

        // Calculate total salary
        foreach (var employee in employees)
        {
            var salary = await _context.Salaries
                .Where(s => s.FkemployeePin == employee.EmployeePin)
                .Select(s => s.SalaryAmount)
                .FirstOrDefaultAsync();

            totalSalary += salary;
        }

        return totalSalary;
    }
}