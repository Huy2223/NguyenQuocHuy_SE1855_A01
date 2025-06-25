using BusinessObject;
using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class ProductRepository : IProductRepository
    {
        private ProductDAO productDAO;

        public ProductRepository()
        {
            productDAO = new ProductDAO();
        }

        public IEnumerable<Products> GetAllProducts()
        {
            return productDAO.GetAllProducts();
        }

        public Products GetProductByID(int productID)
        {
            return productDAO.GetProductByID(productID);
        }

        public void AddProduct(Products product)
        {
            productDAO.AddProduct(product);
        }

        public void UpdateProduct(Products product)
        {
            productDAO.UpdateProduct(product);
        }

        public void DeleteProduct(int productID)
        {
            productDAO.DeleteProduct(productID);
        }

        public IEnumerable<Products> SearchProductsByName(string name)
        {
            return productDAO.SearchProductsByName(name);
        }

        public IEnumerable<Products> GetProductsByCategoryID(int categoryID)
        {
            return productDAO.GetProductsByCategoryID(categoryID);
        }

        public IEnumerable<Products> GetProductsByPriceRange(decimal minPrice, decimal maxPrice)
        {
            return productDAO.GetProductsByPriceRange(minPrice, maxPrice);
        }

        public IEnumerable<Products> GetProductsInStock()
        {
            return productDAO.GetProductsInStock();
        }
    }
}
