using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIMS_Akura.Models
{
    public class StockSummaryWidget
    {
        public int TotalProducts { get; set; }
        public int LowStockCount { get; set; }
        public int TotalBatches { get; set; }
        public decimal TotalStockValue { get; set; }
        public int ExpiringSoonCount { get; set; }
    }

}