using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class CategoryDAO
    {
        // Mock data - in a real application, this would be stored in a database
        private static List<Categories> _categories = new List<Categories>
        {
            new Categories
            {
                CategoryID = 1,
                CategoryName = "Electronics",
                Description = "Electronic devices and accessories"
            },
            new Categories
            {
                CategoryID = 2,
                CategoryName = "Computer Accessories",
                Description = "Accessories for computers and laptops"
            },
            new Categories
            {
                CategoryID = 3,
                CategoryName = "Office Supplies",
                Description = "Supplies for office use"
            }
        };

        private static int _nextId = 4;

        // Get all categories
        public List<Categories> GetAllCategories()
        {
            return _categories;
        }

        // Get category by ID
        public Categories GetCategoryByID(int categoryID)
        {
            return _categories.FirstOrDefault(c => c.CategoryID == categoryID);
        }

        // Add a new category
        public void AddCategory(Categories category)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            // Validate required fields
            if (string.IsNullOrWhiteSpace(category.CategoryName))
                throw new ArgumentException("Category name is required");

            // Set new ID
            category.CategoryID = _nextId++;
            _categories.Add(category);
        }

        // Update an existing category
        public void UpdateCategory(Categories category)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            // Validate required fields
            if (string.IsNullOrWhiteSpace(category.CategoryName))
                throw new ArgumentException("Category name is required");

            // Find existing category
            var existingCategory = _categories.FirstOrDefault(c => c.CategoryID == category.CategoryID);
            if (existingCategory == null)
                throw new ArgumentException($"Category with ID {category.CategoryID} not found");

            // Update properties
            existingCategory.CategoryName = category.CategoryName;
            existingCategory.Description = category.Description;
        }

        // Delete a category
        public void DeleteCategory(int categoryID)
        {
            var categoryToDelete = _categories.FirstOrDefault(c => c.CategoryID == categoryID);
            if (categoryToDelete == null)
                throw new ArgumentException($"Category with ID {categoryID} not found");

            _categories.Remove(categoryToDelete);
        }

        // Search categories by name
        public List<Categories> SearchCategoriesByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return _categories;

            return _categories.Where(c => c.CategoryName.Contains(name, StringComparison.OrdinalIgnoreCase)).ToList();
        }
    }
}
