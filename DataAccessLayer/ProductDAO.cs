using BusinessObject;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class ProductDAO
    {
        // Get all products
        public List<Product> GetAllProducts()
        {
            using var context = new LucySalesDataContext();
            return context.Products.Include(p => p.Category).ToList();
        }

        // Get product by ID
        public Product GetProductByID(int productID)
        {
            using var context = new LucySalesDataContext();
            return context.Products.Include(p => p.Category).FirstOrDefault(p => p.ProductId == productID);
        }

        // Add a new product
        public void AddProduct(Product product)
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

            using var context = new LucySalesDataContext();
            context.Products.Add(product);
            context.SaveChanges();
        }

        // Update an existing product
        public void UpdateProduct(Product product)
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

            using var context = new LucySalesDataContext();
            
            // Find existing product
            var existingProduct = context.Products.FirstOrDefault(p => p.ProductId == product.ProductId);
            if (existingProduct == null)
                throw new ArgumentException($"Product with ID {product.ProductId} not found");

            // Update properties
            existingProduct.ProductName = product.ProductName;
            existingProduct.CategoryId = product.CategoryId;
            existingProduct.UnitPrice = product.UnitPrice;
            existingProduct.UnitsInStock = product.UnitsInStock;
            existingProduct.SupplierId = product.SupplierId;
            existingProduct.QuantityPerUnit = product.QuantityPerUnit;
            existingProduct.UnitsOnOrder = product.UnitsOnOrder;
            existingProduct.ReorderLevel = product.ReorderLevel;
            existingProduct.Discontinued = product.Discontinued;
            
            context.SaveChanges();
        }

        // Delete a product
        public void DeleteProduct(int productID)
        {
            using var context = new LucySalesDataContext();
            var product = context.Products.FirstOrDefault(p => p.ProductId == productID);
            if (product == null)
                throw new ArgumentException($"Product with ID {productID} not found");

            context.Products.Remove(product);
            context.SaveChanges();
        }

        // Search products by name
        public List<Product> SearchProductsByName(string name)
        {
            using var context = new LucySalesDataContext();
            if (string.IsNullOrWhiteSpace(name))
                return context.Products.Include(p => p.Category).ToList();

            return context.Products.Include(p => p.Category)
                .Where(p => p.ProductName.Contains(name)).ToList();
        }

        // Get products by category ID
        public List<Product> GetProductsByCategoryID(int categoryID)
        {
            using var context = new LucySalesDataContext();
            return context.Products.Include(p => p.Category)
                .Where(p => p.CategoryId == categoryID).ToList();
        }

        // Get products within price range
        public List<Product> GetProductsByPriceRange(decimal minPrice, decimal maxPrice)
        {
            using var context = new LucySalesDataContext();
            return context.Products.Include(p => p.Category)
                .Where(p => p.UnitPrice >= minPrice && p.UnitPrice <= maxPrice).ToList();
        }

        // Get products in stock
        public List<Product> GetProductsInStock()
        {
            using var context = new LucySalesDataContext();
            return context.Products.Include(p => p.Category)
                .Where(p => p.UnitsInStock > 0).ToList();
        }
    }
}
