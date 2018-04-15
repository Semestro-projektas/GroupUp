using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http.Cors;
using groupon.Data;
using groupon.Models;
using groupon.Models.GroupViewModels;
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
        private readonly IGroupServices _main;

        public GroupController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IGroupServices groupServices)
        {
            _context = context;
            _userManager = userManager;
            _main = groupServices;
        }

        [HttpGet]
        [Route("/api/groups/")]
        [Route("/api/groups/all")]
        public JsonResult GetAll()
        {
            return Json(_main.GetAll(null, null).Select(i => new GroupListViewModel(i)));
        }

        // Needs to be filtered by category
        [HttpGet]
        [Route("/api/groups/{position:int}/{count:int}")]
        public JsonResult GetAllGroups(int position, int count)
        {
            return Json(_main.GetAll(position, count).Select(i => new GroupListViewModel(i)));
        }

        [HttpGet]
        [Route("/api/groups/{id}")]
        public JsonResult GetGroup(int id)
        {
            return _main.Get(id) != null
                ? new JsonResult(new SingleGroupViewModel (_main.Get(id)))
                : new JsonResult(new RequestErrorViewModel("Group not found."));
        }

        [HttpGet]
        [Route("api/groups/hot/{position:int}/{count:int}")]
        public JsonResult GetHot(int position, int count)
        {
            return Json(_main.GetHot(position, count).Select(i => new GroupListViewModel(i)));
        }

        [HttpPost]
        [Route("/api/groups/create")]
        public async Task<JsonResult> CreateGroup(string title, string description)
        {
           return Json(await _main.CreateAsync(title, description));
        }
        
        // Can be merged with getAll
        [HttpGet]
        [Route("api/groups/search")]
        public IEnumerable<GroupSearchListViewModel> GetSearchResult(string filter)
        {
            return _main.GetSearchResult(filter).Select(i => new GroupSearchListViewModel(i));
        }

        [HttpPost]
        [Route("api/groups/edit")]
        public async Task<UpdateGroupResult> EditGroup(int groupId, string title, GroupType type,
            string shortDescription, string description, string image, bool? hot)
        {
            return await _main.EditAsync(groupId, title, type, shortDescription, description, image, hot);
        }

        [HttpPost]
        [Route("api/groups/join")]
        public async Task<Result> AskToJoinRequestAsync(int groupId)
        {
            return await _main.AskToJoinRequestAsync(groupId);
        }

        [HttpPost]
        [Route("api/groups/approve")]
        public async Task<Result> ApproveJoinRequest(string userId, int groupId)
        {
            return await _main.ApproveJoinRequest(userId, groupId);
        }

        [HttpGet]
        [Route("api/groups/requests")]
        public IEnumerable<GroupMemberRequestViewModel> GetAllRequests(int groupId, int? position, int? count)
        {
            string test = "";
            return _main.ViewAllJoinRequests(groupId, null, null, out test).Select(i => new GroupMemberRequestViewModel(i));
        }
    }
}