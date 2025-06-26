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
using NguyenQuocHuyWPF; // Import the namespace containing OrderViewModel

namespace NguyenQuocHuyWPF.Admin
{
    /// <summary>
    /// Interaction logic for SaleReport.xaml
    /// </summary>
    public partial class SaleReport : Window
    {
        private readonly IOrderService? _orderService;
        private readonly ICustomerService? _customerService;
        private readonly IOrderDetailService? _orderDetailService;
        private readonly IProductService? _productService;

        public SaleReport()
        {
            InitializeComponent();
            
            // Initialize services
            _orderService = new OrderService();
            _customerService = new CustomerService();
            _orderDetailService = new OrderDetailService();
            _productService = new ProductService();
            
            // Set default dates for reports after the UI is fully loaded
            this.Loaded += (s, e) => InitializeReportControls();
        }
        
        private void InitializeReportControls()
        {
            try
            {
                if (dpReportFrom != null && dpReportTo != null)
                {
                    // Set default dates for reports
                    dpReportFrom.SelectedDate = DateTime.Today.AddDays(-30); // Last 30 days
                    dpReportTo.SelectedDate = DateTime.Today;
                    
                    // Update combobox selection based on dates
                    if (cmbReportPeriod != null)
                    {
                        cmbReportPeriod.SelectedIndex = 4; // Custom period
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing report controls: {ex.Message}", 
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void CmbReportPeriod_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (dpReportFrom == null || dpReportTo == null)
                {
                    return; // Exit if date pickers aren't initialized yet
                }
                
                // Update date pickers based on selected period
                DateTime fromDate;
                DateTime toDate = DateTime.Today;
                
                switch (cmbReportPeriod.SelectedIndex)
                {
                    case 0: // This Month
                        fromDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                        break;
                    case 1: // Last Month
                        var lastMonth = DateTime.Today.AddMonths(-1);
                        fromDate = new DateTime(lastMonth.Year, lastMonth.Month, 1);
                        toDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddDays(-1);
                        break;
                    case 2: // This Year
                        fromDate = new DateTime(DateTime.Today.Year, 1, 1);
                        break;
                    case 3: // Last Year
                        fromDate = new DateTime(DateTime.Today.Year - 1, 1, 1);
                        toDate = new DateTime(DateTime.Today.Year - 1, 12, 31);
                        break;
                    case 4: // Custom
                        // Keep existing custom dates
                        return;
                    default:
                        fromDate = DateTime.Today.AddDays(-30);
                        break;
                }
                
                // Update date pickers to reflect the selected period
                dpReportFrom.SelectedDate = fromDate;
                dpReportTo.SelectedDate = toDate;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error setting date range: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void BtnGenerateReport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_orderService == null || _customerService == null || _orderDetailService == null) 
                {
                    MessageBox.Show("Services are not initialized properly.", "Error", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                
                if (dpReportFrom == null || dpReportTo == null || dgSalesReport == null)
                {
                    MessageBox.Show("UI controls are not initialized properly.", "Error", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                
                DateTime fromDate = dpReportFrom.SelectedDate ?? DateTime.Today.AddDays(-30);
                DateTime toDate = dpReportTo.SelectedDate ?? DateTime.Today;
                
                // Get orders for the selected period
                var orders = _orderService.GetOrdersByDateRange(fromDate, toDate);
                
                // Create view models for the report
                var orderViewModels = orders.Select(o => new OrderViewModel
                {
                    OrderID = o.OrderID,
                    CustomerName = GetCustomerName(o.CustomerID),
                    OrderDate = o.OrderDate,
                    EmployeeName = GetEmployeeName(o.EmployeeID),
                    TotalAmount = CalculateOrderTotal(o.OrderID),
                    ItemCount = GetOrderItemCount(o.OrderID)
                }).ToList();
                
                // Display in DataGrid
                dgSalesReport.ItemsSource = orderViewModels;
                
                // Calculate and display summary metrics
                decimal totalRevenue = orderViewModels.Sum(o => o.TotalAmount);
                int totalOrders = orderViewModels.Count;
                decimal avgOrderValue = totalOrders > 0 ? totalRevenue / totalOrders : 0;
                int totalItems = orderViewModels.Sum(o => o.ItemCount);
                
                if (txtTotalOrders != null) txtTotalOrders.Text = totalOrders.ToString();
                if (txtTotalRevenue != null) txtTotalRevenue.Text = totalRevenue.ToString("C");
                if (txtAvgOrderValue != null) txtAvgOrderValue.Text = avgOrderValue.ToString("C");
                if (txtTotalItems != null) txtTotalItems.Text = totalItems.ToString();
                
                // Update status
                if (txtReportStatus != null)
                {
                    txtReportStatus.Text = $"Report generated for period {fromDate:MM/dd/yyyy} - {toDate:MM/dd/yyyy}";
                }
                
                // Get top selling products (to be implemented)
                // This is a placeholder implementation
                if (lstTopProducts != null)
                {
                    lstTopProducts.Items.Clear();
                }
                
                MessageBox.Show($"Report generated successfully. Found {totalOrders} orders in the selected period.", 
                    "Report Generated", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating report: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private string GetCustomerName(int customerId)
        {
            try
            {
                if (_customerService == null) return "Unknown";
                
                var customer = _customerService.GetCustomerByID(customerId);
                return customer != null ? customer.CompanyName : "Unknown";
            }
            catch
            {
                return "Unknown";
            }
        }
        
        private string GetEmployeeName(int employeeId)
        {
            // TODO: Implement using your employee service
            return "Employee " + employeeId; // Placeholder
        }
        
        private decimal CalculateOrderTotal(int orderId)
        {
            try
            {
                if (_orderDetailService == null) return 0;
                
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
                if (_orderDetailService == null) return 0;
                
                var orderDetails = _orderDetailService.GetOrderDetailsByOrderID(orderId);
                return orderDetails.Sum(od => od.Quantity);
            }
            catch
            {
                return 0;
            }
        }
        
        private void BtnExportReport_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Export Report functionality will be implemented later.", 
                "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        
        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            // Return to AdminDashboard
            AdminDashBoard adminDashboard = new AdminDashBoard();
            adminDashboard.Show();
            this.Close();
        }
    }
}
