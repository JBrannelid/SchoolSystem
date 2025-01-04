namespace School.Models;

public partial class Employee
{
    public string EmployeePin { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Gender { get; set; } = null!;

    public DateOnly StartDate { get; set; }

    public string Position { get; set; } = null!;

    public int FkdepartmentId { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<CourseEnrollment> CourseEnrollments { get; set; } = new List<CourseEnrollment>();

    public virtual Department Fkdepartment { get; set; } = null!;

    public virtual ICollection<Salary> Salaries { get; set; } = new List<Salary>();
}