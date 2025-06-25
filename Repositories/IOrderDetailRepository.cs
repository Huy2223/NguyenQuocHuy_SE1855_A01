using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface IOrderDetailRepository
    {
        IEnumerable<OrderDetails> GetAllOrderDetails();
        OrderDetails GetOrderDetail(int orderID, int productID);
        IEnumerable<OrderDetails> GetOrderDetailsByOrderID(int orderID);
        IEnumerable<OrderDetails> GetOrderDetailsByProductID(int productID);
        void AddOrderDetail(OrderDetails orderDetail);
        void UpdateOrderDetail(OrderDetails orderDetail);
        void DeleteOrderDetail(int orderID, int productID);
    }
}
