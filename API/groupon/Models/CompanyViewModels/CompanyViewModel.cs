using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace groupon.Models.CompanyViewModels
{
    public class CompanyListViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Field { get; set; }
        public string Location { get; set; }
        public string ShortDescription { get; set; }
        public string Logo { get; set; }
        public string Owner { get; set; }
        public bool Approved { get; set; }

        public CompanyListViewModel(Company c)
        {
            Id = c.Id;
            Title = c.Title;
            Field = c.Field;
            Location = c.Location;
            ShortDescription = c.ShortDescription;
            Logo = c.Logo;
            Owner = c.Owner.Name != null ? c.Owner.Name : "";
            Approved = c.Approved;
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
        public bool Approved { get; set; }

        public SingleCompanyViewModel(Company c)
        {
            Id = c.Id;
            Title = c.Title;
            Field = c.Field;
            Location = c.Location;
            Description = c.Description;
            Logo = c.Logo;
            Owner = c.Owner.Name != null ? c.Owner.Name : "";
            Approved = c.Approved;
        }
    }

    public class CompanySearchListViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Location { get; set; }
        public string Logo { get; set; }
        public string Owner { get; set; }
        public bool Approved { get; set; }

        public CompanySearchListViewModel(Company c)
        {
            Id = c.Id;
            Title = c.Title;
            Location = c.Location;
            Logo = c.Logo;
            Owner = c.Owner.Name != null ? c.Owner.Name : "";
            Approved = c.Approved;
        }
    }

    public class CompanyMemberRequestViewModel
    {
        public string UserId { get; set; }
        public string Picture { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }

        public CompanyMemberRequestViewModel(CompanyTeam req)
        {
            UserId = req.UserId;
            Picture = req.User.Picture;
            Name = req.User.Name;
            Title = req.User.Title;
        }
    }
}
