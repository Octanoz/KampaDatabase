using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseChallenge
{
    internal abstract record Employee(int EmployeeID, string FirstName, string LastName, string JobTitle);
}
