using School.Core.Interfaces;
using School.Models;
using School.UI.MenuOptions;
using Spectre.Console;

namespace School.UI.Menus;

public class EmployeeMenu : MenuBase
{
    private readonly IEmployeeService _employeeService;

    // Constructor that initializes the menu with console and employee service
    public EmployeeMenu(IAnsiConsole console, IEmployeeService employeeService)
        : base(console)
    {
        _employeeService = employeeService;
    }

    // Main method to display menu and handle user interactions
    public async Task ShowAsync()
    {
        while (true)
        {
            _console.Clear();
            ShowHeader("Employee Management");

            // Get user menu choice
            var choice = _console.Prompt(
                new SelectionPrompt<EmployeeMenuOption>()
                    .Title("Select an option:")
                    .PageSize(10)
                    .AddChoices(Enum.GetValues<EmployeeMenuOption>())
                    .UseConverter(opt => opt.ToString().SplitCamelCase()));

            try
            {
                switch (choice)
                {
                    case EmployeeMenuOption.ViewAll:
                        await ShowAllEmployeesAsync();
                        break;
                    case EmployeeMenuOption.ViewByDepartment:
                        await ShowEmployeesByDepartmentAsync();
                        break;
                    case EmployeeMenuOption.ViewByRole:
                        await ShowEmployeesByRoleAsync();
                        break;
                    case EmployeeMenuOption.AddNew:
                        await AddNewEmployeeAsync();
                        break;
                    case EmployeeMenuOption.Remove:
                        await RemoveEmployeeAsync();
                        break;
                    case EmployeeMenuOption.Back:
                        return;
                }
            }
            catch (Exception ex)
            {
                await ShowErrorAsync(ex.Message);
            }
        }
    }

    // Display all employees in the system
    private async Task ShowAllEmployeesAsync()
    {
        await ShowLoadingSpinnerAsync("Loading employees...", async () =>
        {
            var employees = await _employeeService.GetAllEmployeesAsync();
            await DisplayEmployeesAsync(employees);
        });
    }

    // Display employees filtered by department
    private async Task ShowEmployeesByDepartmentAsync()
    {
        var departments = await _employeeService.GetDepartments();

        if (!departments.Any())
        {
            await ShowErrorAsync("No departments found");
            return;
        }

        // Let user select a department
        var selectedDepartment = _console.Prompt(
            new SelectionPrompt<Department>()
                .Title("[blue]Select Department:[/]")
                .PageSize(10)
                .AddChoices(departments)
                .UseConverter(d => d.Name));

        await ShowLoadingSpinnerAsync(
            $"Loading employees from {selectedDepartment.Name}...",
            async () =>
            {
                var employees = await _employeeService.GetEmployeesByDepartmentAsync(
                    selectedDepartment.DepartmentId);
                await DisplayEmployeesAsync(employees);
            });
    }

    // Display employees filtered by their position/role
    private async Task ShowEmployeesByRoleAsync()
    {
        var positions = new[] { "Principal", "Administrator", "Teacher" };

        var position = _console.Prompt(
            new SelectionPrompt<string>()
                .Title("[blue]Select Position:[/]")
                .PageSize(10)
                .AddChoices(positions));

        await ShowLoadingSpinnerAsync($"Loading {position}s...", async () =>
        {
            var employees = await _employeeService.GetEmployeesByPositionAsync(position);
            await DisplayEmployeesAsync(employees);
        });
    }

    // Display employee information in a formatted table
    private async Task DisplayEmployeesAsync(IEnumerable<Employee> employees)
    {
        _console.Clear();
        var table = CreateBaseTable(
            "PIN",
            "Name",
            "Position",
            "Department",
            "Start Date",
            "Status"
        );

        foreach (var employee in employees)
        {
            string statusColor = employee.IsActive ? "green" : "red";
            string status = employee.IsActive ? "Active" : "Inactive";

            table.AddRow(
                employee.EmployeePin,
                $"{employee.FirstName} {employee.LastName}",
                employee.Position,
                employee.Fkdepartment.Name,
                employee.StartDate.ToString("yyyy-MM-dd"),
                $"[{statusColor}]{status}[/]"
            );
        }

        _console.Write(table);
        _console.WriteLine();

        // Display statistics
        var stats = new BarChart()
            .Width(60)
            .Label("[blue]Employees by Position[/]")
            .CenterLabel()
            .AddItem("Teachers", employees.Count(e => e.Position == "Teacher"), Color.Blue)
            .AddItem("Administrators", employees.Count(e => e.Position == "Administrator"), Color.Green)
            .AddItem("Principals", employees.Count(e => e.Position == "Principal"), Color.Yellow);

        _console.Write(stats);
        await PauseAsync();
    }

