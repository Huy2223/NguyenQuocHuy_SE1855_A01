using BusinessObject;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class EmployeeDAO
    {
        // Get all employees
        public List<Employee> GetAllEmployees()
        {
            using var context = new LucySalesDataContext();
            return context.Employees.ToList();
        }

        // Get employee by ID
        public Employee GetEmployeeByID(int employeeID)
        {
            using var context = new LucySalesDataContext();
            return context.Employees.FirstOrDefault(e => e.EmployeeId == employeeID);
        }

        // Add a new employee
        public void AddEmployee(Employee employee)
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

            using var context = new LucySalesDataContext();
            
            // Check if username already exists
            if (context.Employees.Any(e => e.UserName.Equals(employee.UserName)))
                throw new ArgumentException("Username already exists");

            context.Employees.Add(employee);
            context.SaveChanges();
        }

        // Update an existing employee
        public void UpdateEmployee(Employee employee)
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

            using var context = new LucySalesDataContext();
            
            // Find existing employee
            var existingEmployee = context.Employees.FirstOrDefault(e => e.EmployeeId == employee.EmployeeId);
            if (existingEmployee == null)
                throw new ArgumentException($"Employee with ID {employee.EmployeeId} not found");

            // Check if updated username already exists (excluding current employee)
            if (context.Employees.Any(e => e.EmployeeId != employee.EmployeeId && 
                               e.UserName.Equals(employee.UserName)))
                throw new ArgumentException("Username already exists");

            // Update properties that exist in the database
            existingEmployee.Name = employee.Name;
            existingEmployee.UserName = employee.UserName;
            existingEmployee.Password = employee.Password;
            existingEmployee.JobTitle = employee.JobTitle;
            existingEmployee.Address = employee.Address;
            existingEmployee.BirthDate = employee.BirthDate;
            existingEmployee.HireDate = employee.HireDate;
            // Note: IsAdmin property is not in the database schema, so we don't update it here
            
            context.SaveChanges();
        }

        // Delete an employee
        public void DeleteEmployee(int employeeID)
        {
            using var context = new LucySalesDataContext();
            var employee = context.Employees.FirstOrDefault(e => e.EmployeeId == employeeID);
            if (employee == null)
                throw new ArgumentException($"Employee with ID {employeeID} not found");

            context.Employees.Remove(employee);
            context.SaveChanges();
        }

        // Search employees by name
        public List<Employee> SearchEmployeesByName(string name)
        {
            using var context = new LucySalesDataContext();
            if (string.IsNullOrWhiteSpace(name))
                return context.Employees.ToList();

            return context.Employees.Where(e => e.Name.Contains(name)).ToList();
        }

        // Authenticate employee
        public Employee Authenticate(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return null;

            using var context = new LucySalesDataContext();
            return context.Employees.FirstOrDefault(e => 
                e.UserName.Equals(username) && 
                e.Password.Equals(password));
        }
    }
}
