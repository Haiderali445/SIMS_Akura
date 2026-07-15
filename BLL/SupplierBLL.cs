using System;
using System.Collections.Generic;
using SIMS_Akura.DAL;
using SIMS_Akura.Models;

namespace SIMS_Akura.BLL
{
    public class SupplierBLL
    {
        private readonly SuppliersDAL dal = new SuppliersDAL();
        private readonly AccountBLL accountBLL = new AccountBLL();

        // ✅ Get all suppliers
        public List<Supplier> GetAll() => dal.GetAll();

        // ✅ Get by ID
        public Supplier GetById(long id) => dal.GetById(id);

        // ✅ Add supplier (auto-creates linked Account)
        public (bool Success, string Message) Add(Supplier s)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(s.Name))
                    return (false, "Supplier name is required.");

                // 🔗 Create linked Account first
                var acc = new Account
                {
                    Id = 0,
                    AccountCode = GenerateAccountCode(),
                    Name = s.Name,
                    AccountType = "Supplier",
                    Phone = s.Phone,
                    Email = s.Email,
                    Address = s.Address,
                    CurrentBalance = 0,
                    IsActive = s.IsActive,
                    CreatedBy = 1
                };

                bool accountSaved = accountBLL.Save(acc);
                if (!accountSaved)
                    return (false, "Failed to create linked account for supplier.");

                // Assign new AccountId to supplier
                var createdAccount = accountBLL.GetByCode(acc.AccountCode);
                if (createdAccount == null)
                    return (false, "Linked account not found after creation.");

                s.AccountId = createdAccount.Id;

                bool added = dal.Add(s);
                return added
                    ? (true, "Supplier added successfully ✅")
                    : (false, "Failed to add supplier.");
            }
            catch (Exception ex)
            {
                return (false, $"Error adding supplier: {ex.Message}");
            }
        }

        // ✅ Update supplier
        public (bool Success, string Message) Update(Supplier s)
        {
            try
            {
                if (s.Id <= 0)
                    return (false, "Invalid supplier ID.");

                if (string.IsNullOrWhiteSpace(s.Name))
                    return (false, "Supplier name is required.");

                bool updated = dal.Update(s);

                // 🔗 Optionally update linked account name/phone/email
                if (s.AccountId.HasValue)
                {
                    var acc = accountBLL.GetById(s.AccountId.Value);
                    if (acc != null)
                    {
                        acc.Name = s.Name;
                        acc.Phone = s.Phone;
                        acc.Email = s.Email;
                        acc.Address = s.Address;
                        accountBLL.Save(acc);
                    }
                }

                return updated
                    ? (true, "Supplier updated successfully ✅")
                    : (false, "Failed to update supplier.");
            }
            catch (Exception ex)
            {
                return (false, $"Error updating supplier: {ex.Message}");
            }
        }

        // ✅ Delete supplier (and optionally linked account)
        public (bool Success, string Message) Delete(long id)
        {
            try
            {
                if (id <= 0)
                    return (false, "Invalid supplier ID.");

                var supplier = dal.GetById(id);
                if (supplier == null)
                    return (false, "Supplier not found.");

                bool deleted = dal.Delete(id);

                // 🔗 Optionally deactivate linked account instead of hard delete
                if (supplier.AccountId.HasValue)
                {
                    accountBLL.SetActiveStatus(supplier.AccountId.Value, false);
                }

                return deleted
                    ? (true, "Supplier deleted successfully 🗑️")
                    : (false, "Failed to delete supplier.");
            }
            catch (Exception ex)
            {
                return (false, $"Error deleting supplier: {ex.Message}");
            }
        }

        // ✅ Search suppliers
        public List<Supplier> Search(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return dal.GetAll();

            return dal.Search(keyword);
        }

        // ✅ Optional: check if supplier exists
        public bool CheckIfExists(string name)
        {
            return dal.CheckIfExists(name);
        }

        // 🔧 Helper: generate ACT- code for linked account
        private string GenerateAccountCode()
        {
            var allAccounts = accountBLL.Search("", "", null);
            var existingCodes = new HashSet<string>();
            foreach (var acc in allAccounts)
                existingCodes.Add(acc.AccountCode);

            string newCode;
            var rand = new Random();
            do
            {
                string randomPart = rand.Next(1000000000, int.MaxValue).ToString();
                newCode = $"ACT-{randomPart}";
            }
            while (existingCodes.Contains(newCode));

            return newCode;
        }
    }
}
