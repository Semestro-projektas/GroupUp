using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace groupon.Models
{
    public class Company
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Field { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public string ShortDescription { get; set; }
        public string Logo { get; set; }
        public bool Approved { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser Owner { get; set; }
    }


}
