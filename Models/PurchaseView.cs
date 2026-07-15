using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIMS_Akura.Models
{
    public class PurchaseView
    {
        public long InvoiceId { get; set; }
        public string InvoiceCode { get; set; }
        public string SupplierName { get; set; }
        public decimal GrandTotal { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}