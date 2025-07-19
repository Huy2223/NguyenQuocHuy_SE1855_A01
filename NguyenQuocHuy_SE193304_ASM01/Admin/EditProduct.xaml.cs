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
    /// Interaction logic for EditProduct.xaml
    /// </summary>
    public partial class EditProduct : Window
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly Product _originalProduct;
        
        // Event to notify parent window when a Product is updated
        public event EventHandler<ProductUpdatedEventArgs>? ProductUpdated;
        
        public EditProduct(Product product)
        {
            InitializeComponent();
            
            // Store the original Product
            _originalProduct = product ?? throw new ArgumentNullException(nameof(product));
            
            // Initialize services
            _productService = new ProductService();
            _categoryService = new CategoryService();
            
            // Load data when window is loaded
            this.Loaded += EditProduct_Loaded;
        }
        
        private void EditProduct_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Set up preview updating events
                txtProductName.TextChanged += UpdatePreview;
                txtUnitPrice.TextChanged += UpdatePreview;
                txtUnitsInStock.TextChanged += UpdatePreview;
                cmbCategory.SelectionChanged += UpdatePreview;
                
                // Load categories and Product data
                LoadCategories();
                LoadProductData();
                
                // Set focus to Product name field
                txtProductName.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing Product editor: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void LoadCategories()
        {
            try
            {
                var categories = _categoryService.GetAllCategories().ToList(); // Convert to List for better handling
                
                // Debug: Check what categories we're loading
                System.Diagnostics.Debug.WriteLine($"=== LOADING CATEGORIES (Edit) ===");
                System.Diagnostics.Debug.WriteLine($"Categories count: {categories.Count}");
                foreach (var cat in categories)
                {
                    System.Diagnostics.Debug.WriteLine($"Category: ID={cat.CategoryId}, Name={cat.CategoryName}");
                }
                System.Diagnostics.Debug.WriteLine($"=================================");
                
                cmbCategory.ItemsSource = categories;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading categories: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void LoadProductData()
        {
            try
            {
                // Set fields to original Product values
                txtProductID.Text = _originalProduct.ProductId.ToString();
                txtProductName.Text = _originalProduct.ProductName;
                txtUnitPrice.Text = _originalProduct.UnitPrice.ToString();
                txtUnitsInStock.Text = _originalProduct.UnitsInStock.ToString();
                
                // Select the appropriate category using multiple approaches
                SetSelectedCategory(_originalProduct.CategoryId);
                
                // Update the preview
                UpdatePreview(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading Product data: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void SetSelectedCategory(int? categoryId)
        {
            if (categoryId == null) return;
            
            try
            {
                // Approach 1: Try SelectedValue first
                cmbCategory.SelectedValue = categoryId;
                
                // Approach 2: If SelectedValue didn't work, find the category manually
                if (cmbCategory.SelectedItem == null && cmbCategory.ItemsSource != null)
                {
                    foreach (Category cat in cmbCategory.ItemsSource)
                    {
                        if (cat.CategoryId == categoryId)
                        {
                            cmbCategory.SelectedItem = cat;
                            break;
                        }
                    }
                }
                
                // Debug: Show what was selected
                System.Diagnostics.Debug.WriteLine($"=== CATEGORY SELECTION ===");
                System.Diagnostics.Debug.WriteLine($"Trying to select CategoryId: {categoryId}");
                System.Diagnostics.Debug.WriteLine($"Final SelectedIndex: {cmbCategory.SelectedIndex}");
                System.Diagnostics.Debug.WriteLine($"Final SelectedItem: {cmbCategory.SelectedItem}");
                System.Diagnostics.Debug.WriteLine($"Final SelectedValue: {cmbCategory.SelectedValue}");
                System.Diagnostics.Debug.WriteLine($"==========================");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error setting selected category: {ex.Message}");
            }
        }
        
        private void UpdatePreview(object sender, EventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                // Just log the error, don't disrupt the UI
                Console.WriteLine($"Error updating preview: {ex.Message}");
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
        
        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
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
                System.Diagnostics.Debug.WriteLine($"=== CATEGORY DEBUG INFO (Edit) ===");
                System.Diagnostics.Debug.WriteLine($"SelectedIndex: {cmbCategory.SelectedIndex}");
                System.Diagnostics.Debug.WriteLine($"SelectedItem: {cmbCategory.SelectedItem}");
                System.Diagnostics.Debug.WriteLine($"SelectedValue: {cmbCategory.SelectedValue}");
                
                if (cmbCategory.SelectedItem is Category selectedCategory)
                {
                    System.Diagnostics.Debug.WriteLine($"Selected Category Name: {selectedCategory.CategoryName}");
                    System.Diagnostics.Debug.WriteLine($"Selected Category ID: {selectedCategory.CategoryId}");
                }
                System.Diagnostics.Debug.WriteLine($"===================================");
                
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
                
                // Create updated Product object, preserving the original ID
                var updatedProduct = new Product
                {
                    ProductId = _originalProduct.ProductId,
                    ProductName = txtProductName.Text.Trim(),
                    CategoryId = categoryId,
                    UnitPrice = unitPrice,
                    UnitsInStock = unitsInStock,
                    // Preserve other properties from original product
                    SupplierId = _originalProduct.SupplierId,
                    QuantityPerUnit = _originalProduct.QuantityPerUnit,
                    UnitsOnOrder = _originalProduct.UnitsOnOrder,
                    ReorderLevel = _originalProduct.ReorderLevel,
                    Discontinued = _originalProduct.Discontinued
                };
                
                // Update in database
                _productService.UpdateProduct(updatedProduct);
                
                // Notify parent window
                ProductUpdated?.Invoke(this, new ProductUpdatedEventArgs(updatedProduct));
                
                // Show success message
                MessageBox.Show("Product updated successfully!", "Success", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
                
                // Close dialog
                this.Close();
            }
            catch (Exception ex)
            {
                ShowError($"Error updating product: {ex.Message}");
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
    
    // Event args for Product updated event
    public class ProductUpdatedEventArgs : EventArgs
    {
        public Product UpdatedProduct { get; private set; }
        
        public ProductUpdatedEventArgs(Product product)
        {
            UpdatedProduct = product;
        }
    }
}
