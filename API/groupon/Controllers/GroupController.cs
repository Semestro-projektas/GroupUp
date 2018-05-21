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
using Microsoft.AspNetCore.Authorization;
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

        #region Views

        [HttpGet]
        [Route("/groups/")]
        public IActionResult GroupOverview(int id)
        {
            return View();
        }

        #endregion

        #region API calls
        [HttpGet]
        [Route("/api/groups/")]
        [Route("/api/groups/all")]
        public IEnumerable<GroupListViewModel> GetAll()
        {
            var result = _main.GetAll(null, null).Select(i => new GroupListViewModel(i));

            return result;
        }

        // Needs to be filtered by category
        [HttpGet]
        [Route("/api/groups/{position:int}/{count:int}")]
        public IEnumerable<GroupListViewModel> GetAllGroups(int position, int count)
        {
            var result = _main.GetAll(position, count).Select(i => new GroupListViewModel(i));

            return result;
        }

        [HttpGet]
        [Route("/api/groups/{id}")]
        public IActionResult GetGroup(int id)
        {
            var result = _main.Get(id);

            if (result != null)
                return Json(new SingleGroupViewModel(result));

            return Json(new RequestErrorViewModel("Group not found."));
        }

        [HttpGet]
        [Route("api/groups/hot/{position:int}/{count:int}")]
        public IEnumerable<GroupListViewModel> GetHot(int position, int count)
        {
            var result = _main.GetHot(position, count).Select(i => new GroupListViewModel(i));

            return result;
        }

        [HttpPost]
        [Route("/api/groups/create")]
        public IActionResult CreateGroup(string title, string description)
        {
            var result = _main.Create(title, description);

            return StatusCode(result.StatusCode, result);
        }
        
        // Can be merged with getAll
        [HttpGet]
        [Route("api/groups/search")]
        public IEnumerable<GroupSearchListViewModel> GetSearchResult(string filter)
        {
            var result = _main.GetSearchResult(filter).Select(i => new GroupSearchListViewModel(i));

            return result;
        }

        [HttpPost]
        [Route("api/groups/edit")]
        public IActionResult EditGroup(int groupId, string title, GroupType type,
            string shortDescription, string description, string image, bool? hot)
        {
            var result = _main.Edit(groupId, title, type, shortDescription, description, image, hot);

            return StatusCode(result.StatusCode, result);
        }

        [HttpPost]
        [Route("api/groups/join")]
        public IActionResult AskToJoinRequest(int groupId)
        {
            var result = _main.AskToJoinRequest(groupId);

            return StatusCode(result.StatusCode, result);
        }

        [HttpPost]
        [Route("api/groups/approve")]
        public IActionResult ApproveJoinRequest(string userId, int groupId)
        {
            var result = _main.ApproveJoinRequest(userId, groupId);

            return StatusCode(result.StatusCode, result);
        }

        [HttpGet]
        [Route("api/groups/requests")]
        public IEnumerable<GroupMemberRequestViewModel> GetAllRequests(int groupId, int? position, int? count)
        {
            string test = "";
            var result = _main.ViewAllJoinRequests(groupId, null, null, out test)
                .Select(i => new GroupMemberRequestViewModel(i));

            return result;
        }

        [HttpGet]
        [Route("api/groups/joined")]
        [Authorize]
        public IEnumerable<GroupListViewModel> GetMyGroups()
        {
            var result = _main.GetAllJoinedGroups(HttpContext.User.Identity.Name).Select(i => new GroupListViewModel(i));

            return result;
        }
        #endregion
    }
}