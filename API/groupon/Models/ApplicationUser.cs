using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace groupon.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        [System.ComponentModel.DefaultValue(Role.User)]
        public Role Role { get; set; }
        public int Company { get; set; }
        public string Field { get; set; }
        public string WorkExperience { get; set; }
        public string Education { get; set; }
        public string Location { get; set; }
        public string Picture { get; set; }
        public string Title { get; set; }
        public string CurrentlyWorking { get; set; }

        public virtual List<Connection> Connections { get; set; }
    }

    public enum Role
    {
        User,
        Admin
    }

    public class ProfileEditViewModel
    {

    }

    public class ProfileOverviewModel
    {
        public string Name { get; set; }
        public int Company { get; set; }
        public string Field { get; set; }
        public string WorkExperience { get; set; }
        public string Education { get; set; }
        public string Location { get; set; }
        public string Picture { get; set; }
        public string CurrentlyWorking { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Title { get; set; }

        public ProfileOverviewModel(ApplicationUser user)
        {
            Name = user.Name;
            Company = user.Company;
            Field = user.Field;
            WorkExperience = user.WorkExperience;
            Education = user.Education;
            Location = user.Location;
            Picture = user.Picture;
            CurrentlyWorking = user.CurrentlyWorking;
            Email = user.Email;
            PhoneNumber = user.PhoneNumber;
            Title = user.Title;
        }
    }

    public class UserShortViewModel
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }

        public UserShortViewModel(ApplicationUser user)
        {
            UserId = user.Id;
            Name = user.Name;
            Title = user.Title;
            Image = user.Picture;
        }
    }
}
