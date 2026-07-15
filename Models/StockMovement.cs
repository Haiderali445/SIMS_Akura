using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIMS_Akura.Models
{
    public class StockMovement
    {
        public long Id { get; set; }
        public long ProductId { get; set; }
        public decimal ChangeQty { get; set; }
        public string MovementType { get; set; }
        public string ReferenceTable { get; set; }
        public long? ReferenceId { get; set; }
        public string ReasonNote { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public long? BatchId { get; set; }
        public decimal? UnitCost { get; set; }

        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public string InvoiceCode { get; set; }

        public string MovementDirection
        {
            get { return ChangeQty >= 0 ? "In" : "Out"; }
        }
    }

}