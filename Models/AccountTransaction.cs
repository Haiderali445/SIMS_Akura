using System;

namespace SIMS_Akura.Models
{
    public class AccountTransaction
    {
        public long Id { get; set; }
        public string TransactionCode { get; set; }
        public long AccountId { get; set; }
        public string TransactionType { get; set; } // e.g. "Purchase", "PaymentOut"
        public decimal Amount { get; set; }
        public string ReferenceTable { get; set; }
        public long? ReferenceId { get; set; }
        public string Note { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
