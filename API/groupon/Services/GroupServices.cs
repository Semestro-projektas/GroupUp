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
        CreateGroupResult Create(string title, string shortDescription);
        Group Get(int id);
        IEnumerable<Group> GetAll(int? position, int? count);
        IEnumerable<Group> GetHot(int? position, int? count);
        IEnumerable<Group> GetSearchResult(string filter);

        UpdateGroupResult Edit(int id, string title, GroupType type, string shortDescription,
            string description, string image, bool? hot);
        Result AskToJoinRequest(int groupId);
        Result ApproveJoinRequest(string userId, int groupId);
        IEnumerable<GroupTeam> ViewAllJoinRequests(int groupId, int? position, int? count, out string error);
        IEnumerable<Group> GetAllJoinedGroups(string userId);
        IEnumerable<ApplicationUser> GetAllMembers(int groupId);
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

        public CreateGroupResult Create(string title, string shortDescription)
        {
            var result = new CreateGroupResult();

            try
            {
                if (!_http.HttpContext.User.Identity.IsAuthenticated)
                    return new CreateGroupResult(401, "You must be logged in to this.");

                var user = _userManager.GetUserAsync(_http.HttpContext.User).Result;

                if (!String.IsNullOrEmpty(title))
                    result.Title = "OK";
                else
                    result.Title = "NO";

                if (!String.IsNullOrEmpty(shortDescription))
                    result.Description = "OK";
                else
                    result.Description = "NO";

                if (user != null)
                    result.Owner = "OK";
                else
                    result.Owner = "NO";

                if (result.Title == "NO" || result.Description == "NO" || result.Owner == "NO")
                {
                    result.Error = "Not all required fields were filled.";
                    result.StatusCode = 412;
                    return result;
                }
                else
                {
                    result.StatusCode = 200;

                    var newGroup = new Group { ShortDescription = shortDescription, Title = title, Owner = user };
                    result.Id = newGroup.Id;


                    _context.Groups.Add(newGroup);
                    _context.GroupTeam.Add(new GroupTeam
                    {
                        RequestDate = DateTime.Today,
                        ApprovalDate = DateTime.Today,
                        Approved = true,
                        Comment = "Init",
                        GroupId = newGroup.Id,
                        UserId = user.Id
                    });
                    _context.SaveChanges();
                    result.Id = newGroup.Id;
                }

                return result;
            }
            catch (Exception ex)
            {
                LogException(ex);
                return new CreateGroupResult(400, ex.Message);
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
            try
            {
                foreach (var group in _context.Groups)
                {
                    if (group.Title.Contains(filter) || group.Description.Contains(filter) ||
                        group.ShortDescription.Contains(filter))
                        selectedGroups.Add(group);
                }
            }
            catch (Exception ex)
            {
                LogException(ex);
            }

            return selectedGroups.AsEnumerable();
        }

        public UpdateGroupResult Edit(int id, string title, GroupType type, string shortDescription,
            string description, string image, bool? hot)
        {
            var result = new UpdateGroupResult();

            try
            {
                var group = _context.Groups.Include(i => i.Owner).FirstOrDefault(i => i.Id == id);
                var user = _userManager.GetUserAsync(_http.HttpContext.User).Result;

                if (!_http.HttpContext.User.Identity.IsAuthenticated)
                    return new UpdateGroupResult(401, "You must be logged in to do this.");

                if (group == null)
                    return new UpdateGroupResult(404, "Company you're editing is not found.");

                if (group.Owner != user && user.Role != Role.Admin)
                    return new UpdateGroupResult(403, "You don't have permissions to do this.");

                if (!String.IsNullOrEmpty(title))
                {
                    group.Title = title;
                    result.Title = "OK";
                }

                if (type != group.Type)
                {
                    group.Type = type;
                    result.Type = "OK";
                }

                if (!String.IsNullOrEmpty(shortDescription))
                {
                    group.ShortDescription = shortDescription;
                    result.ShortDescription = "OK";
                }

                if (!String.IsNullOrEmpty(description))
                {
                    group.Description = description;
                    result.Description = "OK";
                }

                if (!String.IsNullOrEmpty(image))
                {
                    group.Image = image;
                    result.Image = "OK";
                }

                if (hot != null)
                {
                    if (user.Role == Role.Admin)
                        group.Hot = true;
                    else
                        return new UpdateGroupResult(403, "You don't have required permissions to promote a group.");
                }

                _context.SaveChanges(true);

                return result;
            }
            catch (Exception ex)
            {
                LogException(ex);
                return new UpdateGroupResult(400, ex.Message);
            }
        }

        public Result AskToJoinRequest(int groupId)
        {
            try
            {
                if (!IsAuthenticated())
                    return new Result("You must be logged in to do this.", 401);

                var user = _userManager.GetUserAsync(_http.HttpContext.User).Result;
                var group = _context.Groups.FirstOrDefault(i => i.Id == groupId);
                var req = _context.GroupTeam.FirstOrDefault(i => i.GroupId == groupId && i.UserId == user.Id);

                if (group == null)
                    return new Result("Requested group doesn't exist.", 404);

                if (req != null)
                {
                    if (req.Approved)
                        return new Result("You already belong to this group.", 400);
                    else
                        return new Result("You already sent this request.", 400);
                }

                var request = new GroupTeam { GroupId = groupId, UserId = user.Id, RequestDate = DateTime.Now };

                _context.GroupTeam.Add(request);
                _context.SaveChanges(true);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return new Result(ex.Message, 400);
            }

            return new Result();
        }

        public Result ApproveJoinRequest(string userId, int groupId)
        {
            try
            {
                var user = _userManager.GetUserAsync(_http.HttpContext.User).Result;
                var group = _context.Groups.Include(i => i.Owner).FirstOrDefault(i => i.Id == groupId);
                var request = _context.GroupTeam.FirstOrDefault(i => i.GroupId == groupId);

                if (!IsAuthenticated())
                    return new Result("You're not logged in.", 401);

                // Nereikia checkinti ar company nera null, nes kuriant requesta company tuscia nebus padavinejama.

                if (group == null)
                    return new Result("This group doesn't exist.", 404);

                if (request == null)
                    return new Result("This request doesn't exist.", 404);

                if (group.Owner != user)
                    return new Result("You're not owner of this group.", 401);

                request.Approved = true;
                request.ApprovalDate = DateTime.Now;
                _context.SaveChanges(true);

                return new Result();
            }
            catch (Exception ex)
            {
                LogException(ex);
                return new Result(ex.Message, 400);
            }
        }

        public IEnumerable<GroupTeam> ViewAllJoinRequests(int groupId, int? position, int? count, out string error)
        {
            error = "";
            try
            {
                var user = GetCurrentUser().Result;
                var group = _context.Groups.Include(i => i.Owner).FirstOrDefault(i => i.Id == groupId);

                if (!IsAuthenticated())
                {
                    error = "You must be logged in to do this.";
                    return null;
                }

                if (group == null)
                {
                    error = "Requested company doesn't exist.";
                    return null;
                }

                if (group.Owner != user)
                {
                    error = "You're not this company's owner.";
                    return null;
                }

                if (position.HasValue && count.HasValue)
                    return _context.GroupTeam.Where(i => i.GroupId == groupId).Include(i => i.User).Skip(position.Value).Take(count.Value);
                else
                {
                    return _context.GroupTeam.Where(i => i.GroupId == groupId).Include(i => i.User);
                }
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
            return null;
        }

        public IEnumerable<Group> GetAllJoinedGroups(string email)
        {
            List<Group> groups = new List<Group>();
            try
            {
                var userId = _context.Users.FirstOrDefault(i => i.Email == email).Id;
                var joinedGroups = _context.GroupTeam.Where(i => i.UserId == userId).Select(i => i.GroupId);
                foreach (int id in joinedGroups)
                {
                    groups.Add(_context.Groups.FirstOrDefault(i => i.Id == id));
                }
            }
            catch (Exception ex)
            {
                LogException(ex);
            }

            return groups;
        }

        public IEnumerable<ApplicationUser> GetAllMembers(int groupId)
        {
            List<ApplicationUser> members = new List<ApplicationUser>();
            try
            {
                var groups = _context.GroupTeam.Where(i => i.Approved && i.GroupId == groupId).Select(i => i.UserId);
                foreach (string user in groups)
                {
                    members.Add(_context.Users.FirstOrDefault(i => i.Id == user));
                }
            }
            catch (Exception ex)
            {
                LogException(ex);
            }

            return members;
        }

        #region Private functions
        private bool IsAuthenticated()
        {
            try
            {
                if (_http.HttpContext.User.Identity.IsAuthenticated)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                LogException(ex);
            }

            return false;
        }

        private void LogException(Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex + " " + _http.HttpContext.Request.Query);
        }

        private async Task<ApplicationUser> GetCurrentUser()
        {
            return await _userManager.GetUserAsync(_http.HttpContext.User);
        }
        #endregion
    }
}
