using SIMS_Akura.DAL;
using SIMS_Akura.Models;
using System.Collections.Generic;
using System.Linq;

namespace SIMS_Akura.BLL
{
    public class AccountBLL
    {
        private readonly AccountDAL dal = new AccountDAL();

        // 🔹 Get all accounts
        public List<Account> GetAll()
        {
            return dal.GetAll();
        }
        public List<Account> GetSuppliers()
        {
            var allAccounts = GetAll(); // or your DAL method
            return allAccounts.Where(a => a.AccountType == "Supplier").ToList();
        }

        // 🔹 Get account by ID
        public Account GetById(long id)
        {
            return dal.GetById(id);
        }
        public Account GetByCode(string code) => dal.GetByCode(code);


        // 🔹 Save (insert or update)
        public bool Save(Account acc)
        {
            if (acc.Id == 0)
                return dal.Insert(acc);
            else
                return dal.Update(acc);
        }

        // 🔹 Search with filters
        public List<Account> Search(string keyword = "", string accountType = "", bool? isActive = null)
        {
            return dal.Search(keyword, accountType, isActive);
        }

        // 🔹 Future: deactivate account
        public bool SetActiveStatus(long accountId, bool isActive)
        {
            return dal.SetActiveStatus(accountId, isActive);
        }
        // 🔹 Adjust balance by delta amount (+/-)
        public bool AdjustBalance(long accountId, decimal delta)
        {
            return dal.AdjustBalance(accountId, delta);
        }

        // 🔹 Record account transaction
        public bool AddTransaction(AccountTransaction trx)
        {
            return dal.AddTransaction(trx);
        }


        public bool Delete(long id)
        {
            return dal.Delete(id);
        }

        // 🔹 Future: update balance
        public bool UpdateBalance(long accountId, decimal newBalance)
        {
            return dal.UpdateBalance(accountId, newBalance);
        }
    }
}
