using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Microsoft.Win32;
using System.IO;

namespace NguyenQuocHuyWPF.Admin
{
    /// <summary>
    /// Interaction logic for SaleReport.xaml
    /// </summary>
    public partial class SaleReport : Window
    {
        private readonly IOrderService _orderService;
        private readonly ICustomerService _customerService;
        private readonly IOrderDetailService _orderDetailService;
        private readonly IProductService _productService;
        private readonly IEmployeeService _employeeService;
        private ObservableCollection<SalesReportItem> _salesReportItems;
        private bool _isWindowLoaded = false;

        public SaleReport()
        {
            try
            {
                InitializeComponent();

                // Initialize services
                _orderService = new OrderService();
                _customerService = new CustomerService();
                _orderDetailService = new OrderDetailService();
                _productService = new ProductService();
                _employeeService = new EmployeeService();

                // Initialize the observable collection
                _salesReportItems = new ObservableCollection<SalesReportItem>();

                // Wait for window to load before accessing UI elements
                this.Loaded += SaleReport_Loaded;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing the Sales Report: {ex.Message}", "Initialization Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaleReport_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Set datacontext and ItemsSource after window is loaded
                dgSalesReport.ItemsSource = _salesReportItems;

                // Mark window as loaded
                _isWindowLoaded = true;

                // Set default date range to show ALL data (from 2 years back to now)
                DateTime now = DateTime.Now;
                if (dpReportFrom != null && dpReportTo != null)
                {
                    dpReportFrom.SelectedDate = now.AddYears(-2); // Show 2 years back by default
                    dpReportTo.SelectedDate = now;
                }

                // Select "All Data" option by default
                if (cmbReportPeriod != null && cmbReportPeriod.Items.Count > 0)
                {
                    cmbReportPeriod.SelectedIndex = 0; // All Data
                }

                // Automatically generate report for ALL data when window loads
                GenerateAllDataReport();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading the report window: {ex.Message}", "Loading Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CmbReportPeriod_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                // Safety check - only proceed if the window is fully loaded
                if (!_isWindowLoaded) return;

                // Safety check - ensure ComboBox is valid
                if (sender == null || !(sender is ComboBox) || cmbReportPeriod.SelectedItem == null)
                    return;

                if (cmbReportPeriod.SelectedItem is ComboBoxItem selectedItem)
                {
                    string selectedPeriod = selectedItem.Content.ToString();
                    SetDateRangeForPeriod(selectedPeriod);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error changing period: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SetDateRangeForPeriod(string period)
        {
            // Safety checks
            if (string.IsNullOrEmpty(period) || dpReportFrom == null || dpReportTo == null)
                return;

            if (!_isWindowLoaded)
                return;

            try
            {
                // Always ensure we have a fresh DateTime.Now value
                DateTime currentDate = DateTime.Now;

                switch (period)
                {
                    case "All Data":
                        // Show all available data (2 years back)
                        dpReportFrom.SelectedDate = currentDate.AddYears(-2);
                        dpReportTo.SelectedDate = currentDate;
                        dpReportFrom.IsEnabled = false;
                        dpReportTo.IsEnabled = false;
                        GenerateAllDataReport();
                        break;

                    case "This Month":
                        dpReportFrom.SelectedDate = new DateTime(currentDate.Year, currentDate.Month, 1);
                        dpReportTo.SelectedDate = currentDate;
                        dpReportFrom.IsEnabled = false;
                        dpReportTo.IsEnabled = false;
                        GenerateReport();
                        break;

                    case "Last Month":
                        DateTime lastMonth = currentDate.AddMonths(-1);
                        dpReportFrom.SelectedDate = new DateTime(lastMonth.Year, lastMonth.Month, 1);
                        dpReportTo.SelectedDate = new DateTime(lastMonth.Year, lastMonth.Month, DateTime.DaysInMonth(lastMonth.Year, lastMonth.Month));
                        dpReportFrom.IsEnabled = false;
                        dpReportTo.IsEnabled = false;
                        GenerateReport();
                        break;

                    case "This Year":
                        dpReportFrom.SelectedDate = new DateTime(currentDate.Year, 1, 1);
                        dpReportTo.SelectedDate = currentDate;
                        dpReportFrom.IsEnabled = false;
                        dpReportTo.IsEnabled = false;
                        GenerateReport();
                        break;

                    case "Last Year":
                        int lastYear = currentDate.Year - 1;
                        dpReportFrom.SelectedDate = new DateTime(lastYear, 1, 1);
                        dpReportTo.SelectedDate = new DateTime(lastYear, 12, 31);
                        dpReportFrom.IsEnabled = false;
                        dpReportTo.IsEnabled = false;
                        GenerateReport();
                        break;

                    case "Custom":
                        // For custom range, just enable the date pickers but don't set dates
                        // unless they're not already set
                        if (!dpReportFrom.SelectedDate.HasValue)
                            dpReportFrom.SelectedDate = currentDate.AddDays(-30); // Default to last 30 days
                        if (!dpReportTo.SelectedDate.HasValue)
                            dpReportTo.SelectedDate = currentDate;

                        dpReportFrom.IsEnabled = true;
                        dpReportTo.IsEnabled = true;
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error setting date range: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnGenerateReport_Click(object sender, RoutedEventArgs e)
        {
            // Safety check
            if (dpReportFrom == null || dpReportTo == null)
            {
                MessageBox.Show("Date controls are not properly initialized. Please try restarting the application.",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!dpReportFrom.SelectedDate.HasValue || !dpReportTo.SelectedDate.HasValue)
            {
                MessageBox.Show("Please select valid date range", "Invalid Date Range",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (dpReportTo.SelectedDate < dpReportFrom.SelectedDate)
            {
                MessageBox.Show("End date cannot be earlier than start date", "Invalid Date Range",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            GenerateReport();
        }

        private void GenerateAllDataReport()
        {
            try
            {
                // Get ALL orders without date filtering
                var orders = _orderService.GetAllOrders();

                // Clear existing data
                _salesReportItems.Clear();

                // Calculate total revenue and item count
                decimal totalRevenue = 0;
                int totalItems = 0;

                // Dictionary to track product sales for top product report
                Dictionary<int, ProductSales> productSales = new Dictionary<int, ProductSales>();

                // Process each order
                foreach (var order in orders)
                {
                    // Get order details
                    var orderDetails = _orderDetailService.GetOrderDetailsByOrderID(order.OrderId);

                    // Calculate order total and item count
                    decimal orderTotal = 0;
                    int itemCount = 0;

                    foreach (var detail in orderDetails)
                    {
                        decimal lineTotal = detail.UnitPrice * detail.Quantity * (1 - (decimal)detail.Discount);
                        orderTotal += lineTotal;
                        itemCount += detail.Quantity;
                        totalItems += detail.Quantity;

                        // Track product sales for top product report
                        if (!productSales.ContainsKey(detail.ProductId))
                        {
                            var product = _productService.GetProductByID(detail.ProductId);
                            string productName = product != null ? product.ProductName : "Product " + detail.ProductId;
                            productSales.Add(detail.ProductId, new ProductSales 
                            { 
                                ProductId = detail.ProductId, 
                                ProductName = productName, 
                                QuantitySold = 0, 
                                Revenue = 0 
                            });
                        }

                        productSales[detail.ProductId].QuantitySold += detail.Quantity;
                        productSales[detail.ProductId].Revenue += lineTotal;
                    }

                    // Get customer name
                    string customerName = "Unknown";
                    var customer = _customerService.GetCustomerByID(order.CustomerId);
                    if (customer != null)
                    {
                        customerName = customer.CompanyName;
                    }

                    // Get employee name
                    string employeeName = "Employee " + order.EmployeeId;
                    try
                    {
                        var employee = _employeeService.GetEmployeeByID(order.EmployeeId);
                        if (employee != null)
                        {
                            employeeName = employee.Name;
                        }
                    }
                    catch
                    {
                        // Keep default employee name if service fails
                    }

                    // Add to report items
                    _salesReportItems.Add(new SalesReportItem
                    {
                        OrderId = order.OrderId,
                        OrderDate = order.OrderDate,
                        CustomerName = customerName,
                        EmployeeName = employeeName,
                        TotalAmount = orderTotal,
                        ItemCount = itemCount
                    });

                    totalRevenue += orderTotal;
                }

                // Sort by newest orders first
                var sortedItems = _salesReportItems.OrderByDescending(o => o.OrderDate).ToList();
                _salesReportItems.Clear();
                foreach (var item in sortedItems)
                {
                    _salesReportItems.Add(item);
                }

                // Update summary panel
                if (txtTotalOrders != null) txtTotalOrders.Text = _salesReportItems.Count.ToString();
                if (txtTotalRevenue != null) txtTotalRevenue.Text = $"${totalRevenue:0.00}";
                if (txtTotalItems != null) txtTotalItems.Text = totalItems.ToString();

                // Calculate average order value (avoid divide by zero)
                if (_salesReportItems.Count > 0)
                {
                    decimal avgOrderValue = totalRevenue / _salesReportItems.Count;
                    if (txtAvgOrderValue != null) txtAvgOrderValue.Text = $"${avgOrderValue:0.00}";
                }
                else
                {
                    if (txtAvgOrderValue != null) txtAvgOrderValue.Text = "$0.00";
                }

                // Update status
                if (txtReportStatus != null)
                {
                    txtReportStatus.Text = $"Showing all orders | Total: {_salesReportItems.Count} orders";
                }

                // Populate top products list
                PopulateTopProductsList(productSales);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating all data report: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GenerateReport()
        {
            try
            {
                DateTime startDate = dpReportFrom.SelectedDate.Value;
                DateTime endDate = dpReportTo.SelectedDate.Value.AddDays(1).AddSeconds(-1); // End of the selected day

                // Get orders in the selected date range
                var orders = _orderService.GetOrdersByDateRange(startDate, endDate);

                // Clear existing data
                _salesReportItems.Clear();

                // Calculate total revenue and item count
                decimal totalRevenue = 0;
                int totalItems = 0;

                // Dictionary to track product sales for top product report
                Dictionary<int, ProductSales> productSales = new Dictionary<int, ProductSales>();

                // Process each order
                foreach (var order in orders)
                {
                    // Get order details
                    var orderDetails = _orderDetailService.GetOrderDetailsByOrderID(order.OrderId);

                    // Calculate order total and item count
                    decimal orderTotal = 0;
                    int itemCount = 0;

                    foreach (var detail in orderDetails)
                    {
                        decimal lineTotal = detail.UnitPrice * detail.Quantity * (1 - (decimal)detail.Discount);
                        orderTotal += lineTotal;
                        itemCount += detail.Quantity;
                        totalItems += detail.Quantity;

                        // Track product sales for top product report
                        if (!productSales.ContainsKey(detail.ProductId))
                        {
                            var product = _productService.GetProductByID(detail.ProductId);
                            string productName = product != null ? product.ProductName : "Product " + detail.ProductId;
                            productSales.Add(detail.ProductId, new ProductSales 
                            { 
                                ProductId = detail.ProductId, 
                                ProductName = productName, 
                                QuantitySold = 0, 
                                Revenue = 0 
                            });
                        }

                        productSales[detail.ProductId].QuantitySold += detail.Quantity;
                        productSales[detail.ProductId].Revenue += lineTotal;
                    }

                    // Get customer name
                    string customerName = "Unknown";
                    var customer = _customerService.GetCustomerByID(order.CustomerId);
                    if (customer != null)
                    {
                        customerName = customer.CompanyName;
                    }

                    // Get employee name
                    string employeeName = "Employee " + order.EmployeeId;
                    try
                    {
                        var employee = _employeeService.GetEmployeeByID(order.EmployeeId);
                        if (employee != null)
                        {
                            employeeName = employee.Name;
                        }
                    }
                    catch
                    {
                        // Keep default employee name if service fails
                    }

                    // Add to report items
                    _salesReportItems.Add(new SalesReportItem
                    {
                        OrderId = order.OrderId,
                        OrderDate = order.OrderDate,
                        CustomerName = customerName,
                        EmployeeName = employeeName,
                        TotalAmount = orderTotal,
                        ItemCount = itemCount
                    });

                    totalRevenue += orderTotal;
                }

                // Update summary panel
                if (txtTotalOrders != null) txtTotalOrders.Text = _salesReportItems.Count.ToString();
                if (txtTotalRevenue != null) txtTotalRevenue.Text = $"${totalRevenue:0.00}";
                if (txtTotalItems != null) txtTotalItems.Text = totalItems.ToString();

                // Calculate average order value (avoid divide by zero)
                if (_salesReportItems.Count > 0)
                {
                    decimal avgOrderValue = totalRevenue / _salesReportItems.Count;
                    if (txtAvgOrderValue != null) txtAvgOrderValue.Text = $"${avgOrderValue:0.00}";
                }
                else
                {
                    if (txtAvgOrderValue != null) txtAvgOrderValue.Text = "$0.00";
                }

                // Update status
                if (txtReportStatus != null && dpReportFrom != null && dpReportTo != null)
                {
                    txtReportStatus.Text = $"Report generated for period: {dpReportFrom.SelectedDate:MM/dd/yyyy} - {dpReportTo.SelectedDate:MM/dd/yyyy}";
                }

                // Populate top products list
                PopulateTopProductsList(productSales);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating report: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PopulateTopProductsList(Dictionary<int, ProductSales> productSales)
        {
            try
            {
                // Safety check
                if (lstTopProducts == null) return;

                // Clear existing items
                lstTopProducts.Items.Clear();

                // Sort products by revenue and take top 10
                var topProducts = productSales.Values
                    .OrderByDescending(p => p.Revenue)
                    .Take(10)
                    .ToList();

                // Add to list
                foreach (var product in topProducts)
                {
                    ListBoxItem item = new ListBoxItem();
                    item.Content = $"{product.ProductName} - {product.QuantitySold} units (${product.Revenue:0.00})";
                    lstTopProducts.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error populating top product list: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnExportReport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_salesReportItems == null || _salesReportItems.Count == 0)
                {
                    MessageBox.Show("Please generate a report first.", "No Data",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // Safety check
                if (dpReportFrom == null || dpReportTo == null)
                {
                    MessageBox.Show("Date controls are not properly initialized.", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = "CSV Files (*.csv)|*.csv";
                saveDialog.FileName = $"SalesReport_{DateTime.Now:yyyyMMdd_HHmmss}";

                if (saveDialog.ShowDialog() == true)
                {
                    using (StreamWriter writer = new StreamWriter(saveDialog.FileName))
                    {
                        // Write header
                        writer.WriteLine("Order ID,Date,Customer,Items,Total Amount,Employee");

                        // Write data
                        foreach (var item in _salesReportItems)
                        {
                            writer.WriteLine($"{item.OrderId},{item.OrderDate:MM/dd/yyyy},{item.CustomerName},{item.ItemCount},${item.TotalAmount:0.00},{item.EmployeeName}");
                        }

                        // Write summary
                        writer.WriteLine();
                        writer.WriteLine("SUMMARY");
                        writer.WriteLine($"Total Orders,{txtTotalOrders?.Text}");
                        writer.WriteLine($"Total Revenue,{txtTotalRevenue?.Text}");
                        writer.WriteLine($"Average Order Value,{txtAvgOrderValue?.Text}");
                        writer.WriteLine($"Total Items Sold,{txtTotalItems?.Text}");
                    }

                    MessageBox.Show("Report exported successfully!", "Export Complete",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting report: {ex.Message}", "Export Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Go back to admin dashboard
                AdminDashBoard adminDashboard = new AdminDashBoard();
                adminDashboard.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error returning to dashboard: {ex.Message}", "Navigation Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    // Class to represent a sales report item
    public class SalesReportItem
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string CustomerName { get; set; } = "";
        public string EmployeeName { get; set; } = "";
        public decimal TotalAmount { get; set; }
        public int ItemCount { get; set; }
    }

    // Class to track product sales
    public class ProductSales
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = "";
        public int QuantitySold { get; set; }
        public decimal Revenue { get; set; }
    }
}

