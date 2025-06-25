using BusinessObject;
using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private CategoryDAO categoryDAO;

        public CategoryRepository()
        {
            categoryDAO = new CategoryDAO();
        }

        public IEnumerable<Categories> GetAllCategories()
        {
            return categoryDAO.GetAllCategories();
        }

        public Categories GetCategoryByID(int categoryID)
        {
            return categoryDAO.GetCategoryByID(categoryID);
        }

        public void AddCategory(Categories category)
        {
            categoryDAO.AddCategory(category);
        }

        public void UpdateCategory(Categories category)
        {
            categoryDAO.UpdateCategory(category);
        }

        public void DeleteCategory(int categoryID)
        {
            categoryDAO.DeleteCategory(categoryID);
        }

        public IEnumerable<Categories> SearchCategoriesByName(string name)
        {
            return categoryDAO.SearchCategoriesByName(name);
        }
    }
}
