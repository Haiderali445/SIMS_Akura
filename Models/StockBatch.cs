using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIMS_Akura.Models
{
    public class StockBatch
    {
        public long Id { get; set; }
        public long ProductId { get; set; }
        public string BatchCode { get; set; }
        public decimal Qty { get; set; }
        public decimal AvailableQty { get; set; }
        public decimal UnitCost { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public long? CreatedBy { get; set; }
        public long? SupplierId { get; set; }
        public string SupplierName { get; set; }
        public long? PurchaseInvoiceId { get; set; }

        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public string InvoiceCode { get; set; }

        public bool IsExpired
        {
            get { return ExpiresAt.HasValue && ExpiresAt.Value < DateTime.UtcNow; }
        }
    }

}