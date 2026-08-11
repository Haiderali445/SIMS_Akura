using System;
using System.Collections.Generic;
using SIMS_Akura.DAL;
using SIMS_Akura.Models;

namespace SIMS_Akura.BLL
{
    public class UnitBLL
    {
        private readonly UnitsDAL dal = new UnitsDAL();

        public List<Unit> GetAll() => dal.GetAll();

        public (bool Success, string Message) Add(Unit u)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(u.Code))
                    return (false, "Unit code is required.");

                if (string.IsNullOrWhiteSpace(u.Name))
                    return (false, "Unit name is required.");

                bool added = dal.Add(u);
                return added
                    ? (true, "Unit added successfully ✅")
                    : (false, "Failed to add unit. Please try again.");
            }
            catch (Exception ex)
            {
                return (false, $"Error adding unit: {ex.Message}");
            }
        }
        // ✅ Update unit
        public (bool Success, string Message) Update(Unit u)
        {
            try
            {
                if (u.Id <= 0)
                    return (false, "Invalid unit ID.");

                if (string.IsNullOrWhiteSpace(u.Code))
                    return (false, "Unit code is required.");

                if (string.IsNullOrWhiteSpace(u.Name))
                    return (false, "Unit name is required.");

                bool updated = dal.Update(u);
                return updated
                    ? (true, "Unit updated successfully ✅")
                    : (false, "Failed to update unit. Please try again.");
            }
            catch (Exception ex)
            {
                return (false, $"Error updating unit: {ex.Message}");
            }
        }

        // ✅ Delete unit
        public (bool Success, string Message) Delete(long id)
        {
            try
            {
                if (id <= 0)
                    return (false, "Invalid unit ID.");

                bool deleted = dal.Delete(id);
                return deleted
                    ? (true, "Unit deleted successfully 🗑️")
                    : (false, "Failed to delete unit. It may already be deleted.");
            }
            catch (Exception ex)
            {
                return (false, $"Error deleting unit: {ex.Message}");
            }
        }

    }
}
