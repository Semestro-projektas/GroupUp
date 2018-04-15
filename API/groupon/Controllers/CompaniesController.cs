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
        [Route("/api/companies/")]
        [Route("/api/companies/all")]
        public JsonResult GetAll()
        {
            return Json(_main.GetAll(null, null).Select(i => new CompanyListViewModel(i)));
        }

        [HttpGet]
        [Route("/api/companies/{position}-{count}")]
        public JsonResult GetAll(int position, int count)
        {
            return Json(_main.GetAll(position, count).Select(i => new CompanyListViewModel(i)));
        }

        [HttpGet]
        [Route("/api/companies/{id}")]
        public JsonResult Get(int id)
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
        public async Task<CreateCompanyResult> CreateCompany(string title, string shortDescription)
        {
            return await _main.CreateAsync(title, shortDescription);
        }

        [HttpGet]
        [Route("api/companies/search/")]
        public IEnumerable<CompanySearchListViewModel> GetSearchResult(string filter)
        {
            return _main.GetSearchResult(filter).Select(i => new CompanySearchListViewModel(i));
        }

        [HttpPost]
        [Route("api/companies/edit")]
        public async Task<UpdateCompanyResult> EditCompany(int companyId, string title, string field, string location,
            string description, string shortDescription, string logo, bool? approved)
        {
            return await _main.EditAsync(companyId, title, field, location, description, shortDescription, logo, approved);
        }

        // Nepilnai padarytas
        [HttpPost]
        [Route("api/companies/join")]
        public async Task<Result> JoinCompany(int companyId)
        {
            return await _main.AskToJoinRequestAsync(companyId);
        }

        [HttpPost]
        [Route("api/companies/approve")]
        public async Task<Result> ApproveJoinRequest(int companyId, string userId)
        {
            return await _main.ApproveJoinRequest(userId, companyId);
        }

        [HttpGet]
        [Route("api/companies/requests")]
        public IEnumerable<CompanyMemberRequestViewModel> GetAllRequests(int companyId)
        {
            string test = "";
            return _main.ViewAllJoinRequests(companyId, null, null, out test).Select(i => new CompanyMemberRequestViewModel(i));
        }


    }
}