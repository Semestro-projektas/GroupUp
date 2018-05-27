using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace groupon.Models
{
    public class Message
    {
        public DateTime Date { get; set; }
        [Key, ForeignKey("Sender"), Column(Order = 0)]
        public string SenderId { get; set; }
        [Key, ForeignKey("Recipient"), Column(Order = 1)]
        public string RecipientId { get; set; }

        public string Text { get; set; }

        public virtual ApplicationUser Sender { get; set; }
        public virtual ApplicationUser Recipient { get; set; }
    }
}
