using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using groupon.Data;
using groupon.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace groupon.Services
{
    public interface IAccountServices
    {
        Task<UpdateProfileResult> UpdateProfile(string name, int company, string field, string workExperience, string education,
            string location, string picture, string currentlyWorking);
        Task<ApplicationUser> GetUserAsync(string id);
    }

    public class AccountServices : IAccountServices
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _http;

        public AccountServices(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContext)
        {
            _context = context;
            _userManager = userManager;
            _http = httpContext;
        }

        public async Task<ApplicationUser> GetUserAsync(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        public async Task<UpdateProfileResult> UpdateProfile(string name, int company, string field, string workExperience, string education, string location, 
            string picture, string currentlyWorking)
        {
            UpdateProfileResult result = new UpdateProfileResult();

            try
            {
                if (!_http.HttpContext.User.Identity.IsAuthenticated)
                    throw new UnauthorizedAccessException();
                else
                    result.Authorized = "YES";

                var user = await _userManager.GetUserAsync(_http.HttpContext.User);

                if (!String.IsNullOrEmpty(name))
                {
                    user.Name = name;
                    result.Name = "OK";
                }

                if (company != 0)
                {
                    user.Company = company;
                    result.Company = "OK";
                }

                if (!String.IsNullOrEmpty(field))
                {
                    user.Field = field;
                    result.Field = "OK";
                }

                if (!String.IsNullOrEmpty(workExperience))
                {
                    user.WorkExperience = workExperience;
                    result.WorkExperience = "OK";
                }

                if (!String.IsNullOrEmpty(education))
                {
                    user.Education = education;
                    result.Education = "OK";
                }

                if (!String.IsNullOrEmpty(location))
                {
                    user.Location = location;
                    result.Location = "OK";
                }

                if (!String.IsNullOrEmpty(picture))
                {
                    user.Picture = picture;
                    result.Picture = "OK";
                }

                if (!String.IsNullOrEmpty(currentlyWorking))
                {
                    user.CurrentlyWorking = currentlyWorking;
                    result.CurrentlyWorking = "OK";
                }

                await _userManager.UpdateAsync(user);
            }
            catch (Exception ex)
            {
                if (ex is UnauthorizedAccessException)
                    result.Authorized = "NO";
            }

            return result;
        }
    }
}
