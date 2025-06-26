using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
                Phone = "5551234567"
            },
            new Customers
            {
                CustomerID = 2,
                CompanyName = "XYZ Corporation",
                ContactName = "Jane Doe",
                ContactTitle = "CEO",
                Address = "456 Broadway, Boston",
                Phone = "5555678901"
            },
            new Customers
            {
                CustomerID = 3,
                CompanyName = "Global Enterprises",
                ContactName = "Michael Johnson",
                ContactTitle = "Procurement Officer",
                Address = "789 Fifth Avenue, Chicago",
                Phone = "5559012345"
            },
            // Special customer with phone 1234567890 as requested
            new Customers
            {
                CustomerID = 4,
                CompanyName = "FPTU Technology",
                ContactName = "Nguyen Quoc Huy",
                ContactTitle = "CEO",
                Address = "FPTU Campus, District 9, Ho Chi Minh City, Vietnam",
                Phone = "1234567890"
            }
        };

        private static int _nextId = 5;

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
            if (string.IsNullOrWhiteSpace(customer.Phone))
                throw new ArgumentException("Phone number is required");

            // Validate phone number format (10 digits)
            if (!IsValidPhoneNumber(customer.Phone))
                throw new ArgumentException("Phone number must contain exactly 10 digits");

            // Check if phone already exists
            if (_customers.Any(c => c.Phone == customer.Phone))
                throw new ArgumentException("Phone number is already registered");

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
            if (string.IsNullOrWhiteSpace(customer.Phone))
                throw new ArgumentException("Phone number is required");

            // Validate phone number format (10 digits)
            if (!IsValidPhoneNumber(customer.Phone))
                throw new ArgumentException("Phone number must contain exactly 10 digits");

            // Find existing customer
            var existingCustomer = _customers.FirstOrDefault(c => c.CustomerID == customer.CustomerID);
            if (existingCustomer == null)
                throw new ArgumentException($"Customer with ID {customer.CustomerID} not found");

            // Check if updated phone already exists (excluding current customer)
            if (_customers.Any(c => c.CustomerID != customer.CustomerID && c.Phone == customer.Phone))
                throw new ArgumentException("Phone number is already registered by another customer");

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
        
        // Authenticate customer by phone number
        public Customers AuthenticateByPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return null;
                
            // Validate phone number format (10 digits)
            if (!IsValidPhoneNumber(phone))
                return null;

            return _customers.FirstOrDefault(c => 
                c.Phone == phone);
        }
        
        // Get customer by phone
        public Customers GetCustomerByPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return null;
                
            // Validate phone number format (10 digits)
            if (!IsValidPhoneNumber(phone))
                return null;
                
            return _customers.FirstOrDefault(c => 
                c.Phone == phone);
        }
        
        // Validate phone number format (10 digits only)
        private bool IsValidPhoneNumber(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return false;
                
            // Phone must be exactly 10 digits
            return Regex.IsMatch(phone, @"^\d{10}$");
        }
    }
}
