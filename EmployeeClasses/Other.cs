namespace DatabaseChallenge.EmployeeClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal record Other(int EmployeeID, string FirstName, string LastName, string JobTitle, string Category) : Employee(EmployeeID, FirstName, LastName, JobTitle);
