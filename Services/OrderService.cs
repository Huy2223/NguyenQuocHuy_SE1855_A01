using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject;
using Repositories;

namespace Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService()
        {
            _orderRepository = new OrderRepository();
        }

        public IEnumerable<Order> GetAllOrders()
        {
            return _orderRepository.GetAllOrders();
        }

        public Order GetOrderByID(int orderID)
        {
            return _orderRepository.GetOrderByID(orderID);
        }

        public void AddOrder(Order order)
        {
            _orderRepository.AddOrder(order);
        }

        public void UpdateOrder(Order order)
        {
            _orderRepository.UpdateOrder(order);
        }

        public void DeleteOrder(int orderID)
        {
            _orderRepository.DeleteOrder(orderID);
        }

        public IEnumerable<Order> GetOrdersByCustomerID(int customerID)
        {
            return _orderRepository.GetOrdersByCustomerID(customerID);
        }

        public IEnumerable<Order> GetOrdersByEmployeeID(int employeeID)
        {
            return _orderRepository.GetOrdersByEmployeeID(employeeID);
        }

        public IEnumerable<Order> GetOrdersByDateRange(DateTime startDate, DateTime endDate)
        {
            return _orderRepository.GetOrdersByDate(startDate, endDate);
        }
    }
}
