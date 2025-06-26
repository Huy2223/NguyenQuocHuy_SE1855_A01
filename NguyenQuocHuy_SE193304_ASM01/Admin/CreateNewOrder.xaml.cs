using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace NguyenQuocHuyWPF.Admin
{
    /// <summary>
    /// Interaction logic for CreateNewOrder.xaml
    /// </summary>
    public partial class CreateNewOrder : Window
    {
        private readonly ICustomerService _customerService;
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;
        private readonly IOrderDetailService _orderDetailService;
        private readonly IEmployeeService _employeeService;
        
        // Collection to hold order items
        private ObservableCollection<OrderItemViewModel> _orderItems;
        
        // Currently selected product
        private Products? _selectedProduct;
        
        // Event to notify parent window when an order is created
        public event EventHandler<OrderCreatedEventArgs>? OrderCreated;
        
        public CreateNewOrder()
        {
            InitializeComponent();
            
            // Initialize services
            _customerService = new CustomerService();
            _productService = new ProductService();
            _orderService = new OrderService();
            _orderDetailService = new OrderDetailService();
            _employeeService = new EmployeeService();
            
            // Initialize order items collection
            _orderItems = new ObservableCollection<OrderItemViewModel>();
            dgOrderItems.ItemsSource = _orderItems;
            
            // Set default order date to today
            dpOrderDate.SelectedDate = DateTime.Today;
            
            // Load data when window is loaded
            this.Loaded += (s, e) =>
            {
                LoadCustomers();
                LoadProducts();
                LoadEmployees();
            };
        }
        
        private void LoadCustomers()
        {
            try
            {
                var customers = _customerService.GetAllCustomers();
                cmbCustomer.ItemsSource = customers;
                
                // Select the first customer by default if available
                if (customers.Any())
                {
                    cmbCustomer.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading customers: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void LoadProducts()
        {
            try
            {
                var products = _productService.GetAllProducts();
                cmbProducts.ItemsSource = products;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading products: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void LoadEmployees()
        {
            try
            {
                var employees = _employeeService.GetAllEmployees();
                cmbEmployee.ItemsSource = employees;
                
                // Select the first employee by default if available
                if (employees.Any())
                {
                    cmbEmployee.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading employees: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void CmbProducts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedProduct = cmbProducts.SelectedItem as Products;
            UpdateProductPrice();
        }
        
        private void UpdateProductPrice()
        {
            if (_selectedProduct != null)
            {
                txtProductPrice.Text = $"${_selectedProduct.UnitPrice:0.00}";
            }
            else
            {
                txtProductPrice.Text = "$0.00";
            }
        }
        
        private void TxtQuantity_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Only allow digits in quantity field
            e.Handled = !IsDigitsOnly(e.Text);
        }
        
        private void TxtDiscount_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Allow only digits and one decimal point
            e.Handled = !IsValidDecimal(txtDiscount.Text, e.Text);
        }
        
        private bool IsDigitsOnly(string text)
        {
            return Regex.IsMatch(text, @"^\d+$");
        }
        
        private bool IsValidDecimal(string currentText, string input)
        {
            string newText = currentText + input;
            
            // Check if the new text would be a valid decimal
            if (decimal.TryParse(newText, out _))
                return true;
            
            // Special case: Allow a single decimal point
            if (input == "." && !currentText.Contains("."))
                return true;
            
            return false;
        }
        
        private void BtnAddToOrder_Click(object sender, RoutedEventArgs e)
        {
            // Hide previous error message
            HideError();
            
            // Check if a product is selected
            if (_selectedProduct == null)
            {
                ShowError("Please select a product to add to the order.");
                return;
            }
            
            // Validate quantity
            if (!int.TryParse(txtQuantity.Text, out int quantity) || quantity <= 0)
            {
                ShowError("Please enter a valid quantity (greater than zero).");
                txtQuantity.Focus();
                return;
            }
            
            // Validate discount
            if (!float.TryParse(txtDiscount.Text, out float discountPercent) || discountPercent < 0 || discountPercent > 100)
            {
                ShowError("Please enter a valid discount percentage (between 0 and 100).");
                txtDiscount.Focus();
                return;
            }
            
            // Convert discount from percentage to decimal (e.g., 20% -> 0.2)
            float discount = discountPercent / 100f;
            
            // Check if this product is already in the order
            var existingItem = _orderItems.FirstOrDefault(item => item.ProductID == _selectedProduct.ProductID);
            
            if (existingItem != null)
            {
                // Update existing item
                existingItem.Quantity += quantity;
                existingItem.UpdateTotal();
                
                // Refresh the DataGrid
                dgOrderItems.Items.Refresh();
            }
            else
            {
                // Add new item to order
                _orderItems.Add(new OrderItemViewModel
                {
                    ProductID = _selectedProduct.ProductID,
                    ProductName = _selectedProduct.ProductName,
                    UnitPrice = _selectedProduct.UnitPrice,
                    Quantity = quantity,
                    Discount = discount
                });
            }
            
            // Reset quantity to 1 and discount to 0 for next item
            txtQuantity.Text = "1";
            txtDiscount.Text = "0";
            
            // Update order summary
            UpdateOrderSummary();
            
            // Clear product selection
            cmbProducts.SelectedIndex = -1;
            _selectedProduct = null;
            UpdateProductPrice();
        }
        
        private void BtnRemoveItem_Click(object sender, RoutedEventArgs e)
        {
            // Get the item to remove
            var item = (sender as Button)?.DataContext as OrderItemViewModel;
            
            if (item != null)
            {
                // Remove the item from the collection
                _orderItems.Remove(item);
                
                // Update order summary
                UpdateOrderSummary();
            }
        }
        
        private void UpdateOrderSummary()
        {
            // Update total items
            txtTotalItems.Text = _orderItems.Sum(item => item.Quantity).ToString();
            
            // Update order total
            decimal total = _orderItems.Sum(item => item.Total);
            txtOrderTotal.Text = $"${total:0.00}";
        }
        
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            // Confirm if there are items in the order
            if (_orderItems.Count > 0)
            {
                if (MessageBox.Show("Are you sure you want to cancel this order?", "Confirm", 
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                {
                    return;
                }
            }
            
            this.Close();
        }
        
        private void BtnCreateOrder_Click(object sender, RoutedEventArgs e)
        {
            // Hide previous error message
            HideError();
            
            // Validate customer selection
            if (cmbCustomer.SelectedItem == null)
            {
                ShowError("Please select a customer for this order.");
                cmbCustomer.Focus();
                return;
            }
            
            // Validate employee selection
            if (cmbEmployee.SelectedItem == null)
            {
                ShowError("Please select an employee for this order.");
                cmbEmployee.Focus();
                return;
            }
            
            // Validate order date
            if (!dpOrderDate.SelectedDate.HasValue)
            {
                ShowError("Please select an order date.");
                dpOrderDate.Focus();
                return;
            }
            
            // Check if there are items in the order
            if (_orderItems.Count == 0)
            {
                ShowError("Please add at least one product to the order.");
                cmbProducts.Focus();
                return;
            }
            
            try
            {
                // Create order object
                var order = new Orders
                {
                    CustomerID = (int)cmbCustomer.SelectedValue,
                    EmployeeID = (int)cmbEmployee.SelectedValue,
                    OrderDate = dpOrderDate.SelectedDate.Value
                };
                
                // Save order to database
                _orderService.AddOrder(order);
                
                // Retrieve the last added order to get the ID
                var newOrder = _orderService.GetOrdersByCustomerID(order.CustomerID)
                    .OrderByDescending(o => o.OrderDate)
                    .FirstOrDefault();
                
                if (newOrder == null)
                {
                    throw new Exception("Failed to retrieve the new order information.");
                }
                
                // Create order details
                foreach (var item in _orderItems)
                {
                    var orderDetail = new OrderDetails
                    {
                        OrderID = newOrder.OrderID,
                        ProductID = item.ProductID,
                        UnitPrice = item.UnitPrice,
                        Quantity = item.Quantity,
                        Discount = item.Discount
                    };
                    
                    // Save order detail to database
                    _orderDetailService.AddOrderDetail(orderDetail);
                }
                
                // Notify parent window
                OrderCreated?.Invoke(this, new OrderCreatedEventArgs(newOrder));
                
                // Show success message
                MessageBox.Show($"Order #{newOrder.OrderID} created successfully!", "Success", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
                
                // Close dialog
                this.Close();
            }
            catch (Exception ex)
            {
                ShowError($"Error creating order: {ex.Message}");
            }
        }
        
        private void ShowError(string message)
        {
            txtError.Text = message;
            txtError.Visibility = Visibility.Visible;
        }
        
        private void HideError()
        {
            txtError.Text = string.Empty;
            txtError.Visibility = Visibility.Collapsed;
        }
    }
    
    // ViewModel for order items
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
    
    // Event args for order created event
    public class OrderCreatedEventArgs : EventArgs
    {
        public Orders NewOrder { get; private set; }
        
        public OrderCreatedEventArgs(Orders order)
        {
            NewOrder = order;
        }
    }
}
