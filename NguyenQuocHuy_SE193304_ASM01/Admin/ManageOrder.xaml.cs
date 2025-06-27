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
using NguyenQuocHuyWPF; // Import the namespace containing OrderViewModel

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
        private ObservableCollection<OrderViewModel> _orders;

        public ManageOrder()
        {
            InitializeComponent();

            // Initialize services
            _orderService = new OrderService();
            _customerService = new CustomerService();
            _orderDetailService = new OrderDetailService();

            // Initialize observable collection
            _orders = new ObservableCollection<OrderViewModel>();
            dgOrders.ItemsSource = _orders;

            // Set default dates (last 30 days)
            this.Loaded += (s, e) =>
            {
                if (dpOrdersFrom != null) dpOrdersFrom.SelectedDate = DateTime.Today.AddDays(-30);
                if (dpOrdersTo != null) dpOrdersTo.SelectedDate = DateTime.Today;

                // Load orders
                LoadOrders();
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

                IEnumerable<Orders> orders;

                if (fromDate.HasValue && toDate.HasValue)
                {
                    orders = _orderService.GetOrdersByDateRange(fromDate.Value, toDate.Value);
                }
                else
                {
                    orders = _orderService.GetAllOrders();
                }

                // Create view model for displaying orders with additional info
                var orderViewModels = orders.Select(o => new OrderViewModel
                {
                    OrderID = o.OrderID,
                    CustomerName = GetCustomerName(o.CustomerID),
                    OrderDate = o.OrderDate,
                    EmployeeName = GetEmployeeName(o.EmployeeID),
                    TotalAmount = CalculateOrderTotal(o.OrderID),
                    ItemCount = GetOrderItemCount(o.OrderID)
                }).ToList();

                // Update ObservableCollection with the new data
                _orders.Clear();
                foreach (var order in orderViewModels)
                {
                    _orders.Add(order);
                }

                txtOrderStatus.Text = $"Total orders: {_orders.Count}";
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
            LoadOrders();
        }

        private void BtnCreateOrder_Click(object sender, RoutedEventArgs e)
        {
            // Create and show the CreateNewOrder dialog
            var createOrderDialog = new CreateNewOrder();

            // Subscribe to the OrderCreated event
            createOrderDialog.OrderCreated += (s, args) =>
            {
                // Create a new OrderViewModel for the new order
                var newOrderVM = new OrderViewModel
                {
                    OrderID = args.NewOrder.OrderID,
                    CustomerName = GetCustomerName(args.NewOrder.CustomerID),
                    OrderDate = args.NewOrder.OrderDate,
                    EmployeeName = GetEmployeeName(args.NewOrder.EmployeeID),
                    TotalAmount = CalculateOrderTotal(args.NewOrder.OrderID),
                    ItemCount = GetOrderItemCount(args.NewOrder.OrderID)
                };

                // Add the new order to the ObservableCollection
                _orders.Add(newOrderVM);
                txtOrderStatus.Text = $"Total orders: {_orders.Count}";
            };

            createOrderDialog.Owner = this;
            createOrderDialog.ShowDialog();
        }

        private void BtnEditOrder_Click(object sender, RoutedEventArgs e)
        {
            var orderViewModel = (sender as Button)?.DataContext as OrderViewModel;
            if (orderViewModel != null)
            {
                try
                {
                    // Get the order to edit
                    var order = _orderService.GetOrderByID(orderViewModel.OrderID);
                    if (order == null)
                    {
                        MessageBox.Show("Order not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Create and show the EditOrder dialog
                    var editOrderDialog = new EditOrder(order);

                    // Subscribe to the OrderUpdated event
                    editOrderDialog.OrderUpdated += (s, args) =>
                    {
                        // Update the OrderViewModel with the edited order data
                        orderViewModel.OrderDate = args.UpdatedOrder.OrderDate;
                        orderViewModel.CustomerName = GetCustomerName(args.UpdatedOrder.CustomerID);
                        orderViewModel.EmployeeName = GetEmployeeName(args.UpdatedOrder.EmployeeID);
                        orderViewModel.TotalAmount = CalculateOrderTotal(args.UpdatedOrder.OrderID);
                        orderViewModel.ItemCount = GetOrderItemCount(args.UpdatedOrder.OrderID);

                        // Refresh the DataGrid
                        dgOrders.Items.Refresh();
                    };

                    editOrderDialog.Owner = this;
                    editOrderDialog.ShowDialog();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error editing order: {ex.Message}", "Error",
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
                    var orderDetails = _orderDetailService.GetOrderDetailsByOrderID(orderViewModel.OrderID);

                    // Format order details for display
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine($"Order #{orderViewModel.OrderID}");
                    sb.AppendLine($"Date: {orderViewModel.OrderDate:MM/dd/yyyy}");
                    sb.AppendLine($"Customer: {orderViewModel.CustomerName}");
                    sb.AppendLine($"Employee: {orderViewModel.EmployeeName}");
                    sb.AppendLine();
                    sb.AppendLine("Order Details:");
                    sb.AppendLine("--------------------------------------------------");

                    foreach (var detail in orderDetails)
                    {
                        // Get product name - in a real app, you would use a join or a service method
                        string productName = "Product " + detail.ProductID; // Placeholder

                        decimal lineTotal = detail.UnitPrice * detail.Quantity * (1 - (decimal)detail.Discount);

                        sb.AppendLine($"{detail.Quantity} x {productName}");
                        sb.AppendLine($"   Unit Price: ${detail.UnitPrice:0.00}");

                        if (detail.Discount > 0)
                        {
                            sb.AppendLine($"   Discount: {detail.Discount:P0}");
                        }

                        sb.AppendLine($"   Line Total: ${lineTotal:0.00}");
                        sb.AppendLine();
                    }

                    sb.AppendLine("--------------------------------------------------");
                    sb.AppendLine($"Total Items: {orderViewModel.ItemCount}");
                    sb.AppendLine($"Order Total: ${orderViewModel.TotalAmount:0.00}");

                    MessageBox.Show(sb.ToString(), "Order Details", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading order details: {ex.Message}", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnDeleteOrder_Click(object sender, RoutedEventArgs e)
        {
            var orderViewModel = (sender as Button)?.DataContext as OrderViewModel;
            if (orderViewModel != null)
            {
                if (MessageBox.Show($"Are you sure you want to delete Order #{orderViewModel.OrderID}?",
                    "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    try
                    {
                        // Delete order details first manually
                        var orderDetails = _orderDetailService.GetOrderDetailsByOrderID(orderViewModel.OrderID);
                        foreach (var detail in orderDetails)
                        {
                            _orderDetailService.DeleteOrderDetail(detail.OrderID, detail.ProductID);
                        }

                        // Then delete the order
                        _orderService.DeleteOrder(orderViewModel.OrderID);

                        // Remove from ObservableCollection (UI updates automatically)
                        _orders.Remove(orderViewModel);
                        txtOrderStatus.Text = $"Total orders: {_orders.Count}";

                        // Show success message
                        MessageBox.Show("Order deleted successfully.", "Success",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error deleting order: {ex.Message}", "Error",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void DgOrders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Optional: Handle selection changed event
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
