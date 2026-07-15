using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIMS_Akura.Models
{
    public class StockView
    {
        public long ProductId { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string CategoryName { get; set; }
        public string UnitName { get; set; }
        public decimal CurrentStock { get; set; }
        public decimal MinimumStock { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal SalesPrice { get; set; }
        public int RowNo { get; set; }

        public bool IsServiceProduct { get; set; }
        public bool IsActive { get; set; }
        public DateTime LastUpdated { get; set; }

        public decimal StockValue { get { return CurrentStock * PurchasePrice; } }

        public string LastSupplierName { get; set; }
        public DateTime? LastPurchaseDate { get; set; }
        // ✅ Add this for filtering by supplier
        public long SupplierId { get; set; }
        public string SupplierName { get; set; }

        public long? CategoryId { get; set; }
        public long? UnitId { get; set; }
    }

}