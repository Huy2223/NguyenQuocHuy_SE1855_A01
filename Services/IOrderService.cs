﻿using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IOrderService
    {
        IEnumerable<Order> GetAllOrders();
        Order GetOrderByID(int orderID);
        void AddOrder(Order order);
        void UpdateOrder(Order order);
        void DeleteOrder(int orderID);
        IEnumerable<Order> GetOrdersByCustomerID(int customerID);
        IEnumerable<Order> GetOrdersByEmployeeID(int employeeID);
        IEnumerable<Order> GetOrdersByDateRange(DateTime startDate, DateTime endDate);
    }
}
