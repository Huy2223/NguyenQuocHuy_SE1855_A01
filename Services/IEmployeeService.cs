using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IEmployeeService
    {
        IEnumerable<Employees> GetAllEmployees();
        Employees GetEmployeeByID(int employeeID);
        void AddEmployee(Employees employee);
        void UpdateEmployee(Employees employee);
        void DeleteEmployee(int employeeID);
        IEnumerable<Employees> SearchEmployeesByName(string name);
        Employees Login(string username, string password);
    }
}
