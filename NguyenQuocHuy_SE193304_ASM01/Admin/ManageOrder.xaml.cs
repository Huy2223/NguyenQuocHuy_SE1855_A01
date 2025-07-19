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
using OrderEntity = BusinessObject.Order;
using CustomerEntity = BusinessObject.Customer;
using EmployeeEntity = BusinessObject.Employee;
using ProductEntity = BusinessObject.Product;
using OrderDetailEntity = BusinessObject.OrderDetail;

namespace NguyenQuocHuyWPF.Admin
{
    /// <summary>
    /// Interaction logic for ManageOrder.xaml
    /// </summary>
    public partial class ManageOrder : Window
    {
        private readonly IOrderService _orderService;
        private readonly ICustomerService _customerService;
        private readonly IOrderDetailService _orderDetailService;
        private readonly IEmployeeService _employeeService;
        private ObservableCollection<OrderViewModel> _orders;

        public ManageOrder()
        {
            InitializeComponent();

            // Initialize services
            _orderService = new OrderService();
            _customerService = new CustomerService();
            _orderDetailService = new OrderDetailService();
            _employeeService = new EmployeeService();

            // Initialize observable collection
            _orders = new ObservableCollection<OrderViewModel>();
            dgOrders.ItemsSource = _orders;

            // Load all orders when window opens
            this.Loaded += (s, e) =>
            {
                // Set default dates to show all data (2 years back for comprehensive view)
                if (dpOrdersFrom != null) dpOrdersFrom.SelectedDate = DateTime.Today.AddYears(-2);
                if (dpOrdersTo != null) dpOrdersTo.SelectedDate = DateTime.Today;

                // Load ALL Orders by default to show comprehensive view
                LoadAllOrders();
            };
        }

