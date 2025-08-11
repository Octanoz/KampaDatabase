using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseChallenge
{
    internal record Custodian(int EmployeeID, string FirstName, string LastName, string JobTitle) : Employee(EmployeeID, FirstName, LastName, JobTitle);
}
