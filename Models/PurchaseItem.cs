using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIMS_Akura.Models
{
    [Serializable]
    public class PurchaseItem
    {
        public long InvoiceId { get; set; }
        public long ProductId { get; set; }
        public decimal Qty { get; set; }
        public decimal Rate { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public string BatchCode { get; set; }
        public string SupplierName { get; set; }
        public decimal Total
        {
            get { return Qty * Rate; }
        }


        public long CreatedBy { get; set; }
    }

}