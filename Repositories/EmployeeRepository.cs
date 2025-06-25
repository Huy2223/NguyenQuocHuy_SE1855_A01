using BusinessObject;
using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private EmployeeDAO employeeDAO;

        public EmployeeRepository()
        {
            employeeDAO = new EmployeeDAO();
        }

        public IEnumerable<Employees> GetAllEmployees()
        {
            return employeeDAO.GetAllEmployees();
        }

        public Employees GetEmployeeByID(int employeeID)
        {
            return employeeDAO.GetEmployeeByID(employeeID);
        }

        public void AddEmployee(Employees employee)
        {
            employeeDAO.AddEmployee(employee);
        }

        public void UpdateEmployee(Employees employee)
        {
            employeeDAO.UpdateEmployee(employee);
        }

        public void DeleteEmployee(int employeeID)
        {
            employeeDAO.DeleteEmployee(employeeID);
        }

        public IEnumerable<Employees> SearchEmployeesByName(string name)
        {
            return employeeDAO.SearchEmployeesByName(name);
        }

        public Employees Authenticate(string username, string password)
        {
            return employeeDAO.Authenticate(username, password);
        }
    }
}
