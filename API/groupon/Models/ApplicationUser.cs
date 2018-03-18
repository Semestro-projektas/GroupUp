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
    }
}
