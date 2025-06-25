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

        public IEnumerable<Products> GetAllProducts()
        {
            return _productRepository.GetAllProducts();
        }

        public Products GetProductByID(int productID)
        {
            return _productRepository.GetProductByID(productID);
        }

        public void AddProduct(Products product)
        {
            _productRepository.AddProduct(product);
        }

        public void UpdateProduct(Products product)
        {
            _productRepository.UpdateProduct(product);
        }

        public void DeleteProduct(int productID)
        {
            _productRepository.DeleteProduct(productID);
        }

        public IEnumerable<Products> SearchProductsByName(string name)
        {
            return _productRepository.SearchProductsByName(name);
        }

        public IEnumerable<Products> GetProductsByCategoryID(int categoryID)
        {
            return _productRepository.GetProductsByCategoryID(categoryID);
        }

        public IEnumerable<Products> GetProductsByPriceRange(decimal minPrice, decimal maxPrice)
        {
            return _productRepository.GetProductsByPriceRange(minPrice, maxPrice);
        }

        public IEnumerable<Products> GetProductsInStock()
        {
            return _productRepository.GetProductsInStock();
        }
    }
}
