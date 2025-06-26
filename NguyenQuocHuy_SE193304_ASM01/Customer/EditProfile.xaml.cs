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

namespace NguyenQuocHuyWPF.Customer
{
    /// <summary>
    /// Interaction logic for EditProfile.xaml
    /// </summary>
    public partial class EditProfile : Window
    {
        private readonly Customers _customer;
        private readonly ICustomerService _customerService;
        private readonly bool _isFromAdminPanel;
        private Action<Customers> _onProfileUpdated;

        public EditProfile(Customers customer, bool isFromAdminPanel = false, Action<Customers> onProfileUpdated = null)
        {
            InitializeComponent();

            // Store the customer
            _customer = customer ?? throw new ArgumentNullException(nameof(customer));

            // Store whether we're coming from admin panel
            _isFromAdminPanel = isFromAdminPanel;

            // Store callback for when profile is updated
            _onProfileUpdated = onProfileUpdated;

            // Initialize service
            _customerService = new CustomerService();

            // Load customer data into form
            LoadCustomerData();

            // Set window properties to behave as a dialog
            this.Owner = Application.Current.MainWindow;
            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            this.ResizeMode = ResizeMode.NoResize;
            this.ShowInTaskbar = false;
        }

        // Default constructor for design-time support
        public EditProfile()
        {
            InitializeComponent();
        }

        private void LoadCustomerData()
        {
            if (_customer == null)
                return;

            txtContactName.Text = _customer.ContactName;
            txtContactTitle.Text = _customer.ContactTitle;
            txtCompanyName.Text = _customer.CompanyName;
            txtPhone.Text = _customer.Phone;
            txtAddress.Text = _customer.Address;
        }

        private void TxtPhone_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Allow only digits in the phone field
            e.Handled = !Regex.IsMatch(e.Text, @"^\d+$");
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate form
                if (string.IsNullOrWhiteSpace(txtContactName.Text) ||
                    string.IsNullOrWhiteSpace(txtCompanyName.Text) ||
                    string.IsNullOrWhiteSpace(txtPhone.Text))
                {
                    MessageBox.Show("Contact name, company name, and phone are required.",
                        "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Phone validation
                if (txtPhone.Text.Length != 10 || !Regex.IsMatch(txtPhone.Text, @"^\d+$"))
                {
                    MessageBox.Show("Phone number must be exactly 10 digits.",
                        "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Check if phone exists and belongs to another customer
                var existingCustomer = _customerService.GetCustomerByPhone(txtPhone.Text.Trim());
                if (existingCustomer != null && existingCustomer.CustomerID != _customer.CustomerID)
                {
                    MessageBox.Show("This phone number is already registered with another customer.",
                        "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Update customer object
                var updatedCustomer = new Customers
                {
                    CustomerID = _customer.CustomerID,
                    ContactName = txtContactName.Text.Trim(),
                    ContactTitle = txtContactTitle.Text.Trim(),
                    CompanyName = txtCompanyName.Text.Trim(),
                    Phone = txtPhone.Text.Trim(),
                    Address = txtAddress.Text.Trim()
                };

                // Update in database
                _customerService.UpdateCustomer(updatedCustomer);

                // Show success message
                MessageBox.Show("Profile updated successfully!", "Success",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                // Notify parent that profile was updated
                _onProfileUpdated?.Invoke(updatedCustomer);

                // Close this window
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating profile: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            // Close this window without saving
            this.Close();
        }
    }
}
