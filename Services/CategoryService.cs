using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject;
using Repositories;

namespace Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService()
        {
            _categoryRepository = new CategoryRepository();
        }

        public IEnumerable<Category> GetAllCategories()
        {
            return _categoryRepository.GetAllCategories();
        }

        public Category GetCategoryByID(int categoryID)
        {
            return _categoryRepository.GetCategoryByID(categoryID);
        }

        public void AddCategory(Category category)
        {
            _categoryRepository.AddCategory(category);
        }

        public void UpdateCategory(Category category)
        {
            _categoryRepository.UpdateCategory(category);
        }

        public void DeleteCategory(int categoryID)
        {
            _categoryRepository.DeleteCategory(categoryID);
        }

        public IEnumerable<Category> SearchCategoriesByName(string name)
        {
            return _categoryRepository.SearchCategoriesByName(name);
        }
    }
}
