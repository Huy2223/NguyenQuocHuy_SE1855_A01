using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface ICategoryService
    {
        IEnumerable<Categories> GetAllCategories();
        Categories GetCategoryByID(int categoryID);
        void AddCategory(Categories category);
        void UpdateCategory(Categories category);
        void DeleteCategory(int categoryID);
        IEnumerable<Categories> SearchCategoriesByName(string name);
    }
}
