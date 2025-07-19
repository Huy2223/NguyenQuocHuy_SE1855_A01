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
    /// Interaction logic for EditCustomer.xaml
    /// </summary>
    public partial class EditCustomer : Window
    {
        private readonly CustomerEntity _customer;
        private readonly ICustomerService _customerService;
        
        // Event to notify parent window when a Customer is updated
        public event EventHandler<CustomerUpdatedEventArgs>? CustomerUpdated;
        
        public EditCustomer(CustomerEntity customer)
        {
            InitializeComponent();
            
            // Store the Customer
            _customer = customer ?? throw new ArgumentNullException(nameof(customer));
            
            // Initialize service
            _customerService = new CustomerService();
            
            // Load Customer data into form
            LoadCustomerData();
            
            // Set focus to company name field
            this.Loaded += (s, e) => txtCompanyName.Focus();
        }
        
        private void LoadCustomerData()
        {
            if (_customer == null)
                return;
                
            txtCompanyName.Text = _customer.CompanyName;
            txtContactName.Text = _customer.ContactName;
            txtContactTitle.Text = _customer.ContactTitle;
            txtPhone.Text = _customer.Phone;
            txtAddress.Text = _customer.Address;
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
            
            if (!Regex.IsMatch(txtPhone.Text, @"^\d+$"))
            {
                ShowError("Phone Number must contain only digits.");
                txtPhone.Focus();
                return;
            }
            
            try
            {
                // Check if phone exists and belongs to another Customer
                var existingCustomer = _customerService.GetCustomerByPhone(txtPhone.Text.Trim());
                if (existingCustomer != null && existingCustomer.CustomerId != _customer.CustomerId)
                {
                    ShowError("This phone number is already registered with another customer.");
                    txtPhone.Focus();
                    return;
                }
                
                // Create updated Customer object
                var updatedCustomer = new CustomerEntity
                {
                    CustomerId = _customer.CustomerId,
                    CompanyName = txtCompanyName.Text.Trim(),
                    ContactName = txtContactName.Text.Trim(),
                    ContactTitle = txtContactTitle.Text.Trim(),
                    Phone = txtPhone.Text.Trim(),
                    Address = txtAddress.Text.Trim()
                };
                
                // Save to database
                _customerService.UpdateCustomer(updatedCustomer);
                
                // Notify parent window
                CustomerUpdated?.Invoke(this, new CustomerUpdatedEventArgs(updatedCustomer));
                
                // Show success message
                MessageBox.Show("Customer updated successfully!", "Success", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
                
                // Close dialog
                this.Close();
            }
            catch (Exception ex)
            {
                ShowError($"Error updating customer: {ex.Message}");
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
    
    // Event args for Customer updated event
    public class CustomerUpdatedEventArgs : EventArgs
    {
        public CustomerEntity UpdatedCustomer { get; private set; }
        
        public CustomerUpdatedEventArgs(CustomerEntity customer)
        {
            UpdatedCustomer = customer;
        }
    }
}
