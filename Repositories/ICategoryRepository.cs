using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface ICategoryRepository
    {
        IEnumerable<Category> GetAllCategories();
        Category GetCategoryByID(int categoryID);
        void AddCategory(Category category);
        void UpdateCategory(Category category);
        void DeleteCategory(int categoryID);
        IEnumerable<Category> SearchCategoriesByName(string name);
    }
}
