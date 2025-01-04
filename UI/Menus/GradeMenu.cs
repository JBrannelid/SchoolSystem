using School.Core.Interfaces;
using School.Models;
using School.UI.MenuOptions;
using Spectre.Console;

namespace School.UI.Menus;

public class GradeMenu : MenuBase
{
    private readonly IGradeService _gradeService;
    private readonly ICourseService _courseService;
    private readonly IEmployeeService _employeeService;

    // Constructor to initialize the menu with necessary services
    public GradeMenu(
        IAnsiConsole console,
        IGradeService gradeService,
        ICourseService courseService,
        IEmployeeService employeeService)
        : base(console)
    {
        _gradeService = gradeService;
        _courseService = courseService;
        _employeeService = employeeService;
    }

    // Main method to display menu and handle user interactions
    public async Task ShowAsync()
    {
        while (true)
        {
            ShowHeader("Grade Management");

            // Get user menu choice
            var choice = _console.Prompt(
                new SelectionPrompt<GradeMenuOption>()
                    .Title("Select an option:")
                    .PageSize(10)
                    .AddChoices(Enum.GetValues<GradeMenuOption>())
                    .UseConverter(opt => opt.ToString().SplitCamelCase()));

            try
            {
                switch (choice)
                {
                    case GradeMenuOption.AssignGrade:
                        await AssignGradeAsync();
                        break;
                    case GradeMenuOption.ViewStudentGrades:
                        await ViewStudentGradesAsync();
                        break;
                    case GradeMenuOption.ViewDetailedStudentInfo:
                        await ViewDetailedStudentInfoAsync();
                        break;
                    case GradeMenuOption.Back:
                        return;
                }
            }
            catch (Exception ex)
            {
                await ShowErrorAsync(ex.Message);
            }
        }
    }

    // Assign a grade to a student for a specific course
    private async Task AssignGradeAsync()
    {
        ShowHeader("Assign Grade");

        // Get student PIN
        var studentPin = GetValidPin();

        // Get available courses and validate
        var courses = await _courseService.GetActiveCourses();
        if (!courses.Any())
        {
            await ShowErrorAsync("No active courses found");
            return;
        }

        // Let user select a course
        var selectedCourse = _console.Prompt(
            new SelectionPrompt<Course>()
                .Title("[blue]Select Course:[/]")
                .PageSize(10)
                .AddChoices(courses)
                .UseConverter(c => $"{c.CourseCode} - {c.CourseName}"));

        // Get available teachers and validate
        var teachers = await _employeeService.GetEmployeesByPositionAsync("Teacher");
        if (!teachers.Any())
        {
            await ShowErrorAsync("No teachers found");
            return;
        }

        // Let user select a teacher
        var selectedTeacher = _console.Prompt(
            new SelectionPrompt<Employee>()
                .Title("[blue]Select Teacher:[/]")
                .PageSize(10)
                .AddChoices(teachers)
                .UseConverter(t => $"{t.FirstName} {t.LastName}"));

        // Get available grades and validate
        var grades = await _gradeService.GetAllGradeValuesAsync();
        var selectedGrade = _console.Prompt(
            new SelectionPrompt<GradeValue>()
                .Title("[blue]Select Grade:[/]")
                .PageSize(10)
                .AddChoices(grades)
                .UseConverter(g => $"{g.Grade.Trim()} (Value: {g.GradeValue1})"));

        // Confirm and assign the grade
        if (await ConfirmActionAsync(
            $"assign grade {selectedGrade.Grade.Trim()} to student {studentPin}"))
        {
            await ShowLoadingSpinnerAsync("Assigning grade...", async () =>
            {
                var success = await _gradeService.AssignGradeAsync(
                    studentPin,
                    selectedCourse.CourseId,
                    selectedTeacher.EmployeePin,
                    selectedGrade.Grade.Trim());

                if (success)
                    await ShowConfirmationAsync("Grade assigned successfully!");
                else
                    await ShowErrorAsync("Failed to assign grade");
            });
        }
    }

    // View all grades for a specific student
    private async Task ViewStudentGradesAsync()
    {
        var studentPin = GetValidPin();

        await ShowLoadingSpinnerAsync("Loading grades...", async () =>
        {
            var grades = await _gradeService.GetStudentGradesAsync(studentPin);

            if (!grades.Any())
            {
                await ShowErrorAsync("No grades found for this student");
                return;
            }

            DisplayGradesTable(grades);
            await DisplayGradeDistributionAsync(grades);
            await PauseAsync();
        });
    }

