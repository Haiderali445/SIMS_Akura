using System;

namespace SIMS_Akura.Models
{
    public class Customer
    {
        // Customer table (nullable when account has no linked customer yet)
        public long Id { get; set; }                 // 0 means "no customer row yet"
        public string Name { get; set; }             // if Customer row exists; falls back to AccountName in views
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public bool IsActive { get; set; }           // Customer.IsActive when exists, else Account.IsActive
        public DateTime CreatedAt { get; set; }

        // Account linkage
        public long? AccountId { get; set; }         // FK to Accounts.id (nullable)
        public string AccountName { get; set; }
        public string AccountCode { get; set; }
        public string AccountType { get; set; }      // "Customer" expected

        // UI helpers
        public int RowNo { get; set; }

    }
}
