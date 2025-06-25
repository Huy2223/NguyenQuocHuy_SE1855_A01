using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface ICustomerService
    {
        IEnumerable<Customers> GetAllCustomers();
        Customers GetCustomerByID(int customerID);
        void AddCustomer(Customers customer);
        void UpdateCustomer(Customers customer);
        void DeleteCustomer(int customerID);
        
        // Search methods
        IEnumerable<Customers> SearchCustomersByName(string name);
        IEnumerable<Customers> SearchCustomersByCompany(string companyName);
    }
}
