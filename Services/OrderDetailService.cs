using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject;
using Repositories;

namespace Services
{
    public class OrderDetailService : IOrderDetailService
    {
        private readonly IOrderDetailRepository _orderDetailRepository;

        public OrderDetailService()
        {
            _orderDetailRepository = new OrderDetailRepository();
        }

        public IEnumerable<OrderDetail> GetAllOrderDetails()
        {
            return _orderDetailRepository.GetAllOrderDetails();
        }

        public OrderDetail GetOrderDetail(int orderID, int productID)
        {
            return _orderDetailRepository.GetOrderDetail(orderID, productID);
        }

        public IEnumerable<OrderDetail> GetOrderDetailsByOrderID(int orderID)
        {
            return _orderDetailRepository.GetOrderDetailsByOrderID(orderID);
        }

        public IEnumerable<OrderDetail> GetOrderDetailsByProductID(int productID)
        {
            return _orderDetailRepository.GetOrderDetailsByProductID(productID);
        }

        public void AddOrderDetail(OrderDetail orderDetail)
        {
            _orderDetailRepository.AddOrderDetail(orderDetail);
        }

        public void UpdateOrderDetail(OrderDetail orderDetail)
        {
            _orderDetailRepository.UpdateOrderDetail(orderDetail);
        }

        public void DeleteOrderDetail(int orderID, int productID)
        {
            _orderDetailRepository.DeleteOrderDetail(orderID, productID);
        }
    }
}
