using School.Models;

namespace School.Core.Interfaces;

// Interface for department-related operations
public interface IDepartmentService
{
    Task<IEnumerable<Department>> GetAllDepartmentsAsync(); // Returns an async task with a collection of all departments

    Task<IEnumerable<VwTeachersByDepartment>> GetTeacherCountByDepartmentAsync(); // Returns an async task with teacher counts by department


    Task<IEnumerable<VwAvgSalaryByDepartment>> GetDepartmentSalaryStatsAsync(); // Returns an async task with salary statistics by department

    Task<decimal> GetDepartmentTotalSalaryAsync(int departmentId); // Returns an async task with the total salary 

}