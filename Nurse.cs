namespace DatabaseChallenge;

using Spectre.Console;

internal record Nurse(int EmployeeID, string FirstName, string LastName, string JobTitle, string Level) : Employee(EmployeeID, FirstName, LastName, JobTitle), IPage
{
    public void Page()
    {
        AnsiConsole.MarkupLineInterpolated($"[mistyrose3]Paging {JobTitle} {LastName}.[/]");
    }
}
