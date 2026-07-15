using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIMS_Akura.Models
{
    public class StockFilterModel
    {
        public string Keyword { get; set; }
        public long? CategoryId { get; set; }
        public long? UnitId { get; set; }
        public bool? IsActive { get; set; }
        public long? SupplierId { get; set; }
        public DateTime? ExpiryBefore { get; set; }
        public bool? IsExpired { get; set; }
        public bool? IsLowStockOnly { get; set; }
        public bool? IsServiceProduct { get; set; }

        public long? ProductId { get; set; }
        public long? InvoiceId { get; set; }
    }

}