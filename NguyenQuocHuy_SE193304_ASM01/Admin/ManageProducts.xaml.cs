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

namespace NguyenQuocHuyWPF.Admin
{
    /// <summary>
    /// Interaction logic for ManageProducts.xaml
    /// </summary>
    public partial class ManageProducts : Window
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private ObservableCollection<ProductViewModel> _products;
        
        public ManageProducts()
        {
            InitializeComponent();
            
            // Initialize services
            _productService = new ProductService();
            _categoryService = new CategoryService();
            
            // Initialize observable collection
            _products = new ObservableCollection<ProductViewModel>();
            dgProducts.ItemsSource = _products;
            
            // Load products
            LoadProducts();
        }
        
        private void LoadProducts(string searchTerm = "")
        {
            try
            {
                IEnumerable<Products> products;
                
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    products = _productService.GetAllProducts();
                }
                else
                {
                    products = _productService.SearchProductsByName(searchTerm);
                }
                
                // Load products with category names
                var productsWithCategories = products.Select(p => new ProductViewModel
                {
                    ProductID = p.ProductID,
                    ProductName = p.ProductName,
                    CategoryID = p.CategoryID,
                    CategoryName = GetCategoryName(p.CategoryID),
                    UnitPrice = p.UnitPrice,
                    UnitsInStock = p.UnitsInStock
                }).ToList();
                
                // Update ObservableCollection with the new data
                _products.Clear();
                foreach (var product in productsWithCategories)
                {
                    _products.Add(product);
                }
                
                txtProductStatus.Text = $"Total products: {_products.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading products: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private string GetCategoryName(int categoryId)
        {
            try
            {
                var category = _categoryService.GetCategoryByID(categoryId);
                return category?.CategoryName ?? "Unknown";
            }
            catch
            {
                return "Unknown";
            }
        }
        
        private void BtnSearchProduct_Click(object sender, RoutedEventArgs e)
        {
            LoadProducts(txtSearchProduct.Text.Trim());
        }
        
        private void TxtSearchProduct_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                LoadProducts(txtSearchProduct.Text.Trim());
            }
        }
        
        private void BtnAddProduct_Click(object sender, RoutedEventArgs e)
        {
            // Create and show the AddNewProduct dialog
            var addProductDialog = new AddNewProduct();
            
            // Subscribe to the ProductAdded event
            addProductDialog.ProductAdded += (s, args) => 
            {
                // Create a ProductViewModel for the new product
                var newProductVM = new ProductViewModel
                {
                    ProductID = args.NewProduct.ProductID,
                    ProductName = args.NewProduct.ProductName,
                    CategoryID = args.NewProduct.CategoryID,
                    CategoryName = GetCategoryName(args.NewProduct.CategoryID),
                    UnitPrice = args.NewProduct.UnitPrice,
                    UnitsInStock = args.NewProduct.UnitsInStock
                };
                
                // Add the new product to the ObservableCollection
                _products.Add(newProductVM);
                txtProductStatus.Text = $"Total products: {_products.Count}";
            };
            
            addProductDialog.Owner = this;
            addProductDialog.ShowDialog();
        }
        
        private void BtnEditProduct_Click(object sender, RoutedEventArgs e)
        {
            var productVM = (sender as Button)?.DataContext as ProductViewModel;
            if (productVM != null)
            {
                // Convert to regular product
                var product = new Products
                {
                    ProductID = productVM.ProductID,
                    ProductName = productVM.ProductName,
                    CategoryID = productVM.CategoryID,
                    UnitPrice = productVM.UnitPrice,
                    UnitsInStock = productVM.UnitsInStock
                };
                
                // Create and show the EditProduct dialog
                var editProductDialog = new EditProduct(product);
                
                // Subscribe to the ProductUpdated event
                editProductDialog.ProductUpdated += (s, args) =>
                {
                    // Update the ProductViewModel with the changes
                    productVM.ProductName = args.UpdatedProduct.ProductName;
                    productVM.CategoryID = args.UpdatedProduct.CategoryID;
                    productVM.CategoryName = GetCategoryName(args.UpdatedProduct.CategoryID);
                    productVM.UnitPrice = args.UpdatedProduct.UnitPrice;
                    productVM.UnitsInStock = args.UpdatedProduct.UnitsInStock;
                    
                    // Force UI refresh
                    dgProducts.Items.Refresh();
                };
                
                editProductDialog.Owner = this;
                editProductDialog.ShowDialog();
            }
        }
        
        private void BtnDeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            var productVM = (sender as Button)?.DataContext as ProductViewModel;
            if (productVM != null)
            {
                if (MessageBox.Show($"Are you sure you want to delete product '{productVM.ProductName}'?", 
                    "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    try
                    {
                        _productService.DeleteProduct(productVM.ProductID);
                        
                        // Remove from ObservableCollection (UI updates automatically)
                        _products.Remove(productVM);
                        txtProductStatus.Text = $"Total products: {_products.Count}";
                        
                        // Show success message
                        MessageBox.Show("Product deleted successfully.", "Success", 
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error deleting product: {ex.Message}", "Error", 
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
        
        private void DgProducts_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
    
    // ViewModel for Products to display category names
    public class ProductViewModel
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; } = "";
        public int CategoryID { get; set; }
        public string CategoryName { get; set; } = "";
        public decimal UnitPrice { get; set; }
        public int UnitsInStock { get; set; }
    }
}
