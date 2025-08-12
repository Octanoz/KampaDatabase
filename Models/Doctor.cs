namespace DatabaseChallenge.Models;

using Spectre.Console;
using DatabaseChallenge.Interfaces;

internal record Doctor(int EmployeeID, string FirstName, string LastName, string JobTitle, string Specialization) : Employee(EmployeeID, FirstName, LastName, JobTitle), IPage
{
    public void Page()
    {
        AnsiConsole.MarkupLineInterpolated($"[teal]Paging {JobTitle} {LastName}.[/]");
    }
}
