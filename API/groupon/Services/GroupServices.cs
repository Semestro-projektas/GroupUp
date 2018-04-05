using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using groupon.Data;
using groupon.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace groupon.Services
{
    public interface IGroupServices
    {
        Task<Result> CreateAsync(string title, string description);
        Group Get(int id);
        IEnumerable<Group> GetAll(int? position, int? count);
        IEnumerable<Group> GetHot(int? position, int? count);
        IEnumerable<Group> GetSearchResult(string filter);
    }

    public class GroupServices : IGroupServices
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _http;

        public GroupServices(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContext)
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

                var newGroup = new Group();
                newGroup.Title = title;
                var user = await _userManager.GetUserAsync(_http.HttpContext.User);
                newGroup.Owner = user;
                newGroup.Description = description;

                if (newGroup.Title == null || newGroup.Owner == null || newGroup.Description == null)
                    throw new ArgumentException();

                _context.Groups.Add(newGroup);
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

        public Group Get(int id)
        {
            return _context.Groups.Include(p => p.Owner).FirstOrDefault(i => i.Id == id);
        }

        public IEnumerable<Group> GetAll(int? position, int? count)
        {
            if (!position.HasValue && !count.HasValue)
                return _context.Groups.Include(i => i.Owner).AsEnumerable();
            
            return _context.Groups.Include(i => i.Owner).Skip(position.Value).Take(count.Value);
        }

        public IEnumerable<Group> GetHot(int? position, int? count)
        {
            if (!position.HasValue && !count.HasValue)
                return _context.Groups.Where(i => i.Hot).Include(i => i.Owner).AsEnumerable();

            return _context.Groups.Where(i => i.Hot).Include(i => i.Owner).Skip(position.HasValue ? position.Value : 0).Take(count.HasValue ? count.Value : 0);
        }

        public IEnumerable<Group> GetSearchResult(string filter)
        {
            var selectedGroups = new List<Group>();
            foreach (var group in _context.Groups)
            {
                if(group.Title.Contains(filter) || group.Description.Contains(filter) || group.ShortDescription.Contains(filter))
                    selectedGroups.Add(group);
            }

            return selectedGroups.AsEnumerable();
        }
    }
}
