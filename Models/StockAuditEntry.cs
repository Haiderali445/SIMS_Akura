using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIMS_Akura.Models
{
    public class StockAuditEntry
    {
        public DateTime Timestamp { get; set; }
        public string ActionType { get; set; } // Movement, PriceChange
        public string Description { get; set; }

        public decimal? QtyChange { get; set; }
        public decimal? OldPrice { get; set; }
        public decimal? NewPrice { get; set; }

        public string Reference { get; set; }
        public string ReferenceCode { get; set; }

        public long? UserId { get; set; }
        public string UserName { get; set; }

        public long? ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }

        public long? BatchId { get; set; }

        public string MovementType { get; set; } 

        public bool IsPriceChange => ActionType == "PriceChange";
        public bool IsMovement => ActionType == "Movement";
    }

}