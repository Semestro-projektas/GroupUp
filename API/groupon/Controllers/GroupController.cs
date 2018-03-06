using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http.Cors;
using groupon.Data;
using groupon.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace groupon.Controllers
{
    [Produces("application/json")]
    [Route("api/group")]
    [EnableCors(origins: "https://localhost:44330/", headers: "*", methods: "*")]
    public class GroupController : Controller
    {
        private ApplicationDbContext Context;

        public GroupController(ApplicationDbContext context)
        {
            Context = context;
        }

        [HttpGet]
        [Route("/api/groups/all")]
        public JsonResult GetAll()
        {
            return Json(Context.Groups.AsEnumerable());
        }

        [HttpPost]
        [Route("/api/groups/create")]
        public HttpStatusCode Create(string title, int ownerId, string desc)
        {
            try
            {
                var newGroup = new Group();
                newGroup.Title = title;
                newGroup.Owner = ownerId;
                newGroup.Description = desc;

                if (newGroup.Title == null || newGroup.Owner == 0 || newGroup.Description == null)
                    throw new ArgumentException();

                Context.Groups.Add(newGroup);
                Context.SaveChanges();

                return HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                return HttpStatusCode.BadRequest;
            }
        }
    }
}