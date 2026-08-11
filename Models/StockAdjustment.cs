using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIMS_Akura.Models
{
    public class StockAdjustment
    {
        public long ProductId { get; set; }
        public decimal QtyChange { get; set; }
        public string ReasonNote { get; set; }
        public long CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public long? BatchId { get; set; }

        public string ProductName { get; set; }

        public bool IsIncrease
        {
            get { return QtyChange > 0; }
        }
    }

}
