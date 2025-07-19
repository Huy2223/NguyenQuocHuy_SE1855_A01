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
using NguyenQuocHuyWPF.Admin;

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
                // Default to Employee login if neither is checked
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

            // Get values from WPF textboxes
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
                btnEmployeeLogin.Content = "🔄 LOGGING IN...";

                // Authenticate Employee - EmployeeDAO.Authenticate() will query database using FirstOrDefault
                var employeeResult = _employeeService.Login(username, password);

                if (employeeResult != null)
                {
                    // Authentication successful - employeeResult contains actual data from database
                    HideError();

                    // Store logged in user information
                    App.Current.Properties["CurrentUser"] = employeeResult;
                    App.Current.Properties["UserType"] = "Employee";

                    // Show welcome message - all employees have full access
                    MessageBox.Show($"🎉 Welcome back, {employeeResult.Name}!\n\n" +
                                    $"👔 Role: {employeeResult.JobTitle}\n" +
                                    $"🚀 You have full access to the system.",
                                    "Login Successful", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Open Employee Dashboard - all employees get full access (previously called AdminDashBoard)
                    var employeeDashboard = new AdminDashBoard(employeeResult);
                    employeeDashboard.Show();
                    this.Close();
                }
                else
                {
                    // Authentication failed
                    ShowError("Invalid username or password. Please check your credentials and try again.");
                    txtPassword.Password = string.Empty;
                    txtPassword.Focus();
                }
            }
            catch (Exception ex)
            {
                ShowError($"An error occurred during login: {ex.Message}");
            }
            finally
            {
                // Reset button state
                btnEmployeeLogin.IsEnabled = true;
                btnEmployeeLogin.Content = "🚀 LOGIN AS EMPLOYEE";
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

            // Validate phone contains only digits (removed 10-digit requirement)
            if (!IsDigitsOnly(phone))
            {
                ShowError("Phone number must contain only digits.");
                return;
            }

            try
            {
                // Show loading state
                btnCustomerLogin.IsEnabled = false;
                btnCustomerLogin.Content = "🔄 LOGGING IN...";

                // Authenticate Customer by phone
                var customerResult = _customerService.AuthenticateByPhone(phone);

                if (customerResult != null)
                {
                    // Authentication successful
                    HideError();

                    // Store logged in user information in application-wide state
                    App.Current.Properties["CurrentUser"] = customerResult;
                    App.Current.Properties["UserType"] = "Customer";

                    // Brief welcome message
                    MessageBox.Show($"🎉 Welcome back, {customerResult.ContactName}!\n\n" +
                                    $"🏢 Company: {customerResult.CompanyName}\n" +
                                    $"📱 Phone: {customerResult.Phone}",
                                    "Login Successful", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Open Customer Window
                    var customerWindow = new CustomerWindow(customerResult);
                    customerWindow.Show();
                    this.Close();
                }
                else
                {
                    // Authentication failed
                    ShowError("Phone number not found in our system. Please check your number and try again.");
                    txtPhone.Focus();
                }
            }
            catch (Exception ex)
            {
                ShowError($"An error occurred during login: {ex.Message}");
            }
            finally
            {
                // Reset button state
                btnCustomerLogin.IsEnabled = true;
                btnCustomerLogin.Content = "🛒 LOGIN AS CUSTOMER";
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

