using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIMS_Akura.Models
{
    public class Supplier
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long? AccountId { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public int RowNo { get; set; }


        // Optional joined info
        public string AccountName { get; set; }
        public string AccountCode { get; set; }
    }
}