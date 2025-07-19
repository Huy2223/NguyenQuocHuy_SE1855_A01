using BusinessObject;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeService()
        {
            _employeeRepository = new EmployeeRepository();
        }

        public IEnumerable<Employee> GetAllEmployees()
        {
            return _employeeRepository.GetAllEmployees();
        }

        public Employee GetEmployeeByID(int employeeID)
        {
            return _employeeRepository.GetEmployeeByID(employeeID);
        }

        public void AddEmployee(Employee employee)
        {
            _employeeRepository.AddEmployee(employee);
        }

        public void UpdateEmployee(Employee employee)
        {
            _employeeRepository.UpdateEmployee(employee);
        }

        public void DeleteEmployee(int employeeID)
        {
            _employeeRepository.DeleteEmployee(employeeID);
        }

        public IEnumerable<Employee> SearchEmployeesByName(string name)
        {
            return _employeeRepository.SearchEmployeesByName(name);
        }

        public Employee Login(string username, string password)
        {
            return _employeeRepository.Authenticate(username, password);
        }
    }
}
