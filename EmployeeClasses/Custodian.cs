using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseChallenge.EmployeeClasses
{
    internal record Custodian(int EmployeeID, string FirstName, string LastName, string JobTitle, string Category) : Employee(EmployeeID, FirstName, LastName, JobTitle);
}
