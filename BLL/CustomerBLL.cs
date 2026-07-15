using System;
using System.Collections.Generic;
using SIMS_Akura.DAL;
using SIMS_Akura.Models;

namespace SIMS_Akura.BLL
{
    public class CustomerBLL
    {
        private readonly CustomersDAL dal = new CustomersDAL();

        // Master list (Accounts + optional Customer)
        public List<Customer> GetAllAccountsCustomers(bool onlyActiveAccounts = false)
            => dal.GetAllFromAccounts(onlyActiveAccounts);

        public List<Customer> SearchAccountsCustomers(string keyword, bool onlyActiveAccounts = false)
            => dal.SearchFromAccounts(keyword, onlyActiveAccounts);

        // Classic list (only Customers table)
        public List<Customer> GetAllClassic()
            => dal.SearchClassic(""); // reuse classic search with empty kw if you prefer; or add a dedicated GetAllClassic()

        public List<Customer> SearchClassic(string keyword)
            => dal.SearchClassic(keyword);

        public Customer GetByCustomerId(long id)
            => dal.GetByCustomerId(id);

        public Customer GetByAccountId(long accountId)
            => dal.GetByAccountId(accountId);

        public OperationResult Add(Customer c)
        {
            if (c.AccountId == null)
                return OperationResult.Fail("Account link is required to create a Customer.");

            bool ok = dal.Add(c);
            return ok ? OperationResult.Ok("Customer created.") : OperationResult.Fail("Failed to create Customer.");
        }

        public OperationResult Update(Customer c)
        {
            if (c.Id <= 0)
                return OperationResult.Fail("Customer row not found. Create it first.");

            bool ok = dal.Update(c);
            return ok ? OperationResult.Ok("Customer updated.") : OperationResult.Fail("Failed to update Customer.");
        }

        public OperationResult Delete(long id)
        {
            bool ok = dal.Delete(id);
            return ok ? OperationResult.Ok("Customer deleted.") : OperationResult.Fail("Failed to delete Customer.");
        }
    }

    public class OperationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public static OperationResult Ok(string msg) => new OperationResult { Success = true, Message = msg };
        public static OperationResult Fail(string msg) => new OperationResult { Success = false, Message = msg };
    }
}
