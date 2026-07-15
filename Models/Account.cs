using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIMS_Akura.Models
{
    public class Account
    {
        public long Id { get; set; }
        public string AccountCode { get; set; }
        public string Name { get; set; }
        public string AccountType { get; set; } // Supplier, Customer, Expense, Bank, etc.
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public decimal CurrentBalance { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public long? CreatedBy { get; set; }
        public int RowNo { get; set; } // for GridView display only

        // 🔗 Optional references
        public Supplier Supplier { get; set; }
        public Customer Customer { get; set; }

    }
}