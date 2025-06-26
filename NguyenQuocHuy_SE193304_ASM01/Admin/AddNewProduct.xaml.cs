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

namespace NguyenQuocHuyWPF.Admin
{
    /// <summary>
    /// Interaction logic for AddNewProduct.xaml
    /// </summary>
    public partial class AddNewProduct : Window
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        
        // Event to notify parent window when a product is added
        public event EventHandler<ProductAddedEventArgs>? ProductAdded;
        
        public AddNewProduct()
        {
            InitializeComponent();
            
            // Initialize services
            _productService = new ProductService();
            _categoryService = new CategoryService();
            
            // Load categories
            LoadCategories();
            
            // Set up preview updating events
            txtProductName.TextChanged += UpdatePreview;
            txtUnitPrice.TextChanged += UpdatePreview;
            txtUnitsInStock.TextChanged += UpdatePreview;
            cmbCategory.SelectionChanged += UpdatePreview;
            
            // Set focus to product name field
            this.Loaded += (s, e) => txtProductName.Focus();
        }
        
        private void LoadCategories()
        {
            try
            {
                var categories = _categoryService.GetAllCategories();
                cmbCategory.ItemsSource = categories;
                
                // Select the first category by default if available
                if (categories.Any())
                {
                    cmbCategory.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading categories: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void UpdatePreview(object sender, EventArgs e)
        {
            // Update product name preview
            previewProductName.Text = string.IsNullOrWhiteSpace(txtProductName.Text) 
                ? "-" 
                : txtProductName.Text;
            
            // Update price preview
            if (decimal.TryParse(txtUnitPrice.Text, out decimal price))
            {
                previewPrice.Text = $"${price:0.00}";
            }
            else
            {
                previewPrice.Text = "-";
            }
            
            // Update stock preview
            if (int.TryParse(txtUnitsInStock.Text, out int stock))
            {
                previewStock.Text = stock.ToString();
            }
            else
            {
                previewStock.Text = "-";
            }
        }
        
        private void TxtUnitPrice_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Allow only digits and one decimal point
            e.Handled = !IsValidDecimal(txtUnitPrice.Text, e.Text);
        }
        
        private void TxtUnitsInStock_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Only allow digits in stock field
            e.Handled = !IsDigitsOnly(e.Text);
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
        
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // Hide previous error message
            HideError();
            
            // Validate input
            if (string.IsNullOrWhiteSpace(txtProductName.Text))
            {
                ShowError("Product Name is required.");
                txtProductName.Focus();
                return;
            }
            
            if (cmbCategory.SelectedItem == null)
            {
                ShowError("Category is required.");
                cmbCategory.Focus();
                return;
            }
            
            if (string.IsNullOrWhiteSpace(txtUnitPrice.Text))
            {
                ShowError("Unit Price is required.");
                txtUnitPrice.Focus();
                return;
            }
            
            if (!decimal.TryParse(txtUnitPrice.Text, out decimal unitPrice))
            {
                ShowError("Unit Price must be a valid number.");
                txtUnitPrice.Focus();
                return;
            }
            
            if (unitPrice < 0)
            {
                ShowError("Unit Price cannot be negative.");
                txtUnitPrice.Focus();
                return;
            }
            
            if (string.IsNullOrWhiteSpace(txtUnitsInStock.Text))
            {
                ShowError("Units in Stock is required.");
                txtUnitsInStock.Focus();
                return;
            }
            
            if (!int.TryParse(txtUnitsInStock.Text, out int unitsInStock))
            {
                ShowError("Units in Stock must be a valid number.");
                txtUnitsInStock.Focus();
                return;
            }
            
            if (unitsInStock < 0)
            {
                ShowError("Units in Stock cannot be negative.");
                txtUnitsInStock.Focus();
                return;
            }
            
            try
            {
                // Create new product object
                var newProduct = new Products
                {
                    ProductName = txtProductName.Text.Trim(),
                    CategoryID = (int)cmbCategory.SelectedValue,
                    UnitPrice = unitPrice,
                    UnitsInStock = unitsInStock
                };
                
                // Save to database
                _productService.AddProduct(newProduct);
                
                // Get the product from the database to retrieve the assigned ID
                var savedProduct = _productService.SearchProductsByName(newProduct.ProductName)
                    .FirstOrDefault(p => p.UnitPrice == newProduct.UnitPrice && p.CategoryID == newProduct.CategoryID);
                
                if (savedProduct != null)
                {
                    // Notify parent window
                    ProductAdded?.Invoke(this, new ProductAddedEventArgs(savedProduct));
                }
                else
                {
                    // If we can't find the exact product, use the one we created
                    // This is a fallback in case the search doesn't work properly
                    ProductAdded?.Invoke(this, new ProductAddedEventArgs(newProduct));
                }
                
                // Show success message
                MessageBox.Show("Product added successfully!", "Success", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
                
                // Close dialog
                this.Close();
            }
            catch (Exception ex)
            {
                ShowError($"Error adding product: {ex.Message}");
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
    
    // Event args for product added event
    public class ProductAddedEventArgs : EventArgs
    {
        public Products NewProduct { get; private set; }
        
        public ProductAddedEventArgs(Products product)
        {
            NewProduct = product;
        }
    }
}
