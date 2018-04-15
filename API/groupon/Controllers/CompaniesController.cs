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
using groupon.Models.CompanyViewModels;
using groupon.Services;
using Microsoft.AspNetCore.Authorization;

namespace groupon.Controllers
{
    [Produces("application/json")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class CompaniesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICompanyServices _main;

        public CompaniesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ICompanyServices companyServices)
        {
            _context = context;
            _userManager = userManager;
            _main = companyServices;
        }

        [HttpGet]
        [Route("/api/companies/all")]
        public IActionResult GetAll()
        {
            return Json(_main.GetAll(null, null).Select(i => new CompanyListViewModel(i)));
        }

        [HttpGet]
        [Route("/api/companies/all/{position}-{count}")]
        public IActionResult GetAll(int position, int count)
        {
            return Json(_main.GetAll(position, count).Select(i => new CompanyListViewModel(i)));
        }

        [HttpGet]
        [Route("/api/companies/{id}")]
        public IActionResult Get(int id)
        {
            return _main.Get(id) != null
                ? new JsonResult(new SingleCompanyViewModel(_main.Get(id)))
                : new JsonResult(new RequestErrorViewModel("Company not found."));
        }

        [HttpGet]
        [Route("api/companies/hot/{position:int}/{count:int}")]
        public IEnumerable<CompanyListViewModel> GetHot(int position, int count)
        {
            return _main.GetApproved(position, count).Select(i => new CompanyListViewModel(i));
        }

        [HttpPost]
        [Route("/api/companies/create")]
        [Authorize] 
        public IActionResult CreateCompany(string title, string shortDescription)
        {
            var result = _main.Create(title, shortDescription);

            return StatusCode(result.StatusCode, result);
        }

        [HttpGet]
        [Route("api/companies/search/")]
        public IEnumerable<CompanySearchListViewModel> GetSearchResult(string filter)
        {
            return _main.GetSearchResult(filter).Select(i => new CompanySearchListViewModel(i));
        }

        [HttpPost]
        [Route("api/companies/edit")]
        [Authorize]
        public IActionResult EditCompany(int companyId, string title, string field, string location,
            string description, string shortDescription, string logo, bool? approved)
        {
            var result = _main.Edit(companyId, title, field, location, description, shortDescription, logo, approved);

            return StatusCode(result.StatusCode, result);
        }

        [HttpPost]
        [Route("api/companies/join")]
        [Authorize]
        public IActionResult JoinCompany(int companyId)
        {
            var result = _main.AskToJoinRequest(companyId);

            return StatusCode(result.StatusCode, result);
        }

        [HttpPost]
        [Route("api/companies/approve")]
        [Authorize]
        public IActionResult ApproveJoinRequestAsync(int companyId, string userId)
        {
            var result = _main.ApproveJoinRequest(userId, companyId);

            return StatusCode(result.StatusCode, result);
        }

        [HttpGet]
        [Route("api/companies/requests")]
        [Authorize]
        public IEnumerable<CompanyMemberRequestViewModel> GetAllRequests(int companyId)
        {
            string test = "";
            return _main.ViewAllJoinRequests(companyId, null, null, out test).Select(i => new CompanyMemberRequestViewModel(i));
        }
    }
}