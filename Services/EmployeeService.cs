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

        public IEnumerable<Employees> GetAllEmployees()
        {
            return _employeeRepository.GetAllEmployees();
        }

        public Employees GetEmployeeByID(int employeeID)
        {
            return _employeeRepository.GetEmployeeByID(employeeID);
        }

        public void AddEmployee(Employees employee)
        {
            _employeeRepository.AddEmployee(employee);
        }

        public void UpdateEmployee(Employees employee)
        {
            _employeeRepository.UpdateEmployee(employee);
        }

        public void DeleteEmployee(int employeeID)
        {
            _employeeRepository.DeleteEmployee(employeeID);
        }

        public IEnumerable<Employees> SearchEmployeesByName(string name)
        {
            return _employeeRepository.SearchEmployeesByName(name);
        }

        public Employees Login(string username, string password)
        {
            return _employeeRepository.Authenticate(username, password);
        }
    }
}