        private void LoadOrders()
        {
            try
            {
                if (dpOrdersFrom == null || dpOrdersTo == null || dgOrders == null || txtOrderStatus == null)
                {
                    MessageBox.Show("UI controls are not initialized properly.", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                DateTime? fromDate = dpOrdersFrom.SelectedDate;
                DateTime? toDate = dpOrdersTo.SelectedDate;

                IEnumerable<OrderEntity> orders;

                if (fromDate.HasValue && toDate.HasValue)
                {
                    // Filter by date range
                    orders = _orderService.GetOrdersByDateRange(fromDate.Value, toDate.Value);
                    txtOrderStatus.Text = $"📅 Filtered Orders: {orders.Count()} | From {fromDate:MM/dd/yyyy} to {toDate:MM/dd/yyyy}";
                }
                else
                {
                    // Show all orders
                    orders = _orderService.GetAllOrders();
                    txtOrderStatus.Text = $"📋 Total Orders: {orders.Count()} | Showing all orders";
                }

                // Create view model for displaying orders with additional info
                var orderViewModels = orders.Select(o => new OrderViewModel
                {
                    OrderId = o.OrderId,
                    CustomerName = GetCustomerName(o.CustomerId),
                    OrderDate = o.OrderDate,
                    EmployeeName = GetEmployeeName(o.EmployeeId),
                    TotalAmount = CalculateOrderTotal(o.OrderId),
                    ItemCount = GetOrderItemCount(o.OrderId)
                }).OrderByDescending(o => o.OrderDate).ToList(); // Sort by newest first

                // Update ObservableCollection with the new data
                _orders.Clear();
                foreach (var orderViewModel in orderViewModels)
                {
                    _orders.Add(orderViewModel);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading orders: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadAllOrders()
        {
            try
            {
                if (dgOrders == null || txtOrderStatus == null)
                {
                    MessageBox.Show("UI controls are not initialized properly.", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Get ALL orders (not filtered by date initially)
                var orders = _orderService.GetAllOrders();

                // Create view model for displaying orders with additional info
                var orderViewModels = orders.Select(o => new OrderViewModel
                {
                    OrderId = o.OrderId,
                    CustomerName = GetCustomerName(o.CustomerId),
                    OrderDate = o.OrderDate,
                    EmployeeName = GetEmployeeName(o.EmployeeId),
                    TotalAmount = CalculateOrderTotal(o.OrderId),
                    ItemCount = GetOrderItemCount(o.OrderId)
                }).OrderByDescending(o => o.OrderDate).ToList(); // Sort by newest first

                // Update ObservableCollection with the new data
                _orders.Clear();
                foreach (var orderViewModel in orderViewModels)
                {
                    _orders.Add(orderViewModel);
                }

                txtOrderStatus.Text = $"🏪 All Orders Loaded: {_orders.Count} total orders | Ready for management";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading orders: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string GetCustomerName(int customerId)
        {
            try
            {
                var customer = _customerService.GetCustomerByID(customerId);
                return customer != null ? customer.CompanyName : "Unknown Customer";
            }
            catch
            {
                return "Unknown Customer";
            }
        }

        private string GetEmployeeName(int employeeId)
        {
            try
            {
                var employee = _employeeService.GetEmployeeByID(employeeId);
                return employee != null ? employee.Name : $"Employee #{employeeId}";
            }
            catch
            {
                return $"Employee #{employeeId}";
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

        private void BtnSearchOrders_Click(object sender, RoutedEventArgs e)
        {
            // Validate date range before filtering
            if (dpOrdersFrom.SelectedDate.HasValue && dpOrdersTo.SelectedDate.HasValue)
            {
                if (dpOrdersTo.SelectedDate < dpOrdersFrom.SelectedDate)
                {
                    MessageBox.Show("End date cannot be earlier than start date.", "Invalid Date Range",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            // Filter the data based on date range
            LoadOrders();
        }

        private void BtnShowAllOrders_Click(object sender, RoutedEventArgs e)
        {
            // Reset date filters and show all orders
            if (dpOrdersFrom != null) dpOrdersFrom.SelectedDate = DateTime.Today.AddYears(-2);
            if (dpOrdersTo != null) dpOrdersTo.SelectedDate = DateTime.Today;
            LoadAllOrders();
        }

        private void BtnCreateOrder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBox.Show("Create Order functionality will be implemented soon.", "Feature Coming Soon",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                
                // Refresh orders list after potential creation
                LoadAllOrders();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnEditOrder_Click(object sender, RoutedEventArgs e)
        {
            var orderViewModel = (sender as Button)?.DataContext as OrderViewModel;
            if (orderViewModel != null)
            {
                try
                {
                    MessageBox.Show($"Edit Order #{orderViewModel.OrderId} functionality will be implemented soon.", "Feature Coming Soon",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    
                    // Refresh orders list after potential edit
                    LoadAllOrders();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnViewOrder_Click(object sender, RoutedEventArgs e)
        {
            var orderViewModel = (sender as Button)?.DataContext as OrderViewModel;
            if (orderViewModel != null)
            {
                try
                {
                    // Get order details
                    var orderDetails = _orderDetailService.GetOrderDetailsByOrderID(orderViewModel.OrderId);

                    // Format order details for display
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine($"🧾 ORDER DETAILS #{orderViewModel.OrderId}");
                    sb.AppendLine("".PadRight(50, '='));
                    sb.AppendLine($"📅 Date: {orderViewModel.OrderDate:dddd, MMMM dd, yyyy}");
                    sb.AppendLine($"👤 Customer: {orderViewModel.CustomerName}");
                    sb.AppendLine($"👨‍💼 Employee: {orderViewModel.EmployeeName}");
                    sb.AppendLine();
                    sb.AppendLine("📦 ORDER ITEMS:");
                    sb.AppendLine("".PadRight(50, '-'));

                    foreach (var detail in orderDetails)
                    {
                        // Get product name
                        string productName = GetProductName(detail.ProductId);

                        decimal lineTotal = detail.UnitPrice * detail.Quantity * (1 - (decimal)detail.Discount);

                        sb.AppendLine($"• {detail.Quantity}x {productName}");
                        sb.AppendLine($"  💰 Unit Price: ${detail.UnitPrice:0.00}");

                        if (detail.Discount > 0)
                        {
                            sb.AppendLine($"  🏷️ Discount: {detail.Discount:P0}");
                        }

                        sb.AppendLine($"  💵 Line Total: ${lineTotal:0.00}");
                        sb.AppendLine();
                    }

                    sb.AppendLine("".PadRight(50, '='));
                    sb.AppendLine($"📊 Total Items: {orderViewModel.ItemCount}");
                    sb.AppendLine($"💰 ORDER TOTAL: ${orderViewModel.TotalAmount:0.00}");

                    MessageBox.Show(sb.ToString(), $"Order #{orderViewModel.OrderId} Details", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading order details: {ex.Message}", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private string GetProductName(int productId)
        {
            try
            {
                var productService = new ProductService();
                var product = productService.GetProductByID(productId);
                return product != null ? product.ProductName : $"Product #{productId}";
            }
            catch
            {
                return $"Product #{productId}";
            }
        }

        private void BtnDeleteOrder_Click(object sender, RoutedEventArgs e)
        {
            var orderViewModel = (sender as Button)?.DataContext as OrderViewModel;
            if (orderViewModel != null)
            {
                var result = MessageBox.Show(
                    $"⚠️ Are you sure you want to delete Order #{orderViewModel.OrderId}?\n\n" +
                    $"Customer: {orderViewModel.CustomerName}\n" +
                    $"Date: {orderViewModel.OrderDate:MM/dd/yyyy}\n" +
                    $"Total: ${orderViewModel.TotalAmount:0.00}\n\n" +
                    "This action cannot be undone!",
                    "Confirm Delete Order", 
                    MessageBoxButton.YesNo, 
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        // Delete order details first manually
                        var orderDetails = _orderDetailService.GetOrderDetailsByOrderID(orderViewModel.OrderId);
                        foreach (var detail in orderDetails)
                        {
                            _orderDetailService.DeleteOrderDetail(detail.OrderId, detail.ProductId);
                        }

                        // Then delete the order
                        _orderService.DeleteOrder(orderViewModel.OrderId);

                        // Remove from ObservableCollection (UI updates automatically)
                        _orders.Remove(orderViewModel);
                        txtOrderStatus.Text = $"🗑️ Order deleted | Remaining Orders: {_orders.Count}";

                        // Show success message
                        MessageBox.Show($"✅ Order #{orderViewModel.OrderId} has been successfully deleted.", "Delete Successful",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"❌ Error deleting order: {ex.Message}", "Delete Failed",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void DgOrders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Optional: Handle selection changed event for future features
            if (dgOrders.SelectedItem is OrderViewModel selectedOrder)
            {
                // Could show quick info or enable/disable buttons based on selection
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Return to AdminDashboard
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

    // ViewModel for Order display
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

