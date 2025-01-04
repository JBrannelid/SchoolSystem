namespace School.Models;

public partial class Class
{
    public int ClassId { get; set; }

    public string ClassName { get; set; } = null!;

    public int Year { get; set; }

    public string Section { get; set; } = null!;

    public bool IsActive { get; set; }

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}