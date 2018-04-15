using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace groupon.Models
{
    public class GroupTeam
    {

        [Key, ForeignKey("Group"), Column(Order = 0)]
        public int GroupId { get; set; }
        [Key, ForeignKey("User"), Column(Order = 1)]
        public string UserId { get; set; }

        public ApplicationUser User { get; set; }
        public Group Group { get; set; }

        [System.ComponentModel.DefaultValue(false)]
        public bool Approved { get; set; }

        public DateTime RequestDate { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public string Comment { get; set; }
    }

    public class CompanyTeam
    {
        [Key, ForeignKey("Company"), Column(Order = 0)]
        public int CompanyId { get; set; }
        [Key, ForeignKey("User"), Column(Order = 1)]
        public string UserId { get; set; }

        public ApplicationUser User { get; set; }
        public Company Company { get; set; }

        [System.ComponentModel.DefaultValue(false)]
        public bool Approved { get; set; }

        public DateTime RequestDate { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public string Comment { get; set; }
    }
}
