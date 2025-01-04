namespace School.Models;

public partial class VwAvgSalaryByDepartment
{
    public string DepartmentName { get; set; } = null!;

    public decimal? AvgSalary { get; set; }
}