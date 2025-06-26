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

        public IEnumerable<Customers> GetAllCustomers()
        {
            return customerDAO.GetAllCustomers();
        }

        public Customers GetCustomerByID(int customerID)
        {
            return customerDAO.GetCustomerByID(customerID);
        }

        public void AddCustomer(Customers customer)
        {
            customerDAO.AddCustomer(customer);
        }

        public void UpdateCustomer(Customers customer)
        {
            customerDAO.UpdateCustomer(customer);
        }

        public void DeleteCustomer(int customerID)
        {
            customerDAO.DeleteCustomer(customerID);
        }

        public IEnumerable<Customers> SearchCustomersByName(string name)
        {
            return customerDAO.SearchCustomersByName(name);
        }

        public IEnumerable<Customers> SearchCustomersByCompany(string companyName)
        {
            return customerDAO.SearchCustomersByCompany(companyName);
        }

        public Customers AuthenticateByPhone(string phone)
        {
            return customerDAO.AuthenticateByPhone(phone);
        }

        public Customers GetCustomerByPhone(string phone)
        {
            return customerDAO.GetCustomerByPhone(phone);
        }
    }
}
