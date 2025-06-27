using System;

namespace NguyenQuocHuyWPF.Models
{
    // ViewModel for order items - shared between CreateNewOrder and EditOrder
    public class OrderItemViewModel
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; } = "";
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public float Discount { get; set; }
        public decimal Total { get; private set; }
        
        public OrderItemViewModel()
        {
            UpdateTotal();
        }
        
        public void UpdateTotal()
        {
            Total = UnitPrice * Quantity * (1 - (decimal)Discount);
        }
    }
}