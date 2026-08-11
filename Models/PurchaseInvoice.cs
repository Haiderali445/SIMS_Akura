using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIMS_Akura.Models
{
    public class PurchaseInvoice
    {
        public long Id { get; set; }
        public long ShopId { get; set; } = 1;
        
        public string InvoiceCode { get; set; }
        public long AccountId { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal Fare { get; set; } = 0;
        public long CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}