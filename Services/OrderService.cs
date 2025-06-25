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

        public IEnumerable<Orders> GetAllOrders()
        {
            return _orderRepository.GetAllOrders();
        }

        public Orders GetOrderByID(int orderID)
        {
            return _orderRepository.GetOrderByID(orderID);
        }

        public void AddOrder(Orders order)
        {
            _orderRepository.AddOrder(order);
        }

        public void UpdateOrder(Orders order)
        {
            _orderRepository.UpdateOrder(order);
        }

        public void DeleteOrder(int orderID)
        {
            _orderRepository.DeleteOrder(orderID);
        }

        public IEnumerable<Orders> GetOrdersByCustomerID(int customerID)
        {
            return _orderRepository.GetOrdersByCustomerID(customerID);
        }

        public IEnumerable<Orders> GetOrdersByEmployeeID(int employeeID)
        {
            return _orderRepository.GetOrdersByEmployeeID(employeeID);
        }

        public IEnumerable<Orders> GetOrdersByDateRange(DateTime startDate, DateTime endDate)
        {
            return _orderRepository.GetOrdersByDate(startDate, endDate);
        }
    }
}
