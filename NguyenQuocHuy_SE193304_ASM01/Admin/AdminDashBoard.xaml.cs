using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using NguyenQuocHuyWPF.Admin;

namespace NguyenQuocHuyWPF
{
    /// <summary>
    /// Interaction logic for Employee Dashboard (formerly AdminDashBoard.xaml)
    /// All employees have full access to system functionality
    /// </summary>
    public partial class AdminDashBoard : Window
    {
        private readonly ICustomerService? _customerService;
        private readonly IProductService? _productService;
        private readonly IOrderService? _orderService;
        private readonly IOrderDetailService? _orderDetailService;
        
        private Employee? _currentUser;

        public AdminDashBoard(Employee loggedInEmployee)
        {
            InitializeComponent();
            
            // Store the current user
            _currentUser = loggedInEmployee ?? throw new ArgumentNullException(nameof(loggedInEmployee));
            
            // Store the current user in application properties for global access
            Application.Current.Properties["CurrentUser"] = _currentUser;
            
            // Initialize services
            _customerService = new CustomerService();
            _productService = new ProductService();
            _orderService = new OrderService();
            _orderDetailService = new OrderDetailService();
            
            // Set the employee name in the UI (formerly called AdminNameText)
            if (AdminNameText != null)
            {
                AdminNameText.Text = _currentUser.Name;
            }
        }

        public AdminDashBoard() // Default constructor for design-time support
        {
            InitializeComponent();
            
            // Try to get current user from application properties
            if (Application.Current.Properties.Contains("CurrentUser") && 
                Application.Current.Properties["CurrentUser"] is Employee employee)
            {
                _currentUser = employee;
                
                // Initialize services
                _customerService = new CustomerService();
                _productService = new ProductService();
                _orderService = new OrderService();
                _orderDetailService = new OrderDetailService();
                
                // Set the employee name in the UI if possible
                if (AdminNameText != null && _currentUser != null)
                {
                    AdminNameText.Text = _currentUser.Name;
                }
            }
        }

        #region Navigation Methods - All employees can access these features
        
        private void BtnCustomers_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to ManageCustomer window
            ManageCustomer manageCustomerWindow = new ManageCustomer();
            manageCustomerWindow.Show();
            this.Hide(); // Hide the current window instead of closing it
        }

        private void BtnProducts_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to ManageProducts window
            ManageProducts manageProductsWindow = new ManageProducts();
            manageProductsWindow.Show();
            this.Hide(); // Hide the current window instead of closing it
        }

        private void BtnOrders_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to ManageOrder window
            ManageOrder manageOrderWindow = new ManageOrder();
            manageOrderWindow.Show();
            this.Hide(); // Hide the current window instead of closing it
        }

        private void BtnReports_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to SaleReport window
            SaleReport saleReportWindow = new SaleReport();
            saleReportWindow.Show();
            this.Hide(); // Hide the current window instead of closing it
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to logout?", "Confirmation", 
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                // Remove current user from application properties
                if (Application.Current.Properties.Contains("CurrentUser"))
                {
                    Application.Current.Properties.Remove("CurrentUser");
                }
                
                // Open login window and close this window
                NguyenQuocHuy_SE193304_ASM01.Login loginWindow = new NguyenQuocHuy_SE193304_ASM01.Login();
                loginWindow.Show();
                this.Close();
            }
        }
        
        #endregion
    }

    // ViewModel for BusinessObject.Order display
    public class OrderViewModel
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; } = "";
        public DateTime OrderDate { get; set; }
        public string EmployeeName { get; set; } = "";
        public decimal TotalAmount { get; set; }
        public int ItemCount { get; set; }
    }
}
