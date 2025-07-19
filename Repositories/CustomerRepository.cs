using BusinessObject;
using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private CustomerDAO customerDAO;

        public CustomerRepository()
        {
            customerDAO = new CustomerDAO();
        }

        public IEnumerable<Customer> GetAllCustomers()
        {
            return customerDAO.GetAllCustomers();
        }

        public Customer GetCustomerByID(int customerID)
        {
            return customerDAO.GetCustomerByID(customerID);
        }

        public void AddCustomer(Customer customer)
        {
            customerDAO.AddCustomer(customer);
        }

        public void UpdateCustomer(Customer customer)
        {
            customerDAO.UpdateCustomer(customer);
        }

        public void DeleteCustomer(int customerID)
        {
            customerDAO.DeleteCustomer(customerID);
        }

        public IEnumerable<Customer> SearchCustomersByName(string name)
        {
            return customerDAO.SearchCustomersByName(name);
        }

        public IEnumerable<Customer> SearchCustomersByCompany(string companyName)
        {
            return customerDAO.SearchCustomersByCompany(companyName);
        }

        public Customer AuthenticateByPhone(string phone)
        {
            return customerDAO.AuthenticateByPhone(phone);
        }

        public Customer GetCustomerByPhone(string phone)
        {
            return customerDAO.GetCustomerByPhone(phone);
        }
    }
}
