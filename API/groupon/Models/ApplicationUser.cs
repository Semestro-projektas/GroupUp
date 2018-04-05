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
        public int Company { get; set; }
        public string Field { get; set; }
        public string WorkExperience { get; set; }
        public string Education { get; set; }
        public string Location { get; set; }
        public string Picture { get; set; }
        public string CurrentlyWorking { get; set; }
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
        }
    }
}
