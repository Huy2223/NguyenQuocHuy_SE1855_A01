using BusinessObject;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class OrderDetailDAO
    {
        // Get all order details
        public List<OrderDetail> GetAllOrderDetails()
        {
            using var context = new LucySalesDataContext();
            return context.OrderDetails
                .Include(od => od.Order)
                .Include(od => od.Product)
                .ToList();
        }

        // Get order detail by OrderID and ProductID
        public OrderDetail GetOrderDetail(int orderID, int productID)
        {
            using var context = new LucySalesDataContext();
            return context.OrderDetails
                .Include(od => od.Order)
                .Include(od => od.Product)
                .FirstOrDefault(o => o.OrderId == orderID && o.ProductId == productID);
        }

        // Get order details by OrderID
        public List<OrderDetail> GetOrderDetailsByOrderID(int orderID)
        {
            using var context = new LucySalesDataContext();
            return context.OrderDetails
                .Include(od => od.Order)
                .Include(od => od.Product)
                .Where(o => o.OrderId == orderID).ToList();
        }

        // Get order details by ProductID
        public List<OrderDetail> GetOrderDetailsByProductID(int productID)
        {
            using var context = new LucySalesDataContext();
            return context.OrderDetails
                .Include(od => od.Order)
                .Include(od => od.Product)
                .Where(o => o.ProductId == productID).ToList();
        }

        // Add a new order detail
        public void AddOrderDetail(OrderDetail orderDetail)
        {
            if (orderDetail == null)
                throw new ArgumentNullException(nameof(orderDetail));

            // Validate required fields
            if (orderDetail.OrderId <= 0)
                throw new ArgumentException("Order ID must be greater than zero");
            if (orderDetail.ProductId <= 0)
                throw new ArgumentException("Product ID must be greater than zero");
            if (orderDetail.UnitPrice < 0)
                throw new ArgumentException("Unit price cannot be negative");
            if (orderDetail.Quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero");
            if (orderDetail.Discount < 0 || orderDetail.Discount > 1)
                throw new ArgumentException("Discount must be between 0 and 1");

            using var context = new LucySalesDataContext();
            
            // Check if the order detail already exists
            var existingOrderDetail = context.OrderDetails.FirstOrDefault(o => 
                o.OrderId == orderDetail.OrderId && o.ProductId == orderDetail.ProductId);
            
            if (existingOrderDetail != null)
                throw new ArgumentException($"Order detail with Order ID {orderDetail.OrderId} and Product ID {orderDetail.ProductId} already exists");

            context.OrderDetails.Add(orderDetail);
            context.SaveChanges();
        }

        // Update an existing order detail
        public void UpdateOrderDetail(OrderDetail orderDetail)
        {
            if (orderDetail == null)
                throw new ArgumentNullException(nameof(orderDetail));

            // Validate required fields
            if (orderDetail.OrderId <= 0)
                throw new ArgumentException("Order ID must be greater than zero");
            if (orderDetail.ProductId <= 0)
                throw new ArgumentException("Product ID must be greater than zero");
            if (orderDetail.UnitPrice < 0)
                throw new ArgumentException("Unit price cannot be negative");
            if (orderDetail.Quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero");
            if (orderDetail.Discount < 0 || orderDetail.Discount > 1)
                throw new ArgumentException("Discount must be between 0 and 1");

            using var context = new LucySalesDataContext();
            
            // Find existing order detail
            var existingOrderDetail = context.OrderDetails.FirstOrDefault(o => 
                o.OrderId == orderDetail.OrderId && o.ProductId == orderDetail.ProductId);
            
            if (existingOrderDetail == null)
                throw new ArgumentException($"Order detail with Order ID {orderDetail.OrderId} and Product ID {orderDetail.ProductId} not found");

            // Update properties
            existingOrderDetail.UnitPrice = orderDetail.UnitPrice;
            existingOrderDetail.Quantity = orderDetail.Quantity;
            existingOrderDetail.Discount = orderDetail.Discount;
            
            context.SaveChanges();
        }

        // Delete an order detail
        public void DeleteOrderDetail(int orderID, int productID)
        {
            using var context = new LucySalesDataContext();
            var orderDetail = context.OrderDetails.FirstOrDefault(o => 
                o.OrderId == orderID && o.ProductId == productID);
            
            if (orderDetail == null)
                throw new ArgumentException($"Order detail with Order ID {orderID} and Product ID {productID} not found");

            context.OrderDetails.Remove(orderDetail);
            context.SaveChanges();
        }
    }
}
