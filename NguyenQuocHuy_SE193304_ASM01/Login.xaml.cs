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
using NguyenQuocHuyWPF;

namespace NguyenQuocHuy_SE193304_ASM01
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        private readonly IEmployeeService _employeeService;
        private readonly ICustomerService _customerService;

        public Login()
        {
            // Initialize the UI components first
            InitializeComponent();

            // Initialize services
            _employeeService = new EmployeeService();
            _customerService = new CustomerService();

            // Add the Loaded event to set up UI after everything is initialized
            this.Loaded += Login_Loaded;

            // Add visual feedback on textboxes
            txtUsername.GotFocus += (s, e) => AnimateControlOnFocus(txtUsername);
            txtPassword.GotFocus += (s, e) => AnimateControlOnFocus(txtPassword);
            txtPhone.GotFocus += (s, e) => AnimateControlOnFocus(txtPhone);
        }

        private void Login_Loaded(object sender, RoutedEventArgs e)
        {
            // Ensure the correct panel is shown initially
            if (rbEmployee != null && rbEmployee.IsChecked == true)
            {
                ShowEmployeePanel();
            }
            else if (rbCustomer != null && rbCustomer.IsChecked == true)
            {
                ShowCustomerPanel();
            }
            else
            {
                // Default to employee login if neither is checked
                if (rbEmployee != null)
                {
                    rbEmployee.IsChecked = true;
                    ShowEmployeePanel();
                }
            }

            // Set up Enter key handling
            this.PreviewKeyDown += (s, ev) =>
            {
                if (ev.Key == Key.Enter)
                {
                    if (EmployeeLoginPanel != null && EmployeeLoginPanel.Visibility == Visibility.Visible)
                    {
                        BtnEmployeeLogin_Click(s, ev);
                    }
                    else if (CustomerLoginPanel != null && CustomerLoginPanel.Visibility == Visibility.Visible)
                    {
                        BtnCustomerLogin_Click(s, ev);
                    }
                }
            };
        }

        private void AnimateControlOnFocus(Control control)
        {
            // You could add animation here if desired
            // For now, just ensuring focus visual works with the custom template
        }

        private void RbEmployee_Checked(object sender, RoutedEventArgs e)
        {
            ShowEmployeePanel();
        }

        private void ShowEmployeePanel()
        {
            // Use null-conditional operators to avoid NullReferenceException
            if (EmployeeLoginPanel != null && CustomerLoginPanel != null)
            {
                EmployeeLoginPanel.Visibility = Visibility.Visible;
                CustomerLoginPanel.Visibility = Visibility.Collapsed;
                HideError();
                txtUsername?.Focus();
            }
        }

        private void RbCustomer_Checked(object sender, RoutedEventArgs e)
        {
            ShowCustomerPanel();
        }

        private void ShowCustomerPanel()
        {
            // Use null-conditional operators to avoid NullReferenceException
            if (EmployeeLoginPanel != null && CustomerLoginPanel != null)
            {
                EmployeeLoginPanel.Visibility = Visibility.Collapsed;
                CustomerLoginPanel.Visibility = Visibility.Visible;
                HideError();
                txtPhone?.Focus();
            }
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

        private void BtnEmployeeLogin_Click(object sender, RoutedEventArgs e)
        {
            if (txtUsername == null || txtPassword == null || btnEmployeeLogin == null)
                return;

            string username = txtUsername.Text.Trim();
            string password = txtPassword.Password.Trim();

            // Validate input
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ShowError("Please enter both username and password.");
                return;
            }

            try
            {
                // Show loading state
                btnEmployeeLogin.IsEnabled = false;
                btnEmployeeLogin.Content = "LOGGING IN...";

                // Authenticate employee
                Employees employee = _employeeService.Login(username, password);

                if (employee != null)
                {
                    // Authentication successful
                    HideError();

                    // Store logged in user information in application-wide state
                    App.Current.Properties["CurrentUser"] = employee;
                    App.Current.Properties["UserType"] = "Employee";

                    // Brief welcome message
                    MessageBox.Show($"Welcome, {employee.Name}!\n" +
                                    $"Role: {(employee.IsAdmin ? "Administrator" : employee.JobTitle)}",
                                    "Login Successful", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Open the appropriate window based on user role
                    if (employee.IsAdmin)
                    {
                        // Open Admin Dashboard
                        AdminDashBoard adminDashboard = new AdminDashBoard(employee);
                        adminDashboard.Show();
                        this.Close();
                    }
                    else
                    {
                        // TODO: Open Employee Window
                        MessageBox.Show("Employee interface will be implemented later.",
                            "Coming Soon", MessageBoxButton.OK, MessageBoxImage.Information);

                        // For now, just reset the form
                        ResetLoginForm();
                    }
                }
                else
                {
                    // Authentication failed
                    ShowError("Invalid username or password. Please try again.");
                    txtPassword.Password = string.Empty;
                    txtPassword.Focus();
                }
            }
            catch (Exception ex)
            {
                ShowError($"An error occurred: {ex.Message}");
            }
            finally
            {
                // Reset button state
                btnEmployeeLogin.IsEnabled = true;
                btnEmployeeLogin.Content = "LOGIN";
            }
        }

        private void BtnCustomerLogin_Click(object sender, RoutedEventArgs e)
        {
            if (txtPhone == null || btnCustomerLogin == null)
                return;

            string phone = txtPhone.Text.Trim();

            // Validate input
            if (string.IsNullOrEmpty(phone))
            {
                ShowError("Please enter your phone number.");
                return;
            }

            // Validate phone format
            if (phone.Length != 10 || !IsDigitsOnly(phone))
            {
                ShowError("Phone number must be exactly 10 digits.");
                return;
            }

            try
            {
                // Show loading state
                btnCustomerLogin.IsEnabled = false;
                btnCustomerLogin.Content = "LOGGING IN...";

                // Authenticate customer by phone
                Customers customer = _customerService.AuthenticateByPhone(phone);

                if (customer != null)
                {
                    // Authentication successful
                    HideError();

                    // Store logged in user information in application-wide state
                    App.Current.Properties["CurrentUser"] = customer;
                    App.Current.Properties["UserType"] = "Customer";

                    // Brief welcome message
                    MessageBox.Show($"Welcome, {customer.ContactName}!\n" +
                                    $"Company: {customer.CompanyName}",
                                    "Login Successful", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Open Customer Window
                    CustomerWindow customerWindow = new CustomerWindow(customer);
                    customerWindow.Show();
                    this.Close();
                }
                else
                {
                    // Authentication failed
                    ShowError("Invalid phone number. Please try again.");
                    txtPhone.Focus();
                }
            }
            catch (Exception ex)
            {
                ShowError($"An error occurred: {ex.Message}");
            }
            finally
            {
                // Reset button state
                btnCustomerLogin.IsEnabled = true;
                btnCustomerLogin.Content = "LOGIN AS CUSTOMER";
            }
        }

        // Legacy method for backward compatibility
        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            BtnEmployeeLogin_Click(sender, e);
        }

        private void ShowError(string message)
        {
            if (txtError == null) return;

            txtError.Text = message;
            txtError.Visibility = Visibility.Visible;
        }

        private void HideError()
        {
            if (txtError == null) return;

            txtError.Text = string.Empty;
            txtError.Visibility = Visibility.Collapsed;
        }

        private void ResetLoginForm()
        {
            if (txtUsername != null) txtUsername.Text = string.Empty;
            if (txtPassword != null) txtPassword.Password = string.Empty;
            if (txtPhone != null) txtPhone.Text = string.Empty;

            HideError();

            if (EmployeeLoginPanel != null && EmployeeLoginPanel.Visibility == Visibility.Visible)
            {
                txtUsername?.Focus();
            }
            else if (CustomerLoginPanel != null && CustomerLoginPanel.Visibility == Visibility.Visible)
            {
                txtPhone?.Focus();
            }
        }
    }
}
