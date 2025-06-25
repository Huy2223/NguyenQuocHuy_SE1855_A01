using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IOrderService
    {
        IEnumerable<Orders> GetAllOrders();
        Orders GetOrderByID(int orderID);
        void AddOrder(Orders order);
        void UpdateOrder(Orders order);
        void DeleteOrder(int orderID);
        
        // Search methods
        IEnumerable<Orders> GetOrdersByCustomerID(int customerID);
        IEnumerable<Orders> GetOrdersByEmployeeID(int employeeID);
        IEnumerable<Orders> GetOrdersByDateRange(DateTime startDate, DateTime endDate);
    }
}
