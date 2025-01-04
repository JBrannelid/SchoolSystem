namespace School.Models;

public partial class VwEmployeeOverview
{
    public string EmployeePin { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Position { get; set; } = null!;

    public int? YearsWorked { get; set; }

    public string Department { get; set; } = null!;
}