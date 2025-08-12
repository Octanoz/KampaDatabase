namespace DatabaseChallenge.Models;

internal record Other(int EmployeeID, string FirstName, string LastName, string JobTitle, string Category) : Employee(EmployeeID, FirstName, LastName, JobTitle);
