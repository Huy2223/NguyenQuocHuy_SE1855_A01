using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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

            // Load Products
            LoadProducts();
        }

        private void LoadProducts(string searchTerm = "")
        {
            try
            {
                IEnumerable<Product> products;

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
                    ProductId = p.ProductId,
                    ProductName = p.ProductName ?? "",
                    CategoryId = p.CategoryId,
                    CategoryName = GetCategoryName(p.CategoryId ?? 0),
                    UnitPrice = p.UnitPrice,
                    UnitsInStock = p.UnitsInStock,
                    Discontinued = p.Discontinued
                }).ToList();

                // Update ObservableCollection with the new data
                _products.Clear();
                foreach (var product in productsWithCategories)
                {
                    _products.Add(product);
                }

                UpdateStatusText();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading products: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateStatusText()
        {
            txtProductStatus.Text = $"Total Products: {_products.Count}";
        }

        private string GetCategoryName(int categoryId)
        {
            try
            {
                if (categoryId <= 0) return "No Category";
                
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
            LoadProducts(txtSearchProduct.Text?.Trim() ?? "");
        }

        private void TxtSearchProduct_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                LoadProducts(txtSearchProduct.Text?.Trim() ?? "");
            }
        }

        private void BtnAddProduct_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Create and show the AddNewProduct dialog
                var addProductDialog = new AddNewProduct();

                // Subscribe to the ProductAdded event
                addProductDialog.ProductAdded += (s, args) =>
                {
                    // Create a ProductViewModel for the new product
                    var newProductVM = new ProductViewModel
                    {
                        ProductId = args.NewProduct.ProductId,
                        ProductName = args.NewProduct.ProductName ?? "",
                        CategoryId = args.NewProduct.CategoryId,
                        CategoryName = GetCategoryName(args.NewProduct.CategoryId ?? 0),
                        UnitPrice = args.NewProduct.UnitPrice,
                        UnitsInStock = args.NewProduct.UnitsInStock,
                        Discontinued = args.NewProduct.Discontinued
                    };

                    // Add the new product to the ObservableCollection
                    _products.Add(newProductVM);
                    UpdateStatusText();
                };

                addProductDialog.Owner = this;
                addProductDialog.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening add product dialog: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnEditProduct_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var productVM = (sender as Button)?.DataContext as ProductViewModel;
                if (productVM == null)
                {
                    MessageBox.Show("No product selected for editing.", "Warning",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Convert to regular product
                var product = new Product
                {
                    ProductId = productVM.ProductId,
                    ProductName = productVM.ProductName,
                    CategoryId = productVM.CategoryId,
                    UnitPrice = productVM.UnitPrice,
                    UnitsInStock = productVM.UnitsInStock,
                    Discontinued = productVM.Discontinued
                };

                // Create and show the EditProduct dialog
                var editProductDialog = new EditProduct(product);

                // Subscribe to the ProductUpdated event
                editProductDialog.ProductUpdated += (s, args) =>
                {
                    // Update the ProductViewModel with the changes
                    productVM.ProductName = args.UpdatedProduct.ProductName ?? "";
                    productVM.CategoryId = args.UpdatedProduct.CategoryId;
                    productVM.CategoryName = GetCategoryName(args.UpdatedProduct.CategoryId ?? 0);
                    productVM.UnitPrice = args.UpdatedProduct.UnitPrice;
                    productVM.UnitsInStock = args.UpdatedProduct.UnitsInStock;
                    productVM.Discontinued = args.UpdatedProduct.Discontinued;

                    // The ObservableCollection with INotifyPropertyChanged will handle UI updates automatically
                };

                editProductDialog.Owner = this;
                editProductDialog.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error editing product: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnDeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var productVM = (sender as Button)?.DataContext as ProductViewModel;
                if (productVM == null)
                {
                    MessageBox.Show("No product selected for deletion.", "Warning",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var result = MessageBox.Show(
                    $"Are you sure you want to delete product '{productVM.ProductName}'?\n\nThis action cannot be undone.",
                    "Confirm Delete", 
                    MessageBoxButton.YesNo, 
                    MessageBoxImage.Warning,
                    MessageBoxResult.No);

                if (result == MessageBoxResult.Yes)
                {
                    _productService.DeleteProduct(productVM.ProductId);

                    // Remove from ObservableCollection (UI updates automatically)
                    _products.Remove(productVM);
                    UpdateStatusText();

                    // Show success message
                    MessageBox.Show("Product deleted successfully.", "Success",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting product: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DgProducts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Optional: Handle selection changed event
            // Could be used to enable/disable edit/delete buttons based on selection
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
                MessageBox.Show($"Error returning to dashboard: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            // Clear search and reload all products
            txtSearchProduct.Text = "";
            LoadProducts();
        }
    }

    /// <summary>
    /// ViewModel for Product to display Category names with proper data binding support
    /// </summary>
    public class ProductViewModel : INotifyPropertyChanged
    {
        private int _productId;
        private string _productName = "";
        private int? _categoryId;
        private string _categoryName = "";
        private decimal? _unitPrice;
        private int? _unitsInStock;
        private bool _discontinued;

        public int ProductId
        {
            get => _productId;
            set
            {
                if (_productId != value)
                {
                    _productId = value;
                    OnPropertyChanged();
                }
            }
        }

        public string ProductName
        {
            get => _productName;
            set
            {
                if (_productName != value)
                {
                    _productName = value ?? "";
                    OnPropertyChanged();
                }
            }
        }

        public int? CategoryId
        {
            get => _categoryId;
            set
            {
                if (_categoryId != value)
                {
                    _categoryId = value;
                    OnPropertyChanged();
                }
            }
        }

        public string CategoryName
        {
            get => _categoryName;
            set
            {
                if (_categoryName != value)
                {
                    _categoryName = value ?? "";
                    OnPropertyChanged();
                }
            }
        }

        public decimal? UnitPrice
        {
            get => _unitPrice;
            set
            {
                if (_unitPrice != value)
                {
                    _unitPrice = value;
                    OnPropertyChanged();
                }
            }
        }

        public int? UnitsInStock
        {
            get => _unitsInStock;
            set
            {
                if (_unitsInStock != value)
                {
                    _unitsInStock = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool Discontinued
        {
            get => _discontinued;
            set
            {
                if (_discontinued != value)
                {
                    _discontinued = value;
                    OnPropertyChanged();
                }
            }
        }

        // For backward compatibility with existing UI code if needed
        public int ProductID => ProductId;
        public int CategoryID => CategoryId ?? 0;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

