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
using NguyenQuocHuyWPF.Customer;

namespace NguyenQuocHuyWPF.Admin
{
    /// <summary>
    /// Interaction logic for ManageCustomer.xaml
    /// </summary>
    public partial class ManageCustomer : Window
    {
        private readonly ICustomerService _customerService;
        private ObservableCollection<Customers> _customers;
        
        public ManageCustomer()
        {
            InitializeComponent();
            
            // Initialize service
            _customerService = new CustomerService();
            
            // Initialize observable collection
            _customers = new ObservableCollection<Customers>();
            dgCustomers.ItemsSource = _customers;
            
            // Load customers
            LoadCustomers();
        }
        
        private void LoadCustomers(string searchTerm = "")
        {
            try
            {
                IEnumerable<Customers> customers;
                
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    customers = _customerService.GetAllCustomers();
                }
                else
                {
                    // Try to search by name first, then by company if no results
                    customers = _customerService.SearchCustomersByName(searchTerm);
                    
                    if (!customers.Any())
                    {
                        customers = _customerService.SearchCustomersByCompany(searchTerm);
                    }
                }
                
                // Update ObservableCollection with the new data
                _customers.Clear();
                foreach (var customer in customers)
                {
                    _customers.Add(customer);
                }
                
                txtCustomerStatus.Text = $"Total customers: {_customers.Count}";
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
        
        private void BtnAddCustomer_Click(object sender, RoutedEventArgs e)
        {
            // Create and show the AddNewCustomer dialog
            var addCustomerDialog = new AddNewCustomer();
            
            // Subscribe to the CustomerAdded event
            addCustomerDialog.CustomerAdded += (s, args) => 
            {
                // Add the new customer to the ObservableCollection
                _customers.Add(args.NewCustomer);
                txtCustomerStatus.Text = $"Total customers: {_customers.Count}";
            };
            
            addCustomerDialog.Owner = this;
            addCustomerDialog.ShowDialog();
        }
        
        private void BtnEditCustomer_Click(object sender, RoutedEventArgs e)
        {
            var customer = (sender as Button)?.DataContext as Customers;
            if (customer != null)
            {
                // Create and show the EditCustomer dialog
                var editCustomerDialog = new EditCustomer(customer);
                
                // Subscribe to the CustomerUpdated event
                editCustomerDialog.CustomerUpdated += (s, args) =>
                {
                    // Find and update the customer in the ObservableCollection
                    var index = _customers.IndexOf(customer);
                    if (index >= 0)
                    {
                        _customers[index] = args.UpdatedCustomer;
                    }
                };
                
                editCustomerDialog.Owner = this;
                editCustomerDialog.ShowDialog();
            }
        }

        private void BtnEditProfile_Click(object sender, RoutedEventArgs e)
        {
            var customer = (sender as Button)?.DataContext as Customers;
            if (customer != null)
            {
                try
                {
                    // Navigate to EditProfile window, specifying that we're coming from admin panel
                    EditProfile editProfileWindow = new EditProfile(customer, true);
                    
                    // Set up event handler to refresh the customer list when returning
                    editProfileWindow.Closed += (s, args) => 
                    {
                        // Refresh the customer data
                        LoadCustomers();
                        
                        // Show this window again
                        this.Show();
                    };
                    
                    // Show the edit window and hide this one
                    editProfileWindow.Show();
                    this.Hide();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error opening profile editor: {ex.Message}", 
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        
        private void BtnDeleteCustomer_Click(object sender, RoutedEventArgs e)
        {
            var customer = (sender as Button)?.DataContext as Customers;
            if (customer != null)
            {
                if (MessageBox.Show($"Are you sure you want to delete customer '{customer.CompanyName}'?", 
                    "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    try
                    {
                        _customerService.DeleteCustomer(customer.CustomerID);
                        
                        // Remove from ObservableCollection (UI updates automatically)
                        _customers.Remove(customer);
                        txtCustomerStatus.Text = $"Total customers: {_customers.Count}";
                        
                        // Show success message
                        MessageBox.Show("Customer deleted successfully.", "Success", 
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error deleting customer: {ex.Message}", "Error", 
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
        
        private void DgCustomers_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
