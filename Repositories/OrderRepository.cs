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

        public IEnumerable<Order> GetAllOrders()
        {
            return orderDAO.GetAllOrders();
        }

        public Order GetOrderByID(int orderID)
        {
            return orderDAO.GetOrderByID(orderID);
        }

        public void AddOrder(Order order)
        {
            orderDAO.AddOrder(order);
        }

        public void UpdateOrder(Order order)
        {
            orderDAO.UpdateOrder(order);
        }

        public void DeleteOrder(int orderID)
        {
            orderDAO.DeleteOrder(orderID);
        }

        public IEnumerable<Order> GetOrdersByCustomerID(int customerID)
        {
            return orderDAO.GetOrdersByCustomerID(customerID);
        }

        public IEnumerable<Order> GetOrdersByEmployeeID(int employeeID)
        {
            return orderDAO.GetOrdersByEmployeeID(employeeID);
        }

        public IEnumerable<Order> GetOrdersByDate(DateTime startDate, DateTime endDate)
        {
            return orderDAO.GetOrdersByDateRange(startDate, endDate);
        }
    }
}
