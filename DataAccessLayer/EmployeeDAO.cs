using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class EmployeeDAO
    {
        // Mock data - in a real application, this would be stored in a database
        private static List<Employees> _employees = new List<Employees>
        {
            new Employees
            {
                EmployeeID = 1,
                Name = "Admin User",
                UserName = "admin",
                Password = "admin123",
                JobTitle = "System Administrator",
                IsAdmin = true
            },
            new Employees
            {
                EmployeeID = 2,
                Name = "John Doe",
                UserName = "john",
                Password = "john123",
                JobTitle = "Sales Manager",
                IsAdmin = true
            },
            new Employees
            {
                EmployeeID = 3,
                Name = "Jane Smith",
                UserName = "jane",
                Password = "jane123",
                JobTitle = "Customer Service Representative",
                IsAdmin = true
            }
        };

        private static int _nextId = 4;

        // Get all employees
        public List<Employees> GetAllEmployees()
        {
            return _employees;
        }

        // Get employee by ID
        public Employees GetEmployeeByID(int employeeID)
        {
            return _employees.FirstOrDefault(e => e.EmployeeID == employeeID);
        }

        // Add a new employee
        public void AddEmployee(Employees employee)
        {
            if (employee == null)
                throw new ArgumentNullException(nameof(employee));

            // Validate required fields
            if (string.IsNullOrWhiteSpace(employee.Name))
                throw new ArgumentException("Employee name is required");
            if (string.IsNullOrWhiteSpace(employee.UserName))
                throw new ArgumentException("Username is required");
            if (string.IsNullOrWhiteSpace(employee.Password))
                throw new ArgumentException("Password is required");
            if (string.IsNullOrWhiteSpace(employee.JobTitle))
                throw new ArgumentException("Job title is required");

            // Check if username already exists
            if (_employees.Any(e => e.UserName.Equals(employee.UserName, StringComparison.OrdinalIgnoreCase)))
                throw new ArgumentException("Username already exists");

            // Set new ID
            employee.EmployeeID = _nextId++;
            _employees.Add(employee);
        }

        // Update an existing employee
        public void UpdateEmployee(Employees employee)
        {
            if (employee == null)
                throw new ArgumentNullException(nameof(employee));

            // Validate required fields
            if (string.IsNullOrWhiteSpace(employee.Name))
                throw new ArgumentException("Employee name is required");
            if (string.IsNullOrWhiteSpace(employee.UserName))
                throw new ArgumentException("Username is required");
            if (string.IsNullOrWhiteSpace(employee.Password))
                throw new ArgumentException("Password is required");
            if (string.IsNullOrWhiteSpace(employee.JobTitle))
                throw new ArgumentException("Job title is required");

            // Find existing employee
            var existingEmployee = _employees.FirstOrDefault(e => e.EmployeeID == employee.EmployeeID);
            if (existingEmployee == null)
                throw new ArgumentException($"Employee with ID {employee.EmployeeID} not found");

            // Check if updated username already exists (excluding current employee)
            if (_employees.Any(e => e.EmployeeID != employee.EmployeeID && 
                               e.UserName.Equals(employee.UserName, StringComparison.OrdinalIgnoreCase)))
                throw new ArgumentException("Username already exists");

            // Update properties
            existingEmployee.Name = employee.Name;
            existingEmployee.UserName = employee.UserName;
            existingEmployee.Password = employee.Password;
            existingEmployee.JobTitle = employee.JobTitle;
            existingEmployee.IsAdmin = employee.IsAdmin;
        }

        // Delete an employee
        public void DeleteEmployee(int employeeID)
        {
            var employee = _employees.FirstOrDefault(e => e.EmployeeID == employeeID);
            if (employee == null)
                throw new ArgumentException($"Employee with ID {employeeID} not found");

            _employees.Remove(employee);
        }

        // Search employees by name
        public List<Employees> SearchEmployeesByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return _employees;

            return _employees.Where(e => e.Name.Contains(name, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        // Authenticate employee
        public Employees Authenticate(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return null;

            return _employees.FirstOrDefault(e => 
                e.UserName.Equals(username, StringComparison.OrdinalIgnoreCase) && 
                e.Password.Equals(password));
        }
    }
}