    // Display grades in a table format
    private void DisplayGradesTable(IEnumerable<VwStudentCourseGrade> grades)
    {
        var table = CreateBaseTable("Course", "Code", "Teacher", "Grade", "Date Assigned");

        foreach (var grade in grades)
        {
            var gradeColor = GetGradeColor(grade.Grade);
            table.AddRow(
                grade.CourseName,
                grade.CourseCode,
                $"{grade.TeacherFirstName} {grade.TeacherLastName}",
                grade.Grade is null ? "N/A" : $"[{gradeColor}]{grade.Grade}[/]",
                grade.GradeAssignedDate?.ToString("yyyy-MM-dd") ?? "N/A"
            );
        }

        _console.Write(table);
    }

    // Display grade distribution as a bar chart
    private async Task DisplayGradeDistributionAsync(IEnumerable<VwStudentCourseGrade> grades)
    {
        var gradeDistribution = grades
            .Where(g => g.Grade != null)
            .GroupBy(g => g.Grade?.Trim())
            .OrderBy(g => g.Key)
            .ToDictionary(g => g.Key ?? "N/A", g => g.Count());

        var chart = new BarChart()
            .Width(60)
            .Label("[blue]Grade Distribution[/]")
            .CenterLabel();

        foreach (var grade in gradeDistribution)
        {
            var color = GetGradeColor(grade.Key);
            chart.AddItem(
                grade.Key,
                grade.Value,
                Color.FromConsoleColor(GetConsoleColor(color)));
        }

        _console.WriteLine();
        _console.Write(chart);
    }

    // View detailed information about a student's grades and teachers
    private async Task ViewDetailedStudentInfoAsync()
    {
        var studentPin = GetValidPin();

        await ShowLoadingSpinnerAsync("Loading student information...", async () =>
        {
            var studentGrades = await _gradeService.GetDetailedStudentInfoAsync(studentPin);

            if (!studentGrades.Any())
            {
                await ShowErrorAsync("No information found for this student");
                return;
            }

            var firstRecord = studentGrades.First();

            // Display student information panel
            DisplayStudentInfoPanel(firstRecord);

            // Display detailed grade information
            DisplayDetailedGradesTable(studentGrades);

            await PauseAsync();
        });
    }

    // Display student information in a panel
    private void DisplayStudentInfoPanel(VwStudentCourseGrade studentInfo)
    {
        var panel = new Panel(
            $"Name: {studentInfo.StudentFirstName} {studentInfo.StudentLastName}")
        {
            Border = BoxBorder.Rounded,
            Header = new PanelHeader($"Student Information ({studentInfo.StudentPin})")
        };

        _console.Write(panel);
        _console.WriteLine();
    }

    // Display detailed grades in a table
    private void DisplayDetailedGradesTable(IEnumerable<VwStudentCourseGrade> grades)
    {
        var table = CreateBaseTable("Teacher", "Grade", "Date");

        foreach (var grade in grades)
        {
            if (grade.TeacherFirstName != null)
            {
                var gradeColor = GetGradeColor(grade.Grade);
                table.AddRow(
                    $"{grade.TeacherFirstName} {grade.TeacherLastName}",
                    grade.Grade == null ? "N/A" : $"[{gradeColor}]{grade.Grade}[/]",
                    grade.GradeAssignedDate?.ToString("yyyy-MM-dd") ?? "N/A"
                );
            }
        }

        _console.Write(table);
    }

    // Helper method to get a valid PIN input
    private string GetValidPin()
    {
        return _console.Prompt(
            new TextPrompt<string>("[blue]Enter student PIN (YYYYMMDD-XXXX):[/]")
                .Validate(pin =>
                {
                    if (!(pin.Length == 13 && pin[8] == '-'))
                        return ValidationResult.Error("[red]Invalid PIN format[/]");
                    return ValidationResult.Success();
                }));
    }

    // Helper method to get color for grade display
    private string GetGradeColor(string? grade)
    {
        if (grade == null)
            return "grey";

        return grade.Trim() switch
        {
            "A" => "green",
            "B" => "blue",
            "C" => "yellow",
            "D" => "orange",
            "F" => "red",
            _ => "white"
        };
    }

    // Helper method to convert color strings to ConsoleColor
    private ConsoleColor GetConsoleColor(string color)
    {
        return color switch
        {
            "green" => ConsoleColor.Green,
            "blue" => ConsoleColor.Blue,
            "yellow" => ConsoleColor.Yellow,
            "orange" => ConsoleColor.DarkYellow,
            "red" => ConsoleColor.Red,
            "grey" => ConsoleColor.Gray,
            _ => ConsoleColor.White
        };
    }
}