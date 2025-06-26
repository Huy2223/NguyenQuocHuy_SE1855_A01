using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class ProductDAO
    {
        // Mock data - in a real application, this would be stored in a database
        private static List<Products> _products = new List<Products>
        {
            new Products
            {
                ProductID = 1,
                ProductName = "Laptop",
                CategoryID = 1,
                UnitPrice = 1200.00m,
                UnitsInStock = 15
            },
            new Products
            {
                ProductID = 2,
                ProductName = "Smartphone",
                CategoryID = 1,
                UnitPrice = 800.00m,
                UnitsInStock = 25
            },
            new Products
            {
                ProductID = 3,
                ProductName = "Tablet",  // Changed from Headphones to Tablet to match order details
                CategoryID = 1,
                UnitPrice = 500.00m,     // Updated price to match order details
                UnitsInStock = 30
            },
            new Products
            {
                ProductID = 4,
                ProductName = "Headphones", // Moved Headphones to product ID 4
                CategoryID = 2,
                UnitPrice = 150.00m,
                UnitsInStock = 40
            },
            new Products
            {
                ProductID = 5,
                ProductName = "Keyboard",
                CategoryID = 2,
                UnitPrice = 80.00m,
                UnitsInStock = 20
            }
        };

        private static int _nextId = 6;

        // Get all products
        public List<Products> GetAllProducts()
        {
            return _products;
        }

        // Get product by ID
        public Products GetProductByID(int productID)
        {
            return _products.FirstOrDefault(p => p.ProductID == productID);
        }

        // Add a new product
        public void AddProduct(Products product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            // Validate required fields
            if (string.IsNullOrWhiteSpace(product.ProductName))
                throw new ArgumentException("Product name is required");
            if (product.UnitPrice < 0)
                throw new ArgumentException("Unit price cannot be negative");
            if (product.UnitsInStock < 0)
                throw new ArgumentException("Units in stock cannot be negative");

            // Set new ID
            product.ProductID = _nextId++;
            _products.Add(product);
        }

        // Update an existing product
        public void UpdateProduct(Products product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            // Validate required fields
            if (string.IsNullOrWhiteSpace(product.ProductName))
                throw new ArgumentException("Product name is required");
            if (product.UnitPrice < 0)
                throw new ArgumentException("Unit price cannot be negative");
            if (product.UnitsInStock < 0)
                throw new ArgumentException("Units in stock cannot be negative");

            // Find existing product
            var existingProduct = _products.FirstOrDefault(p => p.ProductID == product.ProductID);
            if (existingProduct == null)
                throw new ArgumentException($"Product with ID {product.ProductID} not found");

            // Update properties
            existingProduct.ProductName = product.ProductName;
            existingProduct.CategoryID = product.CategoryID;
            existingProduct.UnitPrice = product.UnitPrice;
            existingProduct.UnitsInStock = product.UnitsInStock;
        }

        // Delete a product
        public void DeleteProduct(int productID)
        {
            var product = _products.FirstOrDefault(p => p.ProductID == productID);
            if (product == null)
                throw new ArgumentException($"Product with ID {productID} not found");

            _products.Remove(product);
        }

        // Search products by name
        public List<Products> SearchProductsByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return _products;

            return _products.Where(p => p.ProductName.Contains(name, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        // Get products by category ID
        public List<Products> GetProductsByCategoryID(int categoryID)
        {
            return _products.Where(p => p.CategoryID == categoryID).ToList();
        }

        // Get products within price range
        public List<Products> GetProductsByPriceRange(decimal minPrice, decimal maxPrice)
        {
            return _products.Where(p => p.UnitPrice >= minPrice && p.UnitPrice <= maxPrice).ToList();
        }

        // Get products in stock
        public List<Products> GetProductsInStock()
        {
            return _products.Where(p => p.UnitsInStock > 0).ToList();
        }
    }
}
