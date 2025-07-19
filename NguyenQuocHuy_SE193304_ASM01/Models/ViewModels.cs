using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NguyenQuocHuyWPF.Models
{
    public class OrderItemViewModel : INotifyPropertyChanged
    {
        private int _productId;
        private string _productName = string.Empty;
        private decimal _unitPrice;
        private int _quantity;
        private float _discount;
        private decimal _total;

        public int ProductId
        {
            get => _productId;
            set { _productId = value; OnPropertyChanged(); }
        }

        public string ProductName
        {
            get => _productName;
            set { _productName = value ?? string.Empty; OnPropertyChanged(); }
        }

        public decimal UnitPrice
        {
            get => _unitPrice;
            set 
            { 
                _unitPrice = value; 
                OnPropertyChanged(); 
                UpdateTotal();
            }
        }

        public int Quantity
        {
            get => _quantity;
            set 
            { 
                _quantity = value; 
                OnPropertyChanged(); 
                UpdateTotal();
            }
        }

        public float Discount
        {
            get => _discount;
            set 
            { 
                _discount = value; 
                OnPropertyChanged(); 
                UpdateTotal();
            }
        }

        public decimal Total
        {
            get => _total;
            private set 
            { 
                _total = value; 
                OnPropertyChanged(); 
            }
        }

        // For backward compatibility with existing UI code
        public int ProductID
        {
            get => ProductId;
            set => ProductId = value;
        }

        public OrderItemViewModel()
        {
            UpdateTotal();
        }

        public void UpdateTotal()
        {
            Total = UnitPrice * Quantity * (1 - (decimal)Discount);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName ?? string.Empty));
        }
    }

    public class CustomerViewModel : INotifyPropertyChanged
    {
        private int _customerId;
        private string _companyName = string.Empty;
        private string _contactName = string.Empty;
        private string _contactTitle = string.Empty;
        private string _address = string.Empty;
        private string _phone = string.Empty;

        public int CustomerId
        {
            get => _customerId;
            set { _customerId = value; OnPropertyChanged(); }
        }

        public string CompanyName
        {
            get => _companyName;
            set { _companyName = value ?? string.Empty; OnPropertyChanged(); }
        }

        public string ContactName
        {
            get => _contactName;
            set { _contactName = value ?? string.Empty; OnPropertyChanged(); }
        }

        public string ContactTitle
        {
            get => _contactTitle;
            set { _contactTitle = value ?? string.Empty; OnPropertyChanged(); }
        }

        public string Address
        {
            get => _address;
            set { _address = value ?? string.Empty; OnPropertyChanged(); }
        }

        public string Phone
        {
            get => _phone;
            set { _phone = value ?? string.Empty; OnPropertyChanged(); }
        }

        // For backward compatibility
        public int CustomerID
        {
            get => CustomerId;
            set => CustomerId = value;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName ?? string.Empty));
        }
    }

    public class OrderViewModel : INotifyPropertyChanged
    {
        private int _orderId;
        private int _customerId;
        private int _employeeId;
        private DateTime _orderDate;
        private string _customerName = string.Empty;
        private string _employeeName = string.Empty;

        public int OrderId
        {
            get => _orderId;
            set { _orderId = value; OnPropertyChanged(); }
        }

        public int CustomerId
        {
            get => _customerId;
            set { _customerId = value; OnPropertyChanged(); }
        }

        public int EmployeeId
        {
            get => _employeeId;
            set { _employeeId = value; OnPropertyChanged(); }
        }

        public DateTime OrderDate
        {
            get => _orderDate;
            set { _orderDate = value; OnPropertyChanged(); }
        }

        public string CustomerName
        {
            get => _customerName;
            set { _customerName = value ?? string.Empty; OnPropertyChanged(); }
        }

        public string EmployeeName
        {
            get => _employeeName;
            set { _employeeName = value ?? string.Empty; OnPropertyChanged(); }
        }

        // For backward compatibility
        public int OrderID
        {
            get => OrderId;
            set => OrderId = value;
        }

        public int CustomerID
        {
            get => CustomerId;
            set => CustomerId = value;
        }

        public int EmployeeID
        {
            get => EmployeeId;
            set => EmployeeId = value;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName ?? string.Empty));
        }
    }

    public class EmployeeViewModel : INotifyPropertyChanged
    {
        private int _employeeId;
        private string _name = string.Empty;
        private string _userName = string.Empty;
        private string _jobTitle = string.Empty;
        private DateTime? _birthDate;
        private DateTime? _hireDate;
        private string _address = string.Empty;

        public int EmployeeId
        {
            get => _employeeId;
            set { _employeeId = value; OnPropertyChanged(); }
        }

        public string Name
        {
            get => _name;
            set { _name = value ?? string.Empty; OnPropertyChanged(); }
        }

        public string UserName
        {
            get => _userName;
            set { _userName = value ?? string.Empty; OnPropertyChanged(); }
        }

        public string JobTitle
        {
            get => _jobTitle;
            set { _jobTitle = value ?? string.Empty; OnPropertyChanged(); }
        }

        public DateTime? BirthDate
        {
            get => _birthDate;
            set { _birthDate = value; OnPropertyChanged(); }
        }

        public DateTime? HireDate
        {
            get => _hireDate;
            set { _hireDate = value; OnPropertyChanged(); }
        }

        public string Address
        {
            get => _address;
            set { _address = value ?? string.Empty; OnPropertyChanged(); }
        }

        // For backward compatibility
        public int EmployeeID
        {
            get => EmployeeId;
            set => EmployeeId = value;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName ?? string.Empty));
        }
    }
}