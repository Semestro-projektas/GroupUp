using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;

namespace groupon.Models
{
    public class Group
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public GroupType Type { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser Owner { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public bool Hot { get; set; }
    }

    public enum GroupType
    {
        Job,
        Startup,
    }

    public class SingleGroupViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public GroupType Type { get; set; }
        public string Owner { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }

        public SingleGroupViewModel(Group group)
        {
            Id = group.Id;
            Title = group.Title;
            Type = group.Type;
            Owner = group.Owner.Name != null ? group.Owner.Name : "";
            Image = group.Image;
            Description = group.Description;
        }
    }

    public class GroupListViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public GroupType Type { get; set; }
        public string Owner { get; set; }
        public string Image { get; set; }
        public string ShortDescription { get; set; }

        public GroupListViewModel(Group group)
        {
            Id = group.Id;
            Title = group.Title;
            Type = group.Type;
            Owner = group.Owner.Name != null ? group.Owner.Name : "";
            Image = group.Image;
            ShortDescription = group.ShortDescription;
        }
    }

    public class GroupSearchListViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public GroupType Type { get; set; }
        public string Image { get; set; }
        public string ShortDescription { get; set; }

        public GroupSearchListViewModel(Group group)
        {
            Id = group.Id;
            Title = group.Title;
            Type = group.Type;
            Image = group.Image;
            ShortDescription = group.ShortDescription;
        }
    }
}
