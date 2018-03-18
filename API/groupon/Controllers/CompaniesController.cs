using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using groupon.Data;
using groupon.Models;
using Microsoft.AspNetCore.Identity;
using System.Net;
using System.Web.Http.Cors;

namespace groupon.Controllers
{
    [Produces("application/json")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class CompaniesController : Controller
    {
        private readonly ApplicationDbContext Context;
        private readonly UserManager<ApplicationUser> UserManager; 

        public CompaniesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            Context = context;
            UserManager = userManager;
        }

        [HttpGet]
        [Route("/api/companies/")]
        [Route("/api/companies/all")]
        public JsonResult GetAll()
        {
            return Json(Context.Companies.Include(i => i.Owner).Select(i => new CompanyListViewModel(i)).AsEnumerable());
        }

        [HttpGet]
        [Route("/api/companies/{position}-{count}")]
        public JsonResult GetAll(int position, int count)
        {
            var groups = Context.Companies.Include(i => i.Owner).Select(i => new CompanyListViewModel(i));

            return Json(groups.Skip(position).Take(count));
        }

        [HttpGet]
        [Route("/api/companies/{id}")]
        public JsonResult Get(int id)
        {
            var entry = Context.Companies.Include(p => p.Owner).FirstOrDefault(i => i.Id == id);
            if (entry != null)
                return Json(new SingleCompanyViewModel(entry));
            else
                return Json(new NotFoundResult());
        }


        [HttpPost]
        [Route("/api/companies/create")]
        public async Task<HttpStatusCode> Create(string title, string desc)
        {
            try
            {
                if (!User.Identity.IsAuthenticated)
                    throw new UnauthorizedAccessException();

                var company = new Company();
                company.Title = title;
                var user = await UserManager.GetUserAsync(HttpContext.User);
                company.Owner = user;
                company.Description = desc;

                if (company.Title == null || company.Owner == null || company.Description == null)
                    throw new ArgumentException();

                Context.Companies.Add(company);
                Context.SaveChanges();

                return HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                if (ex is UnauthorizedAccessException)
                    return HttpStatusCode.Unauthorized;
                else
                    return HttpStatusCode.BadRequest;
            }
        }
    }
}