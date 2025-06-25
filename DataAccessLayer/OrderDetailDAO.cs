using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class OrderDetailDAO
    {
        // Mock data - in a real application, this would be stored in a database
        private static List<OrderDetails> _orderDetails = new List<OrderDetails>
        {
            new OrderDetails
            {
                OrderID = 1,
                ProductID = 1,
                UnitPrice = 1200.00m,
                Quantity = 2,
                Discount = 0.05f
            },
            new OrderDetails
            {
                OrderID = 1,
                ProductID = 3,
                UnitPrice = 150.00m,
                Quantity = 1,
                Discount = 0.0f
            },
            new OrderDetails
            {
                OrderID = 2,
                ProductID = 2,
                UnitPrice = 800.00m,
                Quantity = 1,
                Discount = 0.0f
            },
            new OrderDetails
            {
                OrderID = 3,
                ProductID = 5,
                UnitPrice = 80.00m,
                Quantity = 3,
                Discount = 0.1f
            }
        };

        // Get all order details
        public List<OrderDetails> GetAllOrderDetails()
        {
            return _orderDetails;
        }

        // Get order detail by OrderID and ProductID
        public OrderDetails GetOrderDetail(int orderID, int productID)
        {
            return _orderDetails.FirstOrDefault(o => o.OrderID == orderID && o.ProductID == productID);
        }

        // Get order details by OrderID
        public List<OrderDetails> GetOrderDetailsByOrderID(int orderID)
        {
            return _orderDetails.Where(o => o.OrderID == orderID).ToList();
        }

        // Get order details by ProductID
        public List<OrderDetails> GetOrderDetailsByProductID(int productID)
        {
            return _orderDetails.Where(o => o.ProductID == productID).ToList();
        }

        // Add a new order detail
        public void AddOrderDetail(OrderDetails orderDetail)
        {
            if (orderDetail == null)
                throw new ArgumentNullException(nameof(orderDetail));

            // Validate required fields
            if (orderDetail.OrderID <= 0)
                throw new ArgumentException("Order ID must be greater than zero");
            if (orderDetail.ProductID <= 0)
                throw new ArgumentException("Product ID must be greater than zero");
            if (orderDetail.UnitPrice < 0)
                throw new ArgumentException("Unit price cannot be negative");
            if (orderDetail.Quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero");
            if (orderDetail.Discount < 0 || orderDetail.Discount > 1)
                throw new ArgumentException("Discount must be between 0 and 1");

            // Check if the order detail already exists
            var existingOrderDetail = _orderDetails.FirstOrDefault(o => 
                o.OrderID == orderDetail.OrderID && o.ProductID == orderDetail.ProductID);
            
            if (existingOrderDetail != null)
                throw new ArgumentException($"Order detail with Order ID {orderDetail.OrderID} and Product ID {orderDetail.ProductID} already exists");

            _orderDetails.Add(orderDetail);
        }

        // Update an existing order detail
        public void UpdateOrderDetail(OrderDetails orderDetail)
        {
            if (orderDetail == null)
                throw new ArgumentNullException(nameof(orderDetail));

            // Validate required fields
            if (orderDetail.OrderID <= 0)
                throw new ArgumentException("Order ID must be greater than zero");
            if (orderDetail.ProductID <= 0)
                throw new ArgumentException("Product ID must be greater than zero");
            if (orderDetail.UnitPrice < 0)
                throw new ArgumentException("Unit price cannot be negative");
            if (orderDetail.Quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero");
            if (orderDetail.Discount < 0 || orderDetail.Discount > 1)
                throw new ArgumentException("Discount must be between 0 and 1");

            // Find existing order detail
            var existingOrderDetail = _orderDetails.FirstOrDefault(o => 
                o.OrderID == orderDetail.OrderID && o.ProductID == orderDetail.ProductID);
            
            if (existingOrderDetail == null)
                throw new ArgumentException($"Order detail with Order ID {orderDetail.OrderID} and Product ID {orderDetail.ProductID} not found");

            // Update properties
            existingOrderDetail.UnitPrice = orderDetail.UnitPrice;
            existingOrderDetail.Quantity = orderDetail.Quantity;
            existingOrderDetail.Discount = orderDetail.Discount;
        }

        // Delete an order detail
        public void DeleteOrderDetail(int orderID, int productID)
        {
            var orderDetail = _orderDetails.FirstOrDefault(o => 
                o.OrderID == orderID && o.ProductID == productID);
            
            if (orderDetail == null)
                throw new ArgumentException($"Order detail with Order ID {orderID} and Product ID {productID} not found");

            _orderDetails.Remove(orderDetail);
        }
    }
}
