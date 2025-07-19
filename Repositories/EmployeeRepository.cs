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

        public IEnumerable<Employee> GetAllEmployees()
        {
            return employeeDAO.GetAllEmployees();
        }

        public Employee GetEmployeeByID(int employeeID)
        {
            return employeeDAO.GetEmployeeByID(employeeID);
        }

        public void AddEmployee(Employee employee)
        {
            employeeDAO.AddEmployee(employee);
        }

        public void UpdateEmployee(Employee employee)
        {
            employeeDAO.UpdateEmployee(employee);
        }

        public void DeleteEmployee(int employeeID)
        {
            employeeDAO.DeleteEmployee(employeeID);
        }

        public IEnumerable<Employee> SearchEmployeesByName(string name)
        {
            return employeeDAO.SearchEmployeesByName(name);
        }

        public Employee Authenticate(string username, string password)
        {
            return employeeDAO.Authenticate(username, password);
        }
    }
}
