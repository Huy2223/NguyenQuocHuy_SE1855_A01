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

        public IEnumerable<OrderDetail> GetAllOrderDetails()
        {
            return orderDetailDAO.GetAllOrderDetails();
        }

        public OrderDetail GetOrderDetail(int orderID, int productID)
        {
            return orderDetailDAO.GetOrderDetail(orderID, productID);
        }

        public IEnumerable<OrderDetail> GetOrderDetailsByOrderID(int orderID)
        {
            return orderDetailDAO.GetOrderDetailsByOrderID(orderID);
        }

        public IEnumerable<OrderDetail> GetOrderDetailsByProductID(int productID)
        {
            return orderDetailDAO.GetOrderDetailsByProductID(productID);
        }

        public void AddOrderDetail(OrderDetail orderDetail)
        {
            orderDetailDAO.AddOrderDetail(orderDetail);
        }

        public void UpdateOrderDetail(OrderDetail orderDetail)
        {
            orderDetailDAO.UpdateOrderDetail(orderDetail);
        }

        public void DeleteOrderDetail(int orderID, int productID)
        {
            orderDetailDAO.DeleteOrderDetail(orderID, productID);
        }
    }
}