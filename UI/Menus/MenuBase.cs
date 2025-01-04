using Spectre.Console;

namespace School.UI.Menus;

public abstract class MenuBase
{
    protected readonly IAnsiConsole _console;

    // Constructor that initializes a menu with the console interface (DI-injected).
    protected MenuBase(IAnsiConsole console)
    {
        _console = console; // The console is injected when the service is needed
    }

    // Show a loading spinner and a custom message while executing an asynchronous action for a good UX
    // The Func task Action is used for the async operation to peform (Ex. PauseAsync)
    protected async Task ShowLoadingSpinnerAsync(string message, Func<Task> action)
    {
        await action();
    }

    // Clears the console and displays a header for the current menu (string title). Provides a good UX
    // - title: The text to display in the header
    protected void ShowHeader(string title)
    {
        _console.Clear();

        var rule = new Rule($"[blue]{title}[/]")
        {
            Style = Style.Parse("blue")
        };

        _console.Write(rule);
        _console.WriteLine();
    }

    // Promt user to press Enter to continue, ensuring they have time to read the confirmation
    // - message: The success message to display
    protected async Task ShowConfirmationAsync(string message)
    {
        _console.Clear();
        _console.MarkupLine($"[green]{message}[/]");
        await PauseAsync();
    }

    // Shows an error message in a red panel. It helps the user understand if something went wrong
    // - message: The error message to display
    protected async Task ShowErrorAsync(string message)
    {
        _console.Clear();
        _console.MarkupLine($"[red]{message}[/]");
        await PauseAsync();
    }

    // Pprompt with "Yes" and "No" options to confirm a potentially irreversible action
    // - action: Description of the action requiring confirmation
    // Return a bool value, yes = true/ no = false
    protected async Task<bool> ConfirmActionAsync(string action)
    {
        _console.Clear();
        return _console.Prompt(
            new SelectionPrompt<string>()
                .Title($"[yellow]Are you sure you want to {action}?[/]")
                .AddChoices(new[] { "Yes", "No" })) == "Yes";
    }

    // Prompt to the user asking them to press Enter to continue to give the user a moment to read it.
    protected async Task PauseAsync()
    {
        _console.WriteLine();
        _console.Prompt(
            new TextPrompt<string>("[grey]Press [green]Enter[/] to continue...[/]")
                .AllowEmpty());
    }

    // This method is nesesary to create a table with a border and columns for the ANSI console
    // The columns are defined by passing an array of strings (columns names)
    protected Table CreateBaseTable(params string[] columns)
    {
        var table = new Table().Border(TableBorder.Rounded);
        foreach (var column in columns)
        {
            table.AddColumn(new TableColumn($"[blue]{column}[/]"));
        }
        return table;
    }
}