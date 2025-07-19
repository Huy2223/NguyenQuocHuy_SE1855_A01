using BusinessObject;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class CategoryDAO
    {
        // Get all categories
        public List<Category> GetAllCategories()
        {
            using var context = new LucySalesDataContext();
            return context.Categories.Include(c => c.Products).ToList();
        }

        // Get category by ID
        public Category GetCategoryByID(int categoryID)
        {
            using var context = new LucySalesDataContext();
            return context.Categories.Include(c => c.Products).FirstOrDefault(c => c.CategoryId == categoryID);
        }

        // Add a new category
        public void AddCategory(Category category)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            // Validate required fields
            if (string.IsNullOrWhiteSpace(category.CategoryName))
                throw new ArgumentException("Category name is required");

            using var context = new LucySalesDataContext();
            context.Categories.Add(category);
            context.SaveChanges();
        }

        // Update an existing category
        public void UpdateCategory(Category category)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            // Validate required fields
            if (string.IsNullOrWhiteSpace(category.CategoryName))
                throw new ArgumentException("Category name is required");

            using var context = new LucySalesDataContext();
            
            // Find existing category
            var existingCategory = context.Categories.FirstOrDefault(c => c.CategoryId == category.CategoryId);
            if (existingCategory == null)
                throw new ArgumentException($"Category with ID {category.CategoryId} not found");

            // Update properties
            existingCategory.CategoryName = category.CategoryName;
            existingCategory.Description = category.Description;
            existingCategory.Picture = category.Picture;
            
            context.SaveChanges();
        }

        // Delete a category
        public void DeleteCategory(int categoryID)
        {
            using var context = new LucySalesDataContext();
            var categoryToDelete = context.Categories.FirstOrDefault(c => c.CategoryId == categoryID);
            if (categoryToDelete == null)
                throw new ArgumentException($"Category with ID {categoryID} not found");

            context.Categories.Remove(categoryToDelete);
            context.SaveChanges();
        }

        // Search categories by name
        public List<Category> SearchCategoriesByName(string name)
        {
            using var context = new LucySalesDataContext();
            if (string.IsNullOrWhiteSpace(name))
                return context.Categories.Include(c => c.Products).ToList();

            return context.Categories.Include(c => c.Products)
                .Where(c => c.CategoryName.Contains(name)).ToList();
        }
    }
}
