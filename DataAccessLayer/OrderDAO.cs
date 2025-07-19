using BusinessObject;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class OrderDAO
    {
        // Get all orders
        public List<Order> GetAllOrders()
        {
            using var context = new LucySalesDataContext();
            return context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Employee)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .ToList();
        }

        // Get order by ID
        public Order GetOrderByID(int orderID)
        {
            using var context = new LucySalesDataContext();
            return context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Employee)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .FirstOrDefault(o => o.OrderId == orderID);
        }

        // Add a new order
        public void AddOrder(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            // Validate required fields
            if (order.CustomerId <= 0)
                throw new ArgumentException("Customer ID must be greater than zero");
            if (order.EmployeeId <= 0)
                throw new ArgumentException("Employee ID must be greater than zero");
            if (order.OrderDate == default)
                throw new ArgumentException("Order date is required");

            using var context = new LucySalesDataContext();
            context.Orders.Add(order);
            context.SaveChanges();
        }

        // Update an existing order
        public void UpdateOrder(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            // Validate required fields
            if (order.CustomerId <= 0)
                throw new ArgumentException("Customer ID must be greater than zero");
            if (order.EmployeeId <= 0)
                throw new ArgumentException("Employee ID must be greater than zero");
            if (order.OrderDate == default)
                throw new ArgumentException("Order date is required");

            using var context = new LucySalesDataContext();
            
            // Find existing order
            var existingOrder = context.Orders.FirstOrDefault(o => o.OrderId == order.OrderId);
            if (existingOrder == null)
                throw new ArgumentException($"Order with ID {order.OrderId} not found");

            // Update properties
            existingOrder.CustomerId = order.CustomerId;
            existingOrder.EmployeeId = order.EmployeeId;
            existingOrder.OrderDate = order.OrderDate;
            
            context.SaveChanges();
        }

        // Delete an order
        public void DeleteOrder(int orderID)
        {
            using var context = new LucySalesDataContext();
            var order = context.Orders.FirstOrDefault(o => o.OrderId == orderID);
            if (order == null)
                throw new ArgumentException($"Order with ID {orderID} not found");

            context.Orders.Remove(order);
            context.SaveChanges();
        }

        // Get orders by customer ID
        public List<Order> GetOrdersByCustomerID(int customerID)
        {
            using var context = new LucySalesDataContext();
            return context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Employee)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .Where(o => o.CustomerId == customerID).ToList();
        }

        // Get orders by employee ID
        public List<Order> GetOrdersByEmployeeID(int employeeID)
        {
            using var context = new LucySalesDataContext();
            return context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Employee)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .Where(o => o.EmployeeId == employeeID).ToList();
        }

        // Get orders within a date range
        public List<Order> GetOrdersByDateRange(DateTime startDate, DateTime endDate)
        {
            using var context = new LucySalesDataContext();
            // Ensure the end date is inclusive (including the entire day)
            var adjustedEndDate = endDate.Date.AddDays(1).AddSeconds(-1);
            
            return context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Employee)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= adjustedEndDate).ToList();
        }
    }
}
