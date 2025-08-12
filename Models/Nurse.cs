namespace DatabaseChallenge.Models;

using Spectre.Console;
using DatabaseChallenge.Interfaces;

internal record Nurse(int EmployeeID, string FirstName, string LastName, string JobTitle, string Level) : Employee(EmployeeID, FirstName, LastName, JobTitle), IPage
{
    public void Page()
    {
        AnsiConsole.MarkupLineInterpolated($"[mistyrose3]Paging {JobTitle} {LastName}.[/]");
    }
}
