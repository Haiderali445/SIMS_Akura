using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIMS_Akura.Models
{
    public class ProductToggleModel
    {
        public long ProductId { get; set; }
        public bool IsActive { get; set; }
        public long UpdatedBy { get; set; }

    }
}