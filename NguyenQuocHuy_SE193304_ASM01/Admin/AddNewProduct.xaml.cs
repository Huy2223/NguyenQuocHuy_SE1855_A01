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
        
        // Event to notify parent window when a Product is added
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
            
            // Set focus to Product name field
            this.Loaded += (s, e) => txtProductName.Focus();
        }
        
        private void LoadCategories()
        {
            try
            {
                var categories = _categoryService.GetAllCategories().ToList(); // Convert to List for better handling
                
                // Debug: Check what categories we're loading
                System.Diagnostics.Debug.WriteLine($"=== LOADING CATEGORIES ===");
                System.Diagnostics.Debug.WriteLine($"Categories count: {categories.Count}");
                foreach (var cat in categories)
                {
                    System.Diagnostics.Debug.WriteLine($"Category: ID={cat.CategoryId}, Name={cat.CategoryName}");
                }
                System.Diagnostics.Debug.WriteLine($"==========================");
                
                cmbCategory.ItemsSource = categories;
                
                // Select the first category by default if available
                if (categories.Any())
                {
                    // Use both approaches to ensure selection works
                    cmbCategory.SelectedIndex = 0;
                    cmbCategory.SelectedItem = categories.First();
                    
                    System.Diagnostics.Debug.WriteLine($"Selected first category by default:");
                    System.Diagnostics.Debug.WriteLine($"  Index: {cmbCategory.SelectedIndex}");
                    System.Diagnostics.Debug.WriteLine($"  Item: {cmbCategory.SelectedItem}");
                    System.Diagnostics.Debug.WriteLine($"  Value: {cmbCategory.SelectedValue}");
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
            // Update Product name preview
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
            
            // Debug category state before validation
            System.Diagnostics.Debug.WriteLine($"=== PRE-VALIDATION CATEGORY CHECK ===");
            System.Diagnostics.Debug.WriteLine($"SelectedIndex: {cmbCategory.SelectedIndex}");
            System.Diagnostics.Debug.WriteLine($"SelectedItem: {cmbCategory.SelectedItem}");
            System.Diagnostics.Debug.WriteLine($"SelectedValue: {cmbCategory.SelectedValue}");
            System.Diagnostics.Debug.WriteLine($"ItemsSource count: {(cmbCategory.ItemsSource as System.Collections.IEnumerable)?.Cast<object>()?.Count() ?? 0}");
            System.Diagnostics.Debug.WriteLine($"=====================================");
            
            // Validate category selection - simplified check
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
                // Debug: Check what's actually selected
                System.Diagnostics.Debug.WriteLine($"=== CATEGORY DEBUG INFO ===");
                System.Diagnostics.Debug.WriteLine($"SelectedIndex: {cmbCategory.SelectedIndex}");
                System.Diagnostics.Debug.WriteLine($"SelectedItem: {cmbCategory.SelectedItem}");
                System.Diagnostics.Debug.WriteLine($"SelectedValue: {cmbCategory.SelectedValue}");
                
                if (cmbCategory.SelectedItem is Category selectedCategory)
                {
                    System.Diagnostics.Debug.WriteLine($"Selected Category Name: {selectedCategory.CategoryName}");
                    System.Diagnostics.Debug.WriteLine($"Selected Category ID: {selectedCategory.CategoryId}");
                }
                System.Diagnostics.Debug.WriteLine($"============================");
                
                // Try to get category ID using different approaches
                int categoryId = 0;
                
                // Approach 1: Use SelectedItem directly (most reliable)
                if (cmbCategory.SelectedItem is Category category)
                {
                    categoryId = category.CategoryId;
                }
                // Approach 2: Try SelectedValue parsing (backup)
                else if (int.TryParse(cmbCategory.SelectedValue?.ToString(), out int parsedId))
                {
                    categoryId = parsedId;
                }
                else
                {
                    ShowError("Please select a valid category.");
                    cmbCategory.Focus();
                    return;
                }
                
                System.Diagnostics.Debug.WriteLine($"Final Category ID: {categoryId}");
                
                // Create new Product object
                var newProduct = new Product
                {
                    ProductName = txtProductName.Text.Trim(),
                    CategoryId = categoryId,
                    UnitPrice = unitPrice,
                    UnitsInStock = unitsInStock
                };
                
                // Save to database
                _productService.AddProduct(newProduct);
                
                // Get the Product from the database to retrieve the assigned ID
                var savedProduct = _productService.SearchProductsByName(newProduct.ProductName)
                    .FirstOrDefault(p => p.UnitPrice == newProduct.UnitPrice && p.CategoryId == newProduct.CategoryId);
                
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
    
    // Event args for Product added event
    public class ProductAddedEventArgs : EventArgs
    {
        public Product NewProduct { get; private set; }
        
        public ProductAddedEventArgs(Product product)
        {
            NewProduct = product;
        }
    }
}
