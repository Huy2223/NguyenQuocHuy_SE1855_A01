using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IOrderDetailService
    {
        IEnumerable<OrderDetail> GetAllOrderDetails();
        OrderDetail GetOrderDetail(int orderID, int productID);
        IEnumerable<OrderDetail> GetOrderDetailsByOrderID(int orderID);
        IEnumerable<OrderDetail> GetOrderDetailsByProductID(int productID);
        void AddOrderDetail(OrderDetail orderDetail);
        void UpdateOrderDetail(OrderDetail orderDetail);
        void DeleteOrderDetail(int orderID, int productID);
    }
}
