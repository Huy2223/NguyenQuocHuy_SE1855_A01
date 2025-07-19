using BusinessObject;
using Microsoft.EntityFrameworkCore;
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
        // Get all customers
        public List<Customer> GetAllCustomers()
        {
            using var context = new LucySalesDataContext();
            return context.Customers.ToList();
        }

        // Get customer by ID
        public Customer GetCustomerByID(int customerID)
        {
            using var context = new LucySalesDataContext();
            return context.Customers.FirstOrDefault(c => c.CustomerId == customerID);
        }

        // Add a new customer
        public void AddCustomer(Customer customer)
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

            // Validate phone number format (digits only)
            if (!IsValidPhoneNumber(customer.Phone))
                throw new ArgumentException("Phone number must contain only digits");

            using var context = new LucySalesDataContext();
            
            // Check if phone already exists
            if (context.Customers.Any(c => c.Phone == customer.Phone))
                throw new ArgumentException("Phone number is already registered");

            context.Customers.Add(customer);
            context.SaveChanges();
        }

        // Update an existing customer
        public void UpdateCustomer(Customer customer)
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

            // Validate phone number format (digits only)
            if (!IsValidPhoneNumber(customer.Phone))
                throw new ArgumentException("Phone number must contain only digits");

            using var context = new LucySalesDataContext();
            
            // Find existing customer
            var existingCustomer = context.Customers.FirstOrDefault(c => c.CustomerId == customer.CustomerId);
            if (existingCustomer == null)
                throw new ArgumentException($"Customer with ID {customer.CustomerId} not found");

            // Check if updated phone already exists (excluding current customer)
            if (context.Customers.Any(c => c.CustomerId != customer.CustomerId && c.Phone == customer.Phone))
                throw new ArgumentException("Phone number is already registered by another customer");

            // Update properties
            existingCustomer.CompanyName = customer.CompanyName;
            existingCustomer.ContactName = customer.ContactName;
            existingCustomer.ContactTitle = customer.ContactTitle;
            existingCustomer.Address = customer.Address;
            existingCustomer.Phone = customer.Phone;
            
            context.SaveChanges();
        }

        // Delete a customer
        public void DeleteCustomer(int customerID)
        {
            using var context = new LucySalesDataContext();
            var customer = context.Customers.FirstOrDefault(c => c.CustomerId == customerID);
            if (customer == null)
                throw new ArgumentException($"Customer with ID {customerID} not found");

            context.Customers.Remove(customer);
            context.SaveChanges();
        }

        // Search customers by name
        public List<Customer> SearchCustomersByName(string name)
        {
            using var context = new LucySalesDataContext();
            if (string.IsNullOrWhiteSpace(name))
                return context.Customers.ToList();

            return context.Customers.Where(c => c.ContactName.Contains(name)).ToList();
        }

        // Search customers by company name
        public List<Customer> SearchCustomersByCompany(string companyName)
        {
            using var context = new LucySalesDataContext();
            if (string.IsNullOrWhiteSpace(companyName))
                return context.Customers.ToList();

            return context.Customers.Where(c => c.CompanyName.Contains(companyName)).ToList();
        }
        
        // Authenticate customer by phone number
        public Customer AuthenticateByPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return null;
                
            // Validate phone number format (digits only)
            if (!IsValidPhoneNumber(phone))
                return null;

            using var context = new LucySalesDataContext();
            return context.Customers.FirstOrDefault(c => c.Phone == phone);
        }
        
        // Get customer by phone
        public Customer GetCustomerByPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return null;
                
            // Validate phone number format (digits only)
            if (!IsValidPhoneNumber(phone))
                return null;
                
            using var context = new LucySalesDataContext();
            return context.Customers.FirstOrDefault(c => c.Phone == phone);
        }
        
        // Validate phone number format (digits only, no length restriction)
        private bool IsValidPhoneNumber(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return false;
                
            // Phone must contain only digits (removed 10-digit requirement)
            return Regex.IsMatch(phone, @"^\d+$");
        }
    }
}
