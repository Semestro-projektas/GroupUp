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
using groupon.Services;

namespace groupon.Controllers
{
    [Produces("application/json")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class CompaniesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICompanyServices _cServices;

        public CompaniesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ICompanyServices companyServices)
        {
            _context = context;
            _userManager = userManager;
            _cServices = companyServices;
        }

        [HttpGet]
        [Route("/api/companies/")]
        [Route("/api/companies/all")]
        public JsonResult GetAll()
        {
            return Json(_cServices.GetAll(null, null).Select(i => new CompanyListViewModel(i)));
        }

        [HttpGet]
        [Route("/api/companies/{position}-{count}")]
        public JsonResult GetAll(int position, int count)
        {
            return Json(_cServices.GetAll(position, count).Select(i => new CompanyListViewModel(i)));
        }

        [HttpGet]
        [Route("/api/companies/{id}")]
        public JsonResult Get(int id)
        {
            return _cServices.Get(id) != null
                ? new JsonResult(new SingleCompanyViewModel(_cServices.Get(id)))
                : new JsonResult(new RequestErrorViewModel("Company not found."));
        }

        [HttpGet]
        [Route("api/companies/hot/{position:int}/{count:int}")]
        public JsonResult GetHot(int position, int count)
        {
            return Json(_cServices.GetApproved(position, count).Select(i => new CompanyListViewModel(i)));
        }

        [HttpPost]
        [Route("/api/companies/create")]
        public async Task<JsonResult> Create(string title, string desc)
        {
            return Json(await _cServices.CreateAsync(title, desc));
        }

        [HttpGet]
        [Route("api/companies/search/")]
        public JsonResult GetSearchResult(string filter)
        {
            return Json(_cServices.GetSearchResult(filter).Select(i => new CompanySearchListViewModel(i)));
        }
    }
}