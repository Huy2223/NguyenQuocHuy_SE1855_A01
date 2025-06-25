using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface IProductRepository
    {
        IEnumerable<Products> GetAllProducts();
        Products GetProductByID(int productID);
        void AddProduct(Products product);
        void UpdateProduct(Products product);
        void DeleteProduct(int productID);
        IEnumerable<Products> SearchProductsByName(string name);
        IEnumerable<Products> GetProductsByCategoryID(int categoryID);
        IEnumerable<Products> GetProductsByPriceRange(decimal minPrice, decimal maxPrice);
        IEnumerable<Products> GetProductsInStock();
    }
}
