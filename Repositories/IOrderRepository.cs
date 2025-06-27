using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface IOrderRepository
    {
        IEnumerable<Orders> GetAllOrders();
        Orders GetOrderByID(int orderID);
        void AddOrder(Orders order);
        void UpdateOrder(Orders order);
        void DeleteOrder(int orderID);
        IEnumerable<Orders> GetOrdersByCustomerID(int customerID);
        IEnumerable<Orders> GetOrdersByEmployeeID(int employeeID);
        IEnumerable<Orders> GetOrdersByDate(DateTime startDate, DateTime endDate);
    }
}
