using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class OrderDAO
    {
        // Mock data - in a real application, this would be stored in a database
        private static List<Orders> _orders = new List<Orders>
        {
            new Orders
            {
                OrderID = 1,
                CustomerID = 1,
                EmployeeID = 1,
                OrderDate = new DateTime(2023, 6, 1)
            },
            new Orders
            {
                OrderID = 2,
                CustomerID = 2,
                EmployeeID = 2,
                OrderDate = new DateTime(2023, 6, 5)
            },
            new Orders
            {
                OrderID = 3,
                CustomerID = 1,
                EmployeeID = 3,
                OrderDate = new DateTime(2023, 6, 10)
            },
            new Orders
            {
                OrderID = 4,
                CustomerID = 3,
                EmployeeID = 1,
                OrderDate = new DateTime(2023, 6, 15)
            },
            // Orders for the customer with phone 1234567890 (CustomerID = 4)
            new Orders
            {
                OrderID = 5,
                CustomerID = 4, // FPTU Technology
                EmployeeID = 1,
                OrderDate = DateTime.Now.AddDays(-2)
            },
            new Orders
            {
                OrderID = 6,
                CustomerID = 4, // FPTU Technology
                EmployeeID = 2,
                OrderDate = DateTime.Now.AddDays(-7)
            }
        };

        private static int _nextId = 7;

        // Get all orders
        public List<Orders> GetAllOrders()
        {
            return _orders;
        }

        // Get order by ID
        public Orders GetOrderByID(int orderID)
        {
            return _orders.FirstOrDefault(o => o.OrderID == orderID);
        }

        // Add a new order
        public void AddOrder(Orders order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            // Validate required fields
            if (order.CustomerID <= 0)
                throw new ArgumentException("Customer ID must be greater than zero");
            if (order.EmployeeID <= 0)
                throw new ArgumentException("Employee ID must be greater than zero");
            if (order.OrderDate == default)
                throw new ArgumentException("Order date is required");

            // Set new ID
            order.OrderID = _nextId++;
            _orders.Add(order);
        }

        // Update an existing order
        public void UpdateOrder(Orders order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            // Validate required fields
            if (order.CustomerID <= 0)
                throw new ArgumentException("Customer ID must be greater than zero");
            if (order.EmployeeID <= 0)
                throw new ArgumentException("Employee ID must be greater than zero");
            if (order.OrderDate == default)
                throw new ArgumentException("Order date is required");

            // Find existing order
            var existingOrder = _orders.FirstOrDefault(o => o.OrderID == order.OrderID);
            if (existingOrder == null)
                throw new ArgumentException($"Order with ID {order.OrderID} not found");

            // Update properties
            existingOrder.CustomerID = order.CustomerID;
            existingOrder.EmployeeID = order.EmployeeID;
            existingOrder.OrderDate = order.OrderDate;
        }

        // Delete an order
        public void DeleteOrder(int orderID)
        {
            var order = _orders.FirstOrDefault(o => o.OrderID == orderID);
            if (order == null)
                throw new ArgumentException($"Order with ID {orderID} not found");

            _orders.Remove(order);
        }

        // Get orders by customer ID
        public List<Orders> GetOrdersByCustomerID(int customerID)
        {
            return _orders.Where(o => o.CustomerID == customerID).ToList();
        }

        // Get orders by employee ID
        public List<Orders> GetOrdersByEmployeeID(int employeeID)
        {
            return _orders.Where(o => o.EmployeeID == employeeID).ToList();
        }

        // Get orders within a date range
        public List<Orders> GetOrdersByDateRange(DateTime startDate, DateTime endDate)
        {
            // Ensure the end date is inclusive (including the entire day)
            var adjustedEndDate = endDate.Date.AddDays(1).AddSeconds(-1);
            
            return _orders.Where(o => o.OrderDate >= startDate && o.OrderDate <= adjustedEndDate).ToList();
        }
    }
}
