using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace groupon.Models
{
    public class Group
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public int Owner { get; set; }
        public string Description { get; set; }
    }
}
