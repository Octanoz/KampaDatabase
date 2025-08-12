namespace DatabaseChallenge.EmployeeClasses;

internal record Custodian(int EmployeeID, string FirstName, string LastName, string JobTitle, string Category) : Employee(EmployeeID, FirstName, LastName, JobTitle);
