﻿using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http.Cors;
using groupon.Data;
using groupon.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace groupon.Controllers
{
    [Produces("application/json")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class GroupController : Controller
    {
        private readonly ApplicationDbContext Context;
        private readonly UserManager<ApplicationUser> UserManager;

        public GroupController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            Context = context;
            UserManager = userManager;
        }

        [HttpGet]
        [Route("/api/groups/")]
        [Route("/api/groups/all")]
        public JsonResult GetAll()
        {
            return Json(Context.Groups.Include(i => i.Owner).Select(i => new GroupListViewModel(i)).AsEnumerable());
        }

        [HttpGet]
        [Route("/api/groups/{position}-{count}")]
        public JsonResult GetAll(int position, int count)
        {
            var groups = Context.Groups.Include(i => i.Owner).Select(i => new GroupListViewModel(i));

            return Json(groups.Skip(position).Take(count));
        }

        [HttpGet]
        [Route("/api/groups/{id}")]
        public JsonResult Get(int id)
        {
            var entry = Context.Groups.Include(p => p.Owner).FirstOrDefault(i => i.Id == id);
            if (entry != null)
                return Json(new SingleGroupViewModel(entry));
            else
                return Json("No such entry exists");
        }

        [HttpPost]
        [Route("/api/groups/create")]
        public async Task<HttpStatusCode> Create(string title, string desc)
        {
            try
            {
                if (!User.Identity.IsAuthenticated)
                    throw new UnauthorizedAccessException();

                var newGroup = new Group();
                newGroup.Title = title;
                var user = await UserManager.GetUserAsync(HttpContext.User);
                newGroup.Owner = user;
                newGroup.Description = desc;

                if (newGroup.Title == null || newGroup.Owner == null || newGroup.Description == null)
                    throw new ArgumentException();

                Context.Groups.Add(newGroup);
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