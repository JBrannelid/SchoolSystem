namespace School.UI.MenuOptions;

// Enums for the menu options available to the user
// Enums are used in the menu selection and are handled through a switch-case in every user menus
// A helper class called StringExtensions.cs is used to split the Enum values written in CamelCase into separate words for better readability
public enum MainMenuOption
{
    Students,
    Employees,
    Departments,
    Courses,
    Grades,
    Exit
}

public enum StudentMenuOption
{
    ViewAll,
    ViewByClass,
    Search,        
    AddNew,
    Update,      
    Remove,  
    Filter,       
    Back
}

public enum EmployeeMenuOption
{
    ViewAll,
    ViewByDepartment,
    ViewByRole,
    AddNew,
    Remove,  
    Back
}

public enum CourseMenuOption
{
    ViewAll,
    ViewActive,
    AddNew,
    Update,
    Remove,
    ViewGrades,
    Back
}

public enum DepartmentMenuOption
{
    ViewTeacherCount,
    ViewSalaryStats,
    ViewTotalSalaries,
    Back
}

public enum GradeMenuOption
{
    AssignGrade,
    ViewStudentGrades,
    ViewDetailedStudentInfo,
    Back
}