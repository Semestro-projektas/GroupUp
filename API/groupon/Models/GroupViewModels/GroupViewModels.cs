using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace groupon.Models.GroupViewModels
{
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

    public class GroupMemberRequestViewModel
    {
        public string UserId { get; set; }
        public string Picture { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }

        public GroupMemberRequestViewModel(GroupTeam req)
        {
            UserId = req.UserId;
            Picture = req.User.Picture;
            Name = req.User.Name;
            Title = req.User.Title;
        }
    }
}
