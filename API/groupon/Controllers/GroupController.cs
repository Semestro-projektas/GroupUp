using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http.Cors;
using groupon.Data;
using groupon.Models;
using groupon.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace groupon.Controllers
{
    [Produces("application/json")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class GroupController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IGroupServices _gServices;

        public GroupController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IGroupServices groupServices)
        {
            _context = context;
            _userManager = userManager;
            _gServices = groupServices;
        }

        [HttpGet]
        [Route("/api/groups/")]
        [Route("/api/groups/all")]
        public JsonResult GetAll()
        {
            return Json(_gServices.GetAll(null, null).Select(i => new GroupListViewModel(i)));
        }

        [HttpGet]
        [Route("/api/groups/{position:int}/{count:int}")]
        public JsonResult GetAll(int position, int count)
        {
            return Json(_gServices.GetAll(position, count).Select(i => new GroupListViewModel(i)));
        }

        [HttpGet]
        [Route("/api/groups/{id}")]
        public JsonResult Get(int id)
        {
            return _gServices.Get(id) != null
                ? new JsonResult(new SingleGroupViewModel (_gServices.Get(id)))
                : new JsonResult(new RequestErrorViewModel("Group not found."));
        }

        [HttpGet]
        [Route("api/groups/hot/{position:int}/{count:int}")]
        public JsonResult GetHot(int position, int count)
        {
            return Json(_gServices.GetHot(position, count).Select(i => new GroupListViewModel(i)));
        }

        [HttpPost]
        [Route("/api/groups/create")]
        public async Task<JsonResult> Create(string title, string description)
        {
           return Json(await _gServices.CreateAsync(title, description));
        }

        [HttpGet]
        [Route("api/groups/search/")]
        public JsonResult GetSearchResult(string filter)
        {
            return Json(_gServices.GetSearchResult(filter).Select(i => new GroupSearchListViewModel(i)));
        }
    }
}