using BusinessObject;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerService()
        {
            _customerRepository = new CustomerRepository();
        }

        public IEnumerable<Customer> GetAllCustomers()
        {
            return _customerRepository.GetAllCustomers();
        }

        public Customer GetCustomerByID(int customerID)
        {
            return _customerRepository.GetCustomerByID(customerID);
        }

        public void AddCustomer(Customer customer)
        {
            _customerRepository.AddCustomer(customer);
        }

        public void UpdateCustomer(Customer customer)
        {
            _customerRepository.UpdateCustomer(customer);
        }

        public void DeleteCustomer(int customerID)
        {
            _customerRepository.DeleteCustomer(customerID);
        }

        public IEnumerable<Customer> SearchCustomersByName(string name)
        {
            return _customerRepository.SearchCustomersByName(name);
        }

        public IEnumerable<Customer> SearchCustomersByCompany(string companyName)
        {
            return _customerRepository.SearchCustomersByCompany(companyName);
        }

        public Customer AuthenticateByPhone(string phone)
        {
            return _customerRepository.AuthenticateByPhone(phone);
        }

        public Customer GetCustomerByPhone(string phone)
        {
            return _customerRepository.GetCustomerByPhone(phone);
        }
    }
}
