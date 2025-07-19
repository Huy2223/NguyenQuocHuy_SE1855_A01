using BusinessObject;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService()
        {
            _productRepository = new ProductRepository();
        }

        public IEnumerable<Product> GetAllProducts()
        {
            return _productRepository.GetAllProducts();
        }

        public Product GetProductByID(int productID)
        {
            return _productRepository.GetProductByID(productID);
        }

        public void AddProduct(Product product)
        {
            _productRepository.AddProduct(product);
        }

        public void UpdateProduct(Product product)
        {
            _productRepository.UpdateProduct(product);
        }

        public void DeleteProduct(int productID)
        {
            _productRepository.DeleteProduct(productID);
        }

        public IEnumerable<Product> SearchProductsByName(string name)
        {
            return _productRepository.SearchProductsByName(name);
        }

        public IEnumerable<Product> GetProductsByCategoryID(int categoryID)
        {
            return _productRepository.GetProductsByCategoryID(categoryID);
        }

        public IEnumerable<Product> GetProductsByPriceRange(decimal minPrice, decimal maxPrice)
        {
            return _productRepository.GetProductsByPriceRange(minPrice, maxPrice);
        }

        public IEnumerable<Product> GetProductsInStock()
        {
            return _productRepository.GetProductsInStock();
        }
    }
}
