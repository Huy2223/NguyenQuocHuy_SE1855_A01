using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BusinessObject;
using Services;
using NguyenQuocHuy_SE193304_ASM01;
using NguyenQuocHuyWPF.Customer;
using CustomerEntity = BusinessObject.Customer;

namespace NguyenQuocHuyWPF
{
    /// <summary>
    /// Interaction logic for CustomerWindow.xaml
    /// </summary>
    public partial class CustomerWindow : Window
    {
        private readonly CustomerEntity _currentCustomer;
        private readonly IOrderService _orderService;
        private readonly IOrderDetailService _orderDetailService;
        private readonly ICustomerService _customerService;

        public CustomerWindow(CustomerEntity customer)
        {
            InitializeComponent();
            
            // Store the current customer
            _currentCustomer = customer ?? throw new ArgumentNullException(nameof(customer));
            
            // Initialize services
            _orderService = new OrderService();
            _orderDetailService = new OrderDetailService();
            _customerService = new CustomerService();
            
            // Load customer information
            LoadCustomerInfo();
            
            // Load order history
            LoadOrderHistory();
        }
        
        // Default constructor for design-time support
        public CustomerWindow()
        {
            InitializeComponent();
        }

        private void LoadCustomerInfo()
        {
            if (_currentCustomer == null)
                return;
                
            // Set header information
            CustomerNameText.Text = _currentCustomer.ContactName;
            CompanyNameText.Text = _currentCustomer.CompanyName;
            
            // Set profile information
            txtContactName.Text = _currentCustomer.ContactName;
            txtContactTitle.Text = _currentCustomer.ContactTitle;
            txtCompanyName.Text = _currentCustomer.CompanyName;
            
            // Format phone number for display (add dashes)
            string phone = _currentCustomer.Phone;
            if (phone.Length == 10)
            {
                txtPhone.Text = $"{phone.Substring(0, 3)}-{phone.Substring(3, 3)}-{phone.Substring(6, 4)}";
            }
            else
            {
                txtPhone.Text = phone;
            }
            
            txtAddress.Text = _currentCustomer.Address;
            
            // Set initials for avatar
            string[] nameParts = _currentCustomer.ContactName.Split(' ');
            if (nameParts.Length >= 2)
            {
                CustomerInitials.Text = $"{nameParts[0][0]}{nameParts[1][0]}".ToUpper();
            }
            else if (nameParts.Length == 1 && nameParts[0].Length > 0)
            {
                CustomerInitials.Text = nameParts[0].Substring(0, 1).ToUpper();
            }
            else
            {
                CustomerInitials.Text = "C";
            }
        }

        private void LoadOrderHistory()
        {
            try
            {
                // Get orders for this customer
                var orders = _orderService.GetOrdersByCustomerID(_currentCustomer.CustomerId);
                
                // Create view model for displaying orders with additional info
                var orderViewModels = orders.Select(o => new CustomerOrderViewModel
                {
                    OrderId = o.OrderId,
                    OrderDate = o.OrderDate,
                    TotalAmount = CalculateOrderTotal(o.OrderId),
                    ItemCount = GetOrderItemCount(o.OrderId),
                    Status = "Completed" // Default status for simplicity
                }).ToList();
                
                // Sort by date descending (newest first)
                orderViewModels = orderViewModels.OrderByDescending(o => o.OrderDate).ToList();
                
                dgOrderHistory.ItemsSource = orderViewModels;
                
                // Update status
                txtOrderStatus.Text = $"Found {orderViewModels.Count} order(s)";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading order history: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private decimal CalculateOrderTotal(int orderId)
        {
            try
            {
                var orderDetails = _orderDetailService.GetOrderDetailsByOrderID(orderId);
                return orderDetails.Sum(od => od.UnitPrice * od.Quantity * (1 - (decimal)od.Discount));
            }
            catch
            {
                return 0;
            }
        }

        private int GetOrderItemCount(int orderId)
        {
            try
            {
                var orderDetails = _orderDetailService.GetOrderDetailsByOrderID(orderId);
                return orderDetails.Sum(od => od.Quantity);
            }
            catch
            {
                return 0;
            }
        }

        private void BtnEditProfile_Click(object sender, RoutedEventArgs e)
        {
            // Create the EditProfile dialog with a callback to refresh customer data
            EditProfile editProfileWindow = new EditProfile(
                _currentCustomer, 
                false, 
                updatedCustomer => {
                    // Store the updated customer
                    if (updatedCustomer != null)
                    {
                        // Update the current customer reference with the updated data
                        _currentCustomer.ContactName = updatedCustomer.ContactName;
                        _currentCustomer.ContactTitle = updatedCustomer.ContactTitle;
                        _currentCustomer.CompanyName = updatedCustomer.CompanyName;
                        _currentCustomer.Phone = updatedCustomer.Phone;
                        _currentCustomer.Address = updatedCustomer.Address;
                        
                        // Refresh the UI with updated customer data
                        LoadCustomerInfo();
                    }
                });
            
            // Show the dialog - this will keep CustomerWindow visible
            editProfileWindow.ShowDialog();
        }

        private void BtnViewOrderDetails_Click(object sender, RoutedEventArgs e)
        {
            var order = (sender as Button)?.DataContext as CustomerOrderViewModel;
            if (order != null)
            {
                MessageBox.Show($"Order details functionality will be implemented in the future.\n\nOrder ID: {order.OrderId}\nDate: {order.OrderDate:d}\nTotal: {order.TotalAmount:C}", 
                    "Order Details", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void DgOrderHistory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Optional: Handle order selection
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to logout?", "Confirmation", 
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                // Open login window and close this window
                Login loginWindow = new Login();
                loginWindow.Show();
                this.Close();
            }
        }
    }
    
    // ViewModel for order display in customer window
    public class CustomerOrderViewModel
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public int ItemCount { get; set; }
        public string Status { get; set; } = "Completed";
    }
}

