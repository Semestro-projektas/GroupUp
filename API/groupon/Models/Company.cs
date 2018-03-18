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
        [ForeignKey("UserId")]
        public virtual ApplicationUser Owner { get; set; }
    }

    public class CompanyListViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Field { get; set; }
        public string Location { get; set; }
        public string ShortDescription { get; set; }
        public string Logo { get; set; }
        public string Owner { get; set; }

        public CompanyListViewModel(Company c)
        {
            Id = c.Id;
            Title = c.Title;
            Field = c.Field;
            Location = c.Location;
            ShortDescription = c.ShortDescription;
            Logo = c.Logo;
            Owner = c.Owner.Name != null ? c.Owner.Name : "";
        }
    }

    public class SingleCompanyViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Field { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public string Logo { get; set; }
        public string Owner { get; set; }

        public SingleCompanyViewModel(Company c)
        {
            Id = c.Id;
            Title = c.Title;
            Field = c.Field;
            Location = c.Location;
            Description = c.Description;
            Logo = c.Logo;
            Owner = c.Owner.Name != null ? c.Owner.Name : "";
        }
    }
}
