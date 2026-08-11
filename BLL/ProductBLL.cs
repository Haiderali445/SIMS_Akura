using System;
using System.Collections.Generic;
using SIMS_Akura.DAL;
using SIMS_Akura.Models;

namespace SIMS_Akura.BLL
{
    public class ProductBLL
    {
        private readonly ProductDAL dal = new ProductDAL();

        public List<Product> GetAll() => dal.GetAll();

        public Product GetById(long id) => dal.GetById(id);

        public List<Product> Search(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return dal.GetAll();
            return dal.Search(keyword);
        }

        public (bool Success, string Message) Add(Product p)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(p.Name))
                    return (false, "Product name is required.");
                if (string.IsNullOrWhiteSpace(p.ProductCode))
                    return (false, "Product code is required.");
                if (p.UnitId == null)
                    return (false, "Please select a valid unit.");
                if (p.CategoryId == null)
                    return (false, "Please select a valid category.");
                if (p.DefaultPurchasePrice != null && p.DefaultPurchasePrice < 0)
                    return (false, "Invalid purchase price.");
                if (p.DefaultSalesPrice != null && p.DefaultSalesPrice < 0)
                    return (false, "Invalid sales price.");
                if (p.MinimumStock < 0)
                    return (false, "Minimum stock cannot be negative.");

                bool added = dal.Add(p);
                return added
                    ? (true, "Product added successfully ✅")
                    : (false, "Failed to add product.");
            }
            catch (Exception ex)
            {
                return (false, $"Error adding product: {ex.Message}");
            }
        }

        public (bool Success, string Message) Update(Product p)
        {
            try
            {
                if (p.Id <= 0)
                    return (false, "Invalid product ID.");
                if (string.IsNullOrWhiteSpace(p.Name))
                    return (false, "Product name is required.");

                bool updated = dal.Update(p);
                return updated
                    ? (true, "Product updated successfully ✅")
                    : (false, "Failed to update product.");
            }
            catch (Exception ex)
            {
                return (false, $"Error updating product: {ex.Message}");
            }
        }

        public (bool Success, string Message) Delete(long id, long userId)
        {
            try
            {
                if (id <= 0)
                    return (false, "Invalid product ID.");

                bool deleted = dal.Delete(id, userId);
                return deleted
                    ? (true, "Product deleted successfully 🗑️")
                    : (false, "Failed to delete product.");
            }
            catch (Exception ex)
            {
                return (false, $"Error deleting product: {ex.Message}");
            }
        }
    }
}
