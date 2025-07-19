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
using CustomerEntity = BusinessObject.Customer;

namespace NguyenQuocHuyWPF.Admin
{
    /// <summary>
    /// Interaction logic for ManageCustomer.xaml
    /// </summary>
    public partial class ManageCustomer : Window
    {
        private readonly ICustomerService _customerService;
        private ObservableCollection<CustomerEntity> _customers;

        public ManageCustomer()
        {
            InitializeComponent();

            // Initialize service
            _customerService = new CustomerService();

            // Initialize observable collection
            _customers = new ObservableCollection<CustomerEntity>();
            dgCustomers.ItemsSource = _customers;

            // Load customers when window is loaded
            this.Loaded += (s, e) => LoadAllCustomers();
        }

        private void LoadAllCustomers()
        {
            try
            {
                var customers = _customerService.GetAllCustomers();
                
                // Update ObservableCollection with the new data
                _customers.Clear();
                foreach (var customer in customers)
                {
                    _customers.Add(customer);
                }

                txtCustomerStatus.Text = $"?? Total customers: {_customers.Count} | Ready to manage customer data";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading customers: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadCustomers(string searchTerm = "")
        {
            try
            {
                IEnumerable<CustomerEntity> customers;

                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    customers = _customerService.GetAllCustomers();
                }
                else
                {
                    // Search in both name and company
                    var customersByName = _customerService.SearchCustomersByName(searchTerm);
                    var customersByCompany = _customerService.SearchCustomersByCompany(searchTerm);
                    
                    // Combine and remove duplicates using LINQ
                    customers = customersByName.Concat(customersByCompany)
                                               .GroupBy(c => c.CustomerId)
                                               .Select(g => g.First())
                                               .ToList();
                }

                // Update ObservableCollection with the new data
                _customers.Clear();
                foreach (var customer in customers)
                {
                    _customers.Add(customer);
                }

                txtCustomerStatus.Text = string.IsNullOrWhiteSpace(searchTerm) 
                    ? $"?? Total customers: {_customers.Count} | Ready to manage customer data"
                    : $"?? Search results: {_customers.Count} customers found for '{searchTerm}'";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading customers: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnSearchCustomer_Click(object sender, RoutedEventArgs e)
        {
            LoadCustomers(txtSearchCustomer.Text.Trim());
        }

        private void TxtSearchCustomer_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                LoadCustomers(txtSearchCustomer.Text.Trim());
            }
        }

        private void BtnRefreshCustomers_Click(object sender, RoutedEventArgs e)
        {
            txtSearchCustomer.Text = string.Empty;
            LoadAllCustomers();
        }

        private void BtnAddCustomer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var addCustomerWindow = new AddNewCustomer();
                
                // Handle customer added event
                addCustomerWindow.CustomerAdded += (s, args) =>
                {
                    LoadAllCustomers(); // Refresh the list
                };
                
                addCustomerWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening add customer window: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnEditCustomer_Click(object sender, RoutedEventArgs e)
        {
            var customer = (sender as Button)?.DataContext as CustomerEntity;
            if (customer != null)
            {
                try
                {
                    var editCustomerWindow = new EditCustomer(customer);
                    
                    // Handle customer updated event
                    editCustomerWindow.CustomerUpdated += (s, args) =>
                    {
                        LoadAllCustomers(); // Refresh the list
                    };
                    
                    editCustomerWindow.ShowDialog();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error opening edit customer window: {ex.Message}", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnDeleteCustomer_Click(object sender, RoutedEventArgs e)
        {
            var customer = (sender as Button)?.DataContext as CustomerEntity;
            if (customer != null)
            {
                var result = MessageBox.Show(
                    $"?? Are you sure you want to delete customer '{customer.CompanyName}'?\n\n" +
                    $"Contact: {customer.ContactName}\n" +
                    $"Phone: {customer.Phone}\n\n" +
                    "?? This action will also delete all orders associated with this customer!\n" +
                    "This action cannot be undone!",
                    "Confirm Delete Customer", 
                    MessageBoxButton.YesNo, 
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        _customerService.DeleteCustomer(customer.CustomerId);
                        
                        // Remove from ObservableCollection (UI updates automatically)
                        _customers.Remove(customer);
                        
                        txtCustomerStatus.Text = $"? Customer deleted | Remaining customers: {_customers.Count}";

                        // Show success message
                        MessageBox.Show($"? Customer '{customer.CompanyName}' has been successfully deleted.", "Delete Successful",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"? Error deleting customer: {ex.Message}", "Delete Failed",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void DgCustomers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Optional: Handle selection changed event for future features
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
}
