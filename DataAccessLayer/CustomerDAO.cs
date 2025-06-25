using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class CustomerDAO
    {
        // Mock data - in a real application, this would be stored in a database
        private static List<Customers> _customers = new List<Customers>
        {
            new Customers
            {
                CustomerID = 1,
                CompanyName = "ABC Company",
                ContactName = "John Smith",
                ContactTitle = "Purchasing Manager",
                Address = "123 Main Street, New York",
                Phone = "555-1234"
            },
            new Customers
            {
                CustomerID = 2,
                CompanyName = "XYZ Corporation",
                ContactName = "Jane Doe",
                ContactTitle = "CEO",
                Address = "456 Broadway, Boston",
                Phone = "555-5678"
            },
            new Customers
            {
                CustomerID = 3,
                CompanyName = "Global Enterprises",
                ContactName = "Michael Johnson",
                ContactTitle = "Procurement Officer",
                Address = "789 Fifth Avenue, Chicago",
                Phone = "555-9012"
            }
        };

        private static int _nextId = 4;

        // Get all customers
        public List<Customers> GetAllCustomers()
        {
            return _customers;
        }

        // Get customer by ID
        public Customers GetCustomerByID(int customerID)
        {
            return _customers.FirstOrDefault(c => c.CustomerID == customerID);
        }

        // Add a new customer
        public void AddCustomer(Customers customer)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            // Validate required fields
            if (string.IsNullOrWhiteSpace(customer.CompanyName))
                throw new ArgumentException("Company name is required");
            if (string.IsNullOrWhiteSpace(customer.ContactName))
                throw new ArgumentException("Contact name is required");

            // Set new ID
            customer.CustomerID = _nextId++;
            _customers.Add(customer);
        }

        // Update an existing customer
        public void UpdateCustomer(Customers customer)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            // Validate required fields
            if (string.IsNullOrWhiteSpace(customer.CompanyName))
                throw new ArgumentException("Company name is required");
            if (string.IsNullOrWhiteSpace(customer.ContactName))
                throw new ArgumentException("Contact name is required");

            // Find existing customer
            var existingCustomer = _customers.FirstOrDefault(c => c.CustomerID == customer.CustomerID);
            if (existingCustomer == null)
                throw new ArgumentException($"Customer with ID {customer.CustomerID} not found");

            // Update properties
            existingCustomer.CompanyName = customer.CompanyName;
            existingCustomer.ContactName = customer.ContactName;
            existingCustomer.ContactTitle = customer.ContactTitle;
            existingCustomer.Address = customer.Address;
            existingCustomer.Phone = customer.Phone;
        }

        // Delete a customer
        public void DeleteCustomer(int customerID)
        {
            var customer = _customers.FirstOrDefault(c => c.CustomerID == customerID);
            if (customer == null)
                throw new ArgumentException($"Customer with ID {customerID} not found");

            _customers.Remove(customer);
        }

        // Search customers by name
        public List<Customers> SearchCustomersByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return _customers;

            return _customers.Where(c => c.ContactName.Contains(name, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        // Search customers by company name
        public List<Customers> SearchCustomersByCompany(string companyName)
        {
            if (string.IsNullOrWhiteSpace(companyName))
                return _customers;

            return _customers.Where(c => c.CompanyName.Contains(companyName, StringComparison.OrdinalIgnoreCase)).ToList();
        }
    }
}
