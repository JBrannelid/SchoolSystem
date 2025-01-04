namespace School.Models;

public partial class Salary
{
    public int SalaryId { get; set; }

    public string FkemployeePin { get; set; } = null!;

    public decimal SalaryAmount { get; set; }

    public virtual Employee FkemployeePinNavigation { get; set; } = null!;
}