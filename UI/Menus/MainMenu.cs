using School.Core.Interfaces;
using School.UI.MenuOptions;
using Spectre.Console;

namespace School.UI.Menus;

public class MainMenu
{
    private readonly IAnsiConsole _console;
    private readonly IStudentService _studentService;
    private readonly IEmployeeService _employeeService;
    private readonly ICourseService _courseService;
    private readonly IDepartmentService _departmentService;
    private readonly IGradeService _gradeService;

    // Constructor initializes the necessary services (injected through dependency injection) for the main menu to interact with
    // To ensure that the main menu can handle related operations
    public MainMenu(
        IAnsiConsole console,
        IStudentService studentService,
        IEmployeeService employeeService,
        ICourseService courseService,
        IDepartmentService departmentService,
        IGradeService gradeService)
    {
        _console = console;
        _studentService = studentService;
        _employeeService = employeeService;
        _courseService = courseService;
        _departmentService = departmentService;
        _gradeService = gradeService;
    }

    // This method displays the main menu of the application and processes user input
    public async Task RenderMainMenuAsync()
    {
        while (true)
        {
            _console.Clear();
            var choice = _console.Prompt(
                new SelectionPrompt<MainMenuOption>()
                    .Title("[blue]School Management System[/]")
                    .PageSize(10)
                    .AddChoices(Enum.GetValues<MainMenuOption>())
                    .UseConverter(opt => opt.ToString().SplitCamelCase()));

            try
            {
                // Depending on the user’s selection, navigate to the appropriate submenu
                switch (choice)
                {
                    case MainMenuOption.Students:
                        var studentMenu = new StudentMenu(_console, _studentService);
                        await studentMenu.ShowAsync();
                        break;

                    case MainMenuOption.Employees:
                        var employeeMenu = new EmployeeMenu(_console, _employeeService);
                        await employeeMenu.ShowAsync();
                        break;

                    case MainMenuOption.Courses:
                        var courseMenu = new CourseMenu(_console, _courseService);
                        await courseMenu.ShowAsync();
                        break;

                    case MainMenuOption.Departments:
                        var departmentMenu = new DepartmentMenu(_console, _departmentService);
                        await departmentMenu.ShowAsync();
                        break;

                    case MainMenuOption.Grades:
                        var gradeMenu = new GradeMenu(_console, _gradeService, _courseService, _employeeService);
                        await gradeMenu.ShowAsync();
                        break;

                    case MainMenuOption.Exit:
                        _console.Clear();
                        AnsiConsole.MarkupLine("[green]Thank you for using the School Management System![/]");
                        return;
                }
            }
            // If an exception occurs, print every exception and prompt the user to continue. The program wont crash in case of an error
            catch (Exception ex)
            {
                _console.WriteException(ex);
                await PauseAsync();
            }
        }
    }

    // PauseAsync pause the application and prompt the user to press "Enter"
    // It is used after an exception or when the system needs to wait for the user's input
    private async Task PauseAsync()
    {
        _console.WriteLine();
        _console.Prompt(
            new TextPrompt<string>("Press [green]Enter[/] to continue...")
                .AllowEmpty()); // Allow empty input
    }
}