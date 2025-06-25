using BusinessObject;
using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private OrderDAO orderDAO;

        public OrderRepository()
        {
            orderDAO = new OrderDAO();
        }

        public IEnumerable<Orders> GetAllOrders()
        {
            return orderDAO.GetAllOrders();
        }

        public Orders GetOrderByID(int orderID)
        {
            return orderDAO.GetOrderByID(orderID);
        }

        public void AddOrder(Orders order)
        {
            orderDAO.AddOrder(order);
        }

        public void UpdateOrder(Orders order)
        {
            orderDAO.UpdateOrder(order);
        }

        public void DeleteOrder(int orderID)
        {
            orderDAO.DeleteOrder(orderID);
        }

        public IEnumerable<Orders> GetOrdersByCustomerID(int customerID)
        {
            return orderDAO.GetOrdersByCustomerID(customerID);
        }

        public IEnumerable<Orders> GetOrdersByEmployeeID(int employeeID)
        {
            return orderDAO.GetOrdersByEmployeeID(employeeID);
        }

        public IEnumerable<Orders> GetOrdersByDate(DateTime startDate, DateTime endDate)
        {
            return orderDAO.GetOrdersByDateRange(startDate, endDate);
        }
    }
}
