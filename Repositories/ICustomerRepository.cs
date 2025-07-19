using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface ICustomerRepository
    {
        IEnumerable<Customer> GetAllCustomers();
        Customer GetCustomerByID(int customerID);
        void AddCustomer(Customer customer);
        void UpdateCustomer(Customer customer);
        void DeleteCustomer(int customerID);
        IEnumerable<Customer> SearchCustomersByName(string name);
        IEnumerable<Customer> SearchCustomersByCompany(string companyName);
        Customer AuthenticateByPhone(string phone);
        Customer GetCustomerByPhone(string phone);
    }
}
