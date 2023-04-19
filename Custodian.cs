using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseChallenge
{
    internal class Custodian : Employee
    {
        public Custodian(int employeeID, string firstName, string lastName, string jobTitle) : base(employeeID, firstName, lastName, jobTitle)
        {
        }
    }
}
