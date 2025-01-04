using School.Models;
using System.Collections.Generic;

namespace School.Core.Interfaces;

// Interface for employee-related operations
public interface IEmployeeService
{
    Task<IEnumerable<Employee>> GetAllEmployeesAsync();  // Returns an async task with a collection of all employees

    Task<IEnumerable<Employee>> GetEmployeesByDepartmentAsync(int departmentId);  // Returns an async task with employees in a department (by ID) 

    Task<IEnumerable<Employee>> GetEmployeesByPositionAsync(string position); // Gets a list of employees with a specific position

    Task<Employee?> GetEmployeeByPinAsync(string pin); // Gets an employee by their unique identification number (PIN). Null if not found

    Task<bool> AddEmployeeAsync(Employee employee); // Add Employee. Bool value indicating success or failure.

    Task<IEnumerable<Department>> GetDepartments(); // Gets a list of all departments in the system

    Task<bool> RemoveEmployeeAsync(string employeePin); // Removes an employee from the system. Bool indicating success or failure
}