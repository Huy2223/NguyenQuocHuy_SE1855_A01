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
using NguyenQuocHuyWPF.Models; // Import the namespace with the shared OrderItemViewModel class

namespace NguyenQuocHuyWPF.Admin
{
    /// <summary>
    /// Interaction logic for EditOrder.xaml
    /// </summary>
    public partial class EditOrder : Window
    {
        private readonly Orders _order;
        private readonly ICustomerService _customerService;
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;
        private readonly IOrderDetailService _orderDetailService;
        private readonly IEmployeeService _employeeService;
        
        // Collection to hold order items
        private ObservableCollection<OrderItemViewModel> _orderItems;
        
        // Currently selected product
        private Products? _selectedProduct;
        
        // Event to notify parent window when an order is updated
        public event EventHandler<OrderUpdatedEventArgs>? OrderUpdated;
        
        public EditOrder(Orders order)
        {
            InitializeComponent();
            
            // Store the order
            _order = order ?? throw new ArgumentNullException(nameof(order));
            
            // Initialize services
            _customerService = new CustomerService();
            _productService = new ProductService();
            _orderService = new OrderService();
            _orderDetailService = new OrderDetailService();
            _employeeService = new EmployeeService();
            
            // Initialize order items collection
            _orderItems = new ObservableCollection<OrderItemViewModel>();
            dgOrderItems.ItemsSource = _orderItems;
            
            // Load data when window is loaded
            this.Loaded += (s, e) =>
            {
                LoadOrderData();
                LoadCustomers();
                LoadProducts();
                LoadEmployees();
                LoadOrderItems();
            };
        }
        
        private void LoadOrderData()
        {
            // Display order ID (read-only)
            txtOrderID.Text = _order.OrderID.ToString();
            
            // Set order date
            dpOrderDate.SelectedDate = _order.OrderDate;
        }
        
        private void LoadCustomers()
        {
            try
            {
                var customers = _customerService.GetAllCustomers();
                cmbCustomer.ItemsSource = customers;
                
                // Select the customer for this order
                cmbCustomer.SelectedValue = _order.CustomerID;
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
                
                // Select the employee for this order
                cmbEmployee.SelectedValue = _order.EmployeeID;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading employees: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void LoadOrderItems()
        {
            try
            {
                // Get order details for this order
                var orderDetails = _orderDetailService.GetOrderDetailsByOrderID(_order.OrderID);
                
                // Clear existing items
                _orderItems.Clear();
                
                // Add each item to the collection
                foreach (var detail in orderDetails)
                {
                    // Get product information
                    var product = _productService.GetProductByID(detail.ProductID);
                    if (product == null)
                        continue;
                        
                    // Add to collection
                    _orderItems.Add(new OrderItemViewModel
                    {
                        ProductID = product.ProductID,
                        ProductName = product.ProductName,
                        UnitPrice = detail.UnitPrice,
                        Quantity = detail.Quantity,
                        Discount = detail.Discount
                    });
                }
                
                // Update order summary
                UpdateOrderSummary();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading order items: {ex.Message}", "Error", 
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
            // Confirm if there are changes to the order
            if (MessageBox.Show("Are you sure you want to cancel editing this order? Any changes will be lost.", 
                "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }
        
        private void BtnSaveChanges_Click(object sender, RoutedEventArgs e)
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
                // Update order object
                _order.CustomerID = (int)cmbCustomer.SelectedValue;
                _order.EmployeeID = (int)cmbEmployee.SelectedValue;
                _order.OrderDate = dpOrderDate.SelectedDate.Value;
                
                // Update order in database
                _orderService.UpdateOrder(_order);
                
                // Delete existing order details
                var existingDetails = _orderDetailService.GetOrderDetailsByOrderID(_order.OrderID);
                foreach (var detail in existingDetails)
                {
                    _orderDetailService.DeleteOrderDetail(_order.OrderID, detail.ProductID);
                }
                
                // Create new order details
                foreach (var item in _orderItems)
                {
                    var orderDetail = new OrderDetails
                    {
                        OrderID = _order.OrderID,
                        ProductID = item.ProductID,
                        UnitPrice = item.UnitPrice,
                        Quantity = item.Quantity,
                        Discount = item.Discount
                    };
                    
                    // Save order detail to database
                    _orderDetailService.AddOrderDetail(orderDetail);
                }
                
                // Notify parent window
                OrderUpdated?.Invoke(this, new OrderUpdatedEventArgs(_order));
                
                // Show success message
                MessageBox.Show($"Order #{_order.OrderID} updated successfully!", "Success", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
                
                // Close dialog
                this.Close();
            }
            catch (Exception ex)
            {
                ShowError($"Error updating order: {ex.Message}");
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
    
    // Event args for order updated event
    public class OrderUpdatedEventArgs : EventArgs
    {
        public Orders UpdatedOrder { get; private set; }
        
        public OrderUpdatedEventArgs(Orders order)
        {
            UpdatedOrder = order;
        }
    }
}
