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

        public IEnumerable<Product> GetAllProducts()
        {
            return productDAO.GetAllProducts();
        }

        public Product GetProductByID(int productID)
        {
            return productDAO.GetProductByID(productID);
        }

        public void AddProduct(Product product)
        {
            productDAO.AddProduct(product);
        }

        public void UpdateProduct(Product product)
        {
            productDAO.UpdateProduct(product);
        }

        public void DeleteProduct(int productID)
        {
            productDAO.DeleteProduct(productID);
        }

        public IEnumerable<Product> SearchProductsByName(string name)
        {
            return productDAO.SearchProductsByName(name);
        }

        public IEnumerable<Product> GetProductsByCategoryID(int categoryID)
        {
            return productDAO.GetProductsByCategoryID(categoryID);
        }

        public IEnumerable<Product> GetProductsByPriceRange(decimal minPrice, decimal maxPrice)
        {
            return productDAO.GetProductsByPriceRange(minPrice, maxPrice);
        }

        public IEnumerable<Product> GetProductsInStock()
        {
            return productDAO.GetProductsInStock();
        }
    }
}
