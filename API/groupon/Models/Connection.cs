using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace groupon.Models
{
    public class Connection
    {
        public DateTime RequestDate { get; set; }

        public string User1Id { get; set; }
        public ApplicationUser User1 { get; set; }
        public string User2Id { get; set; }
        public ApplicationUser User2 { get; set; }

        [System.ComponentModel.DefaultValue(false)]
        public bool Confirmed { get; set; }
        [MaxLength(255)]
        public string Comment { get; set; }
    }
}
