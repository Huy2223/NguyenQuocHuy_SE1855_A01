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
        IEnumerable<Product> GetAllProducts();
        Product GetProductByID(int productID);
        void AddProduct(Product product);
        void UpdateProduct(Product product);
        void DeleteProduct(int productID);
        IEnumerable<Product> SearchProductsByName(string name);
        IEnumerable<Product> GetProductsByCategoryID(int categoryID);
        IEnumerable<Product> GetProductsByPriceRange(decimal minPrice, decimal maxPrice);
        IEnumerable<Product> GetProductsInStock();
    }
}
