using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIMS_Akura.Models
{
    public class BatchAuditTrail
    {
        public long BatchId { get; set; }
        public string BatchCode { get; set; }
        public string ProductName { get; set; }
        public decimal InitialQty { get; set; }
        public decimal AvailableQty { get; set; }
        public List<StockMovement> Movements { get; set; }
    }

}