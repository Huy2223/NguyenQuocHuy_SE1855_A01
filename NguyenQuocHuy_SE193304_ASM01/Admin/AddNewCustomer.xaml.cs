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
using CustomerEntity = BusinessObject.Customer;

namespace NguyenQuocHuyWPF.Admin
{
    /// <summary>
    /// Interaction logic for AddNewCustomer.xaml
    /// </summary>
    public partial class AddNewCustomer : Window
    {
        private readonly ICustomerService _customerService;
        
        // Event to notify parent window when a Customer is added
        public event EventHandler<CustomerAddedEventArgs>? CustomerAdded;
        
        public AddNewCustomer()
        {
            InitializeComponent();
            
            // Initialize service
            _customerService = new CustomerService();
            
            // Set focus to company name field
            this.Loaded += (s, e) => txtCompanyName.Focus();
        }
        
        private void TxtPhone_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Only allow digits in phone number field
            e.Handled = !IsDigitsOnly(e.Text);
        }
        
        private bool IsDigitsOnly(string text)
        {
            return Regex.IsMatch(text, @"^\d+$");
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
            if (string.IsNullOrWhiteSpace(txtCompanyName.Text))
            {
                ShowError("Company Name is required.");
                txtCompanyName.Focus();
                return;
            }
            
            if (string.IsNullOrWhiteSpace(txtContactName.Text))
            {
                ShowError("Contact Name is required.");
                txtContactName.Focus();
                return;
            }
            
            if (string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                ShowError("Phone Number is required.");
                txtPhone.Focus();
                return;
            }
            
            if (!IsDigitsOnly(txtPhone.Text))
            {
                ShowError("Phone Number must contain only digits.");
                txtPhone.Focus();
                return;
            }
            
            try
            {
                // Create new Customer object
                var newCustomer = new CustomerEntity
                {
                    CompanyName = txtCompanyName.Text.Trim(),
                    ContactName = txtContactName.Text.Trim(),
                    ContactTitle = txtContactTitle.Text.Trim(),
                    Phone = txtPhone.Text.Trim(),
                    Address = txtAddress.Text.Trim()
                };
                
                // Save to database
                _customerService.AddCustomer(newCustomer);
                
                // Get the new Customer with ID (for demonstration purposes)
                var savedCustomer = _customerService.GetCustomerByPhone(newCustomer.Phone);
                
                // Notify parent window
                CustomerAdded?.Invoke(this, new CustomerAddedEventArgs(savedCustomer));
                
                // Show success message
                MessageBox.Show("Customer added successfully!", "Success", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
                
                // Close dialog
                this.Close();
            }
            catch (Exception ex)
            {
                ShowError($"Error adding customer: {ex.Message}");
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
    
    // Event args for Customer added event
    public class CustomerAddedEventArgs : EventArgs
    {
        public CustomerEntity NewCustomer { get; private set; }
        
        public CustomerAddedEventArgs(CustomerEntity customer)
        {
            NewCustomer = customer;
        }
    }
}
