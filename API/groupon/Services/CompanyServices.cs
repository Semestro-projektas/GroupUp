using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Security.AccessControl;
using System.Security.Authentication;
using System.Threading.Tasks;
using groupon.Data;
using groupon.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace groupon.Services
{
    public interface ICompanyServices
    {
        CreateCompanyResult Create(string title, string shortDescription);
        Company Get(int id);
        IEnumerable<Company> GetAll(int? position, int? count);
        IEnumerable<Company> GetApproved(int? position, int? count);
        IEnumerable<Company> GetSearchResult(string filter);
        UpdateCompanyResult Edit(int id, string title, string field, string location, string desc,
            string shortDesc, string logo, bool? approved);
        Result AskToJoinRequest(int companyId);
        Result ApproveJoinRequest(string userId, int companyId);
        IEnumerable<CompanyTeam> ViewAllJoinRequests(int companyId, int? position, int? count, out string error);
    }

    public class CompanyServices : ICompanyServices
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _http;

        public CompanyServices(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContext)
        {
            _context = context;
            _userManager = userManager;
            _http = httpContext;
        }

        public CreateCompanyResult Create(string title, string shortDescription)
        {
            var result = new CreateCompanyResult();

            try
            {
                if (!_http.HttpContext.User.Identity.IsAuthenticated)
                    return new CreateCompanyResult(ResultType.Unauthorized);

                var user = _userManager.GetUserAsync(_http.HttpContext.User).Result;

                if (!String.IsNullOrEmpty(title))
                    result.Title = "OK";
                else
                    result.Title = "NO";

                if (!String.IsNullOrEmpty(shortDescription))
                    result.Description = "OK";
                else
                    result.Description = "NO";

                if (user != null)
                    result.Owner = "OK";
                else
                    result.Owner = "NO";

                if (result.Title == "NO" || result.Description == "NO" || result.Owner == "NO")
                {
                    result.Error = "Not all required fields were filled.";
                    result.StatusCode = 412;
                    return result;
                }
                else
                {
                    result.StatusCode = 200;
                    var newCompany = new Company { ShortDescription = shortDescription, Title = title, Owner = user };
                    _context.Companies.Add(newCompany);
                    _context.CompanyTeam.Add(new CompanyTeam
                    {
                        RequestDate = DateTime.Today,
                        ApprovalDate = DateTime.Today,
                        Approved = true,
                        Comment = "Init",
                        CompanyId = newCompany.Id,
                        UserId = user.Id
                    });
                    _context.SaveChanges();
                }

                return result;
            }
            catch (Exception ex)
            {
                LogException(ex);
                return new CreateCompanyResult(400, ex.Message);
            }
        }

        public Company Get(int id)
        {
            return _context.Companies.Include(p => p.Owner).FirstOrDefault(i => i.Id == id);
        }

        public IEnumerable<Company> GetAll(int? position, int? count)
        {
            if (!position.HasValue && !count.HasValue)
                return _context.Companies.Include(i => i.Owner).AsEnumerable();

            return _context.Companies.Include(i => i.Owner).Skip(position.Value).Take(count.Value);
        }

        public IEnumerable<Company> GetApproved(int? position, int? count)
        {
            if (!position.HasValue && !count.HasValue)
                return _context.Companies.Where(i => i.Approved).Include(i => i.Owner).AsEnumerable();

            return _context.Companies.Where(i => i.Approved).Include(i => i.Owner).Skip(position.HasValue ? position.Value : 0).Take(count.HasValue ? count.Value : 0);
        }

        public IEnumerable<Company> GetSearchResult(string filter)
        {
            var selectedCompanies = new List<Company>();
            foreach (var company in _context.Companies)
            {
                if (company.Title.Contains(filter) || company.Description.Contains(filter) || company.ShortDescription.Contains(filter))
                    selectedCompanies.Add(company);
            }

            return selectedCompanies.AsEnumerable();
        }

        public UpdateCompanyResult Edit(int id, string title, string field, string location, string desc,
            string shortDesc, string logo, bool? approved)
        {
            var result = new UpdateCompanyResult();

            try
            {
                var company = _context.Companies.Include(i => i.Owner).FirstOrDefault(i => i.Id == id);
                var user = _userManager.GetUserAsync(_http.HttpContext.User).Result;

                if (!_http.HttpContext.User.Identity.IsAuthenticated)
                    return new UpdateCompanyResult(401, "You must be logged in to do this.");

                if (company == null)
                    return new UpdateCompanyResult(404, "Company you're editing is not found.");

                if (company.Owner != user && user.Role != Role.Admin)
                    return new UpdateCompanyResult(403, "You don't have permissions to do this.");

                if (!String.IsNullOrEmpty(title))
                {
                    company.Title = title;
                    result.Title = "OK";
                }

                if (!String.IsNullOrEmpty(field))
                {
                    company.Field = field;
                    result.Field = "OK";
                }

                if (!String.IsNullOrEmpty(location))
                {
                    company.Location = location;
                    result.Location = "OK";
                }

                if (!String.IsNullOrEmpty(desc))
                {
                    company.Description = desc;
                    result.Description = "OK";
                }

                if (!String.IsNullOrEmpty(shortDesc))
                {
                    company.ShortDescription = shortDesc;
                    result.ShortDescription = "OK";
                }

                if (!String.IsNullOrEmpty(logo))
                {
                    company.Logo = logo;
                    result.Logo = "OK";
                }

                if (approved != null)
                {
                    if (user.Role == Role.Admin)
                        company.Approved = true;
                    else
                        return new UpdateCompanyResult(403, "You don't have required permissions to approve a company.");
                }


                _context.SaveChanges(true);
                result.StatusCode = 200;

                return result;
            }
            catch (Exception ex)
            {
                LogException(ex);
                return new UpdateCompanyResult(400, ex.Message);
            }
        }

        public Result AskToJoinRequest(int companyId)
        {
            try
            {
                var user = _userManager.GetUserAsync(_http.HttpContext.User).Result;
                var company = _context.Companies.FirstOrDefault(i => i.Id == companyId);
                var req = _context.CompanyTeam.FirstOrDefault(i => i.UserId == user.Id && i.CompanyId == companyId);

                if (!IsAuthenticated())
                    return new Result("You must be logged in to do this.", 401);

                if (company == null)
                    return new Result("Requested company doesn't exist.", 404);

                if (req != null)
                {
                    if (req.Approved)
                        return new Result("You already belong to this group.", 400);
                    else
                        return new Result("You've already sent this request.", 400);
                }

                var request = new CompanyTeam { CompanyId = companyId, UserId = user.Id, RequestDate = DateTime.Now };

                _context.CompanyTeam.Add(request);
                _context.SaveChanges(true);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return new Result(ex.Message, 400);
            }

            return new Result();
        }

        public Result ApproveJoinRequest(string userId, int companyId)
        {
            try
            {
                var user = _userManager.GetUserAsync(_http.HttpContext.User).Result;
                var requestor = _context.Users.FirstOrDefault(i => i.Id == userId);
                var company = _context.Companies.Include(i => i.Owner).FirstOrDefault(i => i.Id == companyId);
                var request = _context.CompanyTeam.FirstOrDefault(i => i.CompanyId == companyId);

                if (!IsAuthenticated())
                    return new Result("You're not logged in.", 401);

                if (requestor == null)
                    return new Result("Such users doesn't exist.", 404);

                // Nereikia checkinti ar company nera null, nes kuriant requesta company tuscia nebus padavinejama.

                if (company == null)
                    return new Result("This company doesn't exist.", 404);

                if (request == null)
                    return new Result("This request doesn't exist.", 404);

                if (company.Owner != user)
                    return new Result("You're not owner of this company.", 401);


                request.Approved = true;
                request.ApprovalDate = DateTime.Now;
                _context.SaveChanges(true);

                return new Result();
            }
            catch (Exception ex)
            {
                LogException(ex);
                return new Result(ex.Message, 400);
            }
        }

        public IEnumerable<CompanyTeam> ViewAllJoinRequests(int companyId, int? position, int? count, out string error)
        {
            error = "";
            try
            {
                var user = GetCurrentUser().Result;
                var company = _context.Companies.Include(i => i.Owner).FirstOrDefault(i => i.Id == companyId);

                if (!IsAuthenticated())
                {
                    error = "You must be logged in to do this.";
                    return null;
                }

                if (company == null)
                {
                    error = "Requested company doesn't exist.";
                    return null;
                }

                if (company.Owner != user)
                {
                    error = "You're not this company's owner.";
                    return null;
                }

                if(position.HasValue && count.HasValue)
                    return _context.CompanyTeam.Where(i => i.CompanyId == companyId).Include(i => i.User).Skip(position.Value).Take(count.Value);
                else
                {
                    return _context.CompanyTeam.Where(i => i.CompanyId == companyId).Include(i => i.User);
                }
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
            return null;
        }

        #region Private functions
        private bool IsAuthenticated()
        {
            try
            {
                if (_http.HttpContext.User.Identity.IsAuthenticated)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to identify authentication: " + ex.Message);
            }

            return false;
        }

        private async Task<ApplicationUser> GetCurrentUser()
        {
            return await _userManager.GetUserAsync(_http.HttpContext.User);
        }

        private void LogException(Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex + " " + _http.HttpContext.Request.Query);
        }
        #endregion
    }
}
