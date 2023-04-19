using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseChallenge
{
    internal abstract class Employee
    {

        public int EmployeeID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string JobTitle { get; set; }

        protected Employee(int employeeID, string firstName, string lastName, string jobTitle)
        {
            EmployeeID = employeeID;
            FirstName = firstName;
            LastName = lastName;
            JobTitle = jobTitle;
        }
    }
}
