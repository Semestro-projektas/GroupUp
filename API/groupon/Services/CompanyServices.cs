using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using groupon.Data;
using groupon.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace groupon.Services
{
    public interface ICompanyServices
    {
        Task<Result> CreateAsync(string title, string description);
        Company Get(int id);
        IEnumerable<Company> GetAll(int? position, int? count);
        IEnumerable<Company> GetApproved(int? position, int? count);
        IEnumerable<Company> GetSearchResult(string filter);
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

        public async Task<Result> CreateAsync(string title, string description)
        {
            try
            {
                if (!_http.HttpContext.User.Identity.IsAuthenticated)
                    throw new UnauthorizedAccessException();

                var newCompany = new Company();
                newCompany.Title = title;
                var user = await _userManager.GetUserAsync(_http.HttpContext.User);
                newCompany.Owner = user;
                newCompany.Description = description;

                if (newCompany.Title == null || newCompany.Owner == null || newCompany.Description == null)
                    throw new ArgumentException();

                _context.Companies.Add(newCompany);
                _context.SaveChanges();

                return new Result();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex + " " + _http.HttpContext.Request.Query);
                if (ex is UnauthorizedAccessException)
                    return new Result("You need to be logged in to do this.");
                else
                    return new Result("Page not found.");
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
    }
}
