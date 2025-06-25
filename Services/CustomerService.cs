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

        public IEnumerable<Customers> GetAllCustomers()
        {
            return _customerRepository.GetAllCustomers();
        }

        public Customers GetCustomerByID(int customerID)
        {
            return _customerRepository.GetCustomerByID(customerID);
        }

        public void AddCustomer(Customers customer)
        {
            _customerRepository.AddCustomer(customer);
        }

        public void UpdateCustomer(Customers customer)
        {
            _customerRepository.UpdateCustomer(customer);
        }

        public void DeleteCustomer(int customerID)
        {
            _customerRepository.DeleteCustomer(customerID);
        }

        public IEnumerable<Customers> SearchCustomersByName(string name)
        {
            return _customerRepository.SearchCustomersByName(name);
        }

        public IEnumerable<Customers> SearchCustomersByCompany(string companyName)
        {
            return _customerRepository.SearchCustomersByCompany(companyName);
        }
    }
}
