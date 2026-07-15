using System;

namespace SIMS_Akura.Models
{
    public class Category
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }

        // Optional: to show creator name on UI joins
        public string CreatedByName { get; set; }
    }
}
