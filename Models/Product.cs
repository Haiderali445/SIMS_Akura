using System;

namespace SIMS_Akura.Models
{
    public class Product
    {
        public long Id { get; set; }
        public string ProductCode { get; set; }
        public string Name { get; set; }
        public long? CategoryId { get; set; }
        public string Brand { get; set; }
        public string Description { get; set; }
        public string Barcode { get; set; }

        public decimal? DefaultPurchasePrice { get; set; }
        public decimal? DefaultSalesPrice { get; set; }

        public decimal OpeningStock { get; set; }
        public decimal CurrentStock { get; set; }
        public decimal MinimumStock { get; set; }

        public long? UnitId { get; set; }
        public bool IsServiceProduct { get; set; }
        public bool IsActive { get; set; }

        public long? CreatedBy { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool Deleted { get; set; }
        public long? DeletedBy { get; set; }

        // ---------- Joined Display Fields ----------
        public string CategoryName { get; set; }
        public string UnitName { get; set; }
        public string CreatedByName { get; set; }
        public long RowNo { get; set; }

        // ---------- Derived/Helper Fields ----------
        public decimal StockValue => CurrentStock * (DefaultPurchasePrice ?? 0);
        public decimal ProfitMargin =>
            (DefaultSalesPrice.HasValue && DefaultPurchasePrice.HasValue && DefaultPurchasePrice > 0)
            ? ((DefaultSalesPrice.Value - DefaultPurchasePrice.Value) / DefaultPurchasePrice.Value) * 100
            : 0;
    }
}
