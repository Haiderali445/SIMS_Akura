using System;
using System.Collections.Generic;
using SIMS_Akura.DAL;
using SIMS_Akura.Models;

namespace SIMS_Akura.BLL
{
    public class CategoryBLL
    {
        private readonly CategoryDAL dal = new CategoryDAL();

        // Get all categories
        public List<Category> GetAll()
        {
            return dal.GetAllCategories();
        }

        //  Get by ID
        public Category GetById(long id)
        {
            return dal.GetById(id);
        }

        // Add new category 
        public (bool Success, string Message) Add(Category category)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(category.Name))
                    return (false, "Category name is required.");

                if (dal.CheckIfExists(category.Name))
                    return (false, $"Category '{category.Name}' already exists.");

                bool added = dal.InsertCategory(category);

                return added
                    ? (true, "Category added successfully ✅")
                    : (false, "Failed to add category. Please try again.");
            }
            catch (Exception ex)
            {
                return (false, $"Error while adding category: {ex.Message}");
            }
        }

        //  Update category
        public (bool Success, string Message) Update(Category category)
        {
            try
            {
                if (category.Id <= 0)
                    return (false, "Invalid category ID.");

                if (string.IsNullOrWhiteSpace(category.Name))
                    return (false, "Category name cannot be empty.");

                bool updated = dal.UpdateCategory(category);

                return updated
                    ? (true, "Category updated successfully ✅")
                    : (false, "Failed to update category. Please refresh and try again.");
            }
            catch (Exception ex)
            {
                return (false, $"Error while updating category: {ex.Message}");
            }
        }

        //  Delete (hard delete)
        public (bool Success, string Message) Delete(long id)
        {
            try
            {
                if (id <= 0)
                    return (false, "Invalid category ID.");

                bool deleted = dal.hardDeleteCategory(id);

                return deleted
                    ? (true, "Category deleted successfully 🗑️")
                    : (false, "Failed to delete category. It may already be deleted.");
            }
            catch (Exception ex)
            {
                return (false, $"Error while deleting category: {ex.Message}");
            }
        }

        // Search
        public List<Category> Search(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return dal.GetAllCategories();

            return dal.Search(keyword);
        }
    }
}
