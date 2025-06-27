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
        IEnumerable<Customers> GetAllCustomers();
        Customers GetCustomerByID(int customerID);
        void AddCustomer(Customers customer);
        void UpdateCustomer(Customers customer);
        void DeleteCustomer(int customerID);
        IEnumerable<Customers> SearchCustomersByName(string name);
        IEnumerable<Customers> SearchCustomersByCompany(string companyName);
        Customers AuthenticateByPhone(string phone);
        Customers GetCustomerByPhone(string phone);
    }
}