    // Add a new employee to the system
    private async Task AddNewEmployeeAsync()
    {
        ShowHeader("Add New Employee");

        // Get employee details
        var firstName = GetValidName("First Name");
        var lastName = GetValidName("Last Name");
        var pin = GetValidPin();

        var gender = _console.Prompt(
            new SelectionPrompt<string>()
                .Title("[blue]Select Gender:[/]")
                .AddChoices(new[] { "Male", "Female", "Other" }));

        var position = _console.Prompt(
            new SelectionPrompt<string>()
                .Title("[blue]Select Position:[/]")
                .AddChoices(new[] { "Principal", "Administrator", "Teacher" }));

        // Get department selection
        var departments = await _employeeService.GetDepartments();
        var selectedDepartment = _console.Prompt(
            new SelectionPrompt<Department>()
                .Title("[blue]Select Department:[/]")
                .PageSize(10)
                .AddChoices(departments)
                .UseConverter(d => d.Name));

        // Create new employee
        var employee = new Employee
        {
            EmployeePin = pin,
            FirstName = firstName,
            LastName = lastName,
            Gender = gender,
            Position = position,
            StartDate = DateOnly.FromDateTime(DateTime.Now),
            IsActive = true,
            FkdepartmentId = selectedDepartment.DepartmentId
        };

        // Save the new employee
        await ShowLoadingSpinnerAsync("Adding new employee...", async () =>
        {
            var success = await _employeeService.AddEmployeeAsync(employee);
            if (success)
                await ShowConfirmationAsync("Employee added successfully!");
            else
                await ShowErrorAsync("Failed to add employee. Please check the input and try again.");
        });
    }

    // Remove an employee from the system (soft delete)
    private async Task RemoveEmployeeAsync()
    {
        var employees = await _employeeService.GetAllEmployeesAsync();
        if (!employees.Any())
        {
            await ShowErrorAsync("No employees found");
            return;
        }

        // Let user select an employee to remove
        var selectedEmployee = _console.Prompt(
            new SelectionPrompt<Employee>()
                .Title("[blue]Select employee to remove:[/]")
                .PageSize(10)
                .AddChoices(employees)
                .UseConverter(e => $"{e.FirstName} {e.LastName} ({e.Position})"));

        if (await ConfirmActionAsync(
            $"remove {selectedEmployee.FirstName} {selectedEmployee.LastName}"))
        {
            await ShowLoadingSpinnerAsync("Removing employee...", async () =>
            {
                var success = await _employeeService.RemoveEmployeeAsync(
                    selectedEmployee.EmployeePin);
                if (success)
                    await ShowConfirmationAsync("Employee removed successfully!");
                else
                    await ShowErrorAsync("Failed to remove employee");
            });
        }
    }

    // Helper method to get a valid name input
    private string GetValidName(string fieldName)
    {
        return _console.Prompt(
            new TextPrompt<string>($"[blue]Enter {fieldName}:[/]")
                .Validate(name =>
                {
                    if (string.IsNullOrWhiteSpace(name))
                        return ValidationResult.Error($"[red]{fieldName} cannot be empty[/]");
                    return ValidationResult.Success();
                }));
    }

    // Helper method to get a valid PIN input
    private string GetValidPin()
    {
        return _console.Prompt(
            new TextPrompt<string>("[blue]Enter PIN (YYYYMMDD-XXXX):[/]")
                .Validate(pin =>
                {
                    if (pin.Length != 13 || pin[8] != '-')
                        return ValidationResult.Error("[red]Invalid PIN format[/]");
                    return ValidationResult.Success();
                }));
    }
}