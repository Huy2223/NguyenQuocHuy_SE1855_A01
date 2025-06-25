using BusinessObject;
using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class OrderDetailRepository : IOrderDetailRepository
    {
        private DataAccessLayer.OrderDetailDAO orderDetailDAO;

        public OrderDetailRepository()
        {
            orderDetailDAO = new DataAccessLayer.OrderDetailDAO();
        }

        public IEnumerable<OrderDetails> GetAllOrderDetails()
        {
            return orderDetailDAO.GetAllOrderDetails();
        }

        public OrderDetails GetOrderDetail(int orderID, int productID)
        {
            return orderDetailDAO.GetOrderDetail(orderID, productID);
        }

        public IEnumerable<OrderDetails> GetOrderDetailsByOrderID(int orderID)
        {
            return orderDetailDAO.GetOrderDetailsByOrderID(orderID);
        }

        public IEnumerable<OrderDetails> GetOrderDetailsByProductID(int productID)
        {
            return orderDetailDAO.GetOrderDetailsByProductID(productID);
        }

        public void AddOrderDetail(OrderDetails orderDetail)
        {
            orderDetailDAO.AddOrderDetail(orderDetail);
        }

        public void UpdateOrderDetail(OrderDetails orderDetail)
        {
            orderDetailDAO.UpdateOrderDetail(orderDetail);
        }

        public void DeleteOrderDetail(int orderID, int productID)
        {
            orderDetailDAO.DeleteOrderDetail(orderID, productID);
        }
    }
}