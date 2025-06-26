using System;
using System.Collections.Generic;
using System.Linq;
using BusinessObject;
using Services;

namespace NguyenQuocHuyWPF
{
    public class MockDataService
    {
        private readonly ICustomerService _customerService;
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;
        private readonly IOrderDetailService _orderDetailService;
        
        public MockDataService(
            ICustomerService customerService,
            IProductService productService,
            IOrderService orderService,
            IOrderDetailService orderDetailService)
        {
            _customerService = customerService;
            _productService = productService;
            _orderService = orderService;
            _orderDetailService = orderDetailService;
        }
        
        public void InitializeMockData()
        {
            CreateMockCustomers();
            CreateMockProducts();
            CreateMockOrders();
        }
        
        private void CreateMockCustomers()
        {
            // Check if the customer with phone 1234567890 already exists
            var existingCustomer = _customerService.GetCustomerByPhone("1234567890");
            if (existingCustomer != null)
                return; // Skip if already exists
                
            // Add the requested customer with phone 1234567890
            var specialCustomer = new Customers
            {
                CompanyName = "FPTU Technology",
                ContactName = "John Smith",
                ContactTitle = "CEO",
                Phone = "1234567890",
                Address = "FPTU Campus, District 9, Ho Chi Minh City, Vietnam"
            };
            _customerService.AddCustomer(specialCustomer);
            
            // Add some more sample customers
            var sampleCustomers = new List<Customers>
            {
                new Customers
                {
                    CompanyName = "ABC Corporation",
                    ContactName = "Alice Johnson",
                    ContactTitle = "Purchasing Manager",
                    Phone = "0987654321",
                    Address = "123 Main Street, New York, NY"
                },
                new Customers
                {
                    CompanyName = "XYZ Industries",
                    ContactName = "Bob Williams",
                    ContactTitle = "Sales Director",
                    Phone = "0123456789",
                    Address = "456 Elm Avenue, Los Angeles, CA"
                },
                new Customers
                {
                    CompanyName = "Tech Solutions",
                    ContactName = "Carol Davis",
                    ContactTitle = "CTO",
                    Phone = "0909090909",
                    Address = "789 Oak Road, San Francisco, CA"
                }
            };
            
            foreach (var customer in sampleCustomers)
            {
                try
                {
                    _customerService.AddCustomer(customer);
                }
                catch
                {
                    // Skip if already exists or other error
                }
            }
        }
        
        private void CreateMockProducts()
        {
            var sampleProducts = new List<Products>
            {
                new Products
                {
                    ProductName = "Laptop",
                    CategoryID = 1,
                    UnitPrice = 1200.00m,
                    UnitsInStock = 25
                },
                new Products
                {
                    ProductName = "Smartphone",
                    CategoryID = 1,
                    UnitPrice = 800.00m,
                    UnitsInStock = 50
                },
                new Products
                {
                    ProductName = "Tablet",
                    CategoryID = 1,
                    UnitPrice = 500.00m,
                    UnitsInStock = 30
                },
                new Products
                {
                    ProductName = "Headphones",
                    CategoryID = 2,
                    UnitPrice = 150.00m,
                    UnitsInStock = 100
                },
                new Products
                {
                    ProductName = "Keyboard",
                    CategoryID = 2,
                    UnitPrice = 80.00m,
                    UnitsInStock = 75
                }
            };
            
            foreach (var product in sampleProducts)
            {
                try
                {
                    _productService.AddProduct(product);
                }
                catch
                {
                    // Skip if already exists or other error
                }
            }
        }
        
        private void CreateMockOrders()
        {
            // Get some customers
            var customers = _customerService.GetAllCustomers().ToList();
            if (!customers.Any())
                return;
                
            // Get special customer with phone 1234567890
            var specialCustomer = _customerService.GetCustomerByPhone("1234567890");
            
            // Get some products
            var products = _productService.GetAllProducts().ToList();
            if (!products.Any())
                return;
                
            // Sample order dates
            var orderDates = new[]
            {
                DateTime.Now.AddDays(-1),
                DateTime.Now.AddDays(-3),
                DateTime.Now.AddDays(-7),
                DateTime.Now.AddDays(-15),
                DateTime.Now.AddDays(-30)
            };
            
            // Create orders for the special customer
            if (specialCustomer != null)
            {
                foreach (var date in orderDates.Take(2))
                {
                    try
                    {
                        var order = new Orders
                        {
                            CustomerID = specialCustomer.CustomerID,
                            EmployeeID = 1, // Assuming employee ID 1 exists
                            OrderDate = date
                        };
                        
                        _orderService.AddOrder(order);
                        
                        // Get the last order for this customer to get its ID
                        var newOrder = _orderService.GetOrdersByCustomerID(specialCustomer.CustomerID)
                            .OrderByDescending(o => o.OrderDate)
                            .FirstOrDefault();
                            
                        if (newOrder != null)
                        {
                            // Add some products to this order
                            foreach (var product in products.Take(3))
                            {
                                var orderDetail = new OrderDetails
                                {
                                    OrderID = newOrder.OrderID,
                                    ProductID = product.ProductID,
                                    UnitPrice = product.UnitPrice,
                                    Quantity = new Random().Next(1, 5),
                                    Discount = 0.1f
                                };
                                
                                _orderDetailService.AddOrderDetail(orderDetail);
                            }
                        }
                    }
                    catch
                    {
                        // Skip if error
                    }
                }
            }
            
            // Create orders for other customers
            foreach (var customer in customers.Where(c => c.Phone != "1234567890").Take(3))
            {
                try
                {
                    var order = new Orders
                    {
                        CustomerID = customer.CustomerID,
                        EmployeeID = 1, // Assuming employee ID 1 exists
                        OrderDate = orderDates[new Random().Next(orderDates.Length)]
                    };
                    
                    _orderService.AddOrder(order);
                    
                    // Get the last order for this customer to get its ID
                    var newOrder = _orderService.GetOrdersByCustomerID(customer.CustomerID)
                        .OrderByDescending(o => o.OrderDate)
                        .FirstOrDefault();
                        
                    if (newOrder != null)
                    {
                        // Add some products to this order
                        foreach (var product in products.Take(new Random().Next(1, 4)))
                        {
                            var orderDetail = new OrderDetails
                            {
                                OrderID = newOrder.OrderID,
                                ProductID = product.ProductID,
                                UnitPrice = product.UnitPrice,
                                Quantity = new Random().Next(1, 5),
                                Discount = 0.05f
                            };
                            
                            _orderDetailService.AddOrderDetail(orderDetail);
                        }
                    }
                }
                catch
                {
                    // Skip if error
                }
            }
        }
    }
}