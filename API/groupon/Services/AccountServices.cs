using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using groupon.Data;
using groupon.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Rewrite.Internal.UrlMatches;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace groupon.Services
{
    public interface IAccountServices
    {
        UpdateProfileResult UpdateProfile(string name, int company, string field, string workExperience, string education,
            string location, string picture, string currentlyWorking);
        ApplicationUser GetUser(string id);
        Result InviteToConnect(string userId, string comment);
        Result ApproveConnection(string userId);
        IEnumerable<Connection> GetAllConnectionRequests(int? position, int? count);
        IEnumerable<Connection> GetAllUserConnection(int? position, int? count);
        Task<Result> Login(string email, string password, bool rememberMe);
        Task<Result> Logout();
        Result UpdateField(string newField);
        Result RemoveField(string field);
        IEnumerable<string> GetAllUsersFields(string userId);
    }

    public class AccountServices : IAccountServices
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _http;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<AccountServices> _logger;

        public AccountServices(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContext,
            ILogger<AccountServices> logger, SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _http = httpContext;
            _logger = logger;
            _signInManager = signInManager;
        }

        public async Task<Result> Login(string email, string password, bool rememberMe)
        {
            var result = await _signInManager.PasswordSignInAsync(email, password, rememberMe, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                _logger.LogInformation(String.Format("User {0} logged in.", email));
                return new Result();
            }
            else if (result.IsLockedOut)
            {
                _logger.LogInformation(String.Format("Attempt to log in to locked user {0}.", email));
                return new Result("User locked.", 401);
            }
            else
            {
                _logger.LogInformation(String.Format("Bad attempt to log in to {0}.", email));
                return new Result("Invalid login attempt.", 400);
            }
        }

        public async Task<Result> Logout()
        {
            var result = _signInManager.SignOutAsync();
            if (result.IsCompletedSuccessfully)
            {
                _logger.LogInformation("User logged out.");
                return new Result();
            }
            else
            {
                _logger.LogInformation("Failed log out attempt.");
                return new Result("Failed to log out.", 400);
            }
        }

        public ApplicationUser GetUser(string id)
        {
            return _userManager.FindByIdAsync(id).Result;
        }

        public UpdateProfileResult UpdateProfile(string name, int company, string field, string workExperience, string education, string location, 
            string picture, string currentlyWorking)
        {
            UpdateProfileResult result = new UpdateProfileResult();

            try
            {
                if (!IsAuthenticated())
                    return new UpdateProfileResult(ResultType.Unauthorized);

                var user = _userManager.GetUserAsync(_http.HttpContext.User).Result;

                if (!String.IsNullOrEmpty(name))
                {
                    user.Name = name;
                    result.Name = "OK";
                }

                if (company != 0)
                {
                    user.Company = company;
                    result.Company = "OK";
                }

                if (!String.IsNullOrEmpty(field))
                {
                    user.Field = field;
                    result.Field = "OK";
                }

                if (!String.IsNullOrEmpty(workExperience))
                {
                    user.WorkExperience = workExperience;
                    result.WorkExperience = "OK";
                }

                if (!String.IsNullOrEmpty(education))
                {
                    user.Education = education;
                    result.Education = "OK";
                }

                if (!String.IsNullOrEmpty(location))
                {
                    user.Location = location;
                    result.Location = "OK";
                }

                if (!String.IsNullOrEmpty(picture))
                {
                    user.Picture = picture;
                    result.Picture = "OK";
                }

                if (!String.IsNullOrEmpty(currentlyWorking))
                {
                    user.CurrentlyWorking = currentlyWorking;
                    result.CurrentlyWorking = "OK";
                }

                _context.SaveChanges(true);
            }
            catch (Exception ex)
            {
                LogException(ex);
                result.StatusCode = 400;
                result.Error = ex.Message;
            }

            return result;
        }

        public Result InviteToConnect(string userId, string comment)
        {
            Result result = new Result();

            try
            {
                if(!IsAuthenticated())
                    return new Result(ResultType.Unauthorized);

                var user = _userManager.GetUserAsync(_http.HttpContext.User).Result;

                if(userId == null)
                    return new Result("Bad request.", 400);

                if (_userManager.FindByIdAsync(userId) == null)
                    return new Result("Requested user doesn't exist.", 400);

                // Check if the other users didn't invite this user before, if yes then confirm connection
                if (_context.Connections.FirstOrDefault(i => i.User1Id == userId && i.User2Id == user.Id) != null)
                {
                    _context.Connections.FirstOrDefault(i => i.User1Id == userId && i.User2Id == user.Id).Confirmed = true;
                }
                else if(_context.Connections.FirstOrDefault(i => i.User1Id == user.Id && i.User2Id == userId) != null)
                {
                    return new Result("Request has already been made.", 400);
                }
                else
                {
                    var request = new Connection
                    {
                        Comment = comment,
                        Confirmed = false,
                        RequestDate = DateTime.Now,
                        User1Id = user.Id, // Request sender
                        User2Id = userId // The one who got request
                    };

                    _context.Connections.Add(request);
                }
                _context.SaveChanges(true);
            }
            catch (Exception ex)
            {
                LogException(ex);
                result = new Result(ex);
            }

            return result;
        }

        public Result ApproveConnection(string userId)
        {
            Result result = new Result();

            try
            {
                if(!IsAuthenticated())
                    return new Result(ResultType.Unauthorized);

                if (_userManager.FindByIdAsync(userId) == null)
                    return new Result("Requested user doesn't exist.", 400);

                var user = _userManager.GetUserAsync(_http.HttpContext.User).Result;
                var connection = _context.Connections.FirstOrDefault(i => i.User1Id == userId && i.User2Id == user.Id);

                if(connection == null)
                    return new Result("There's no invitation to approve.", 400);

                connection.Confirmed = true;
                _context.SaveChanges(true);
            }
            catch (Exception ex)
            {
                LogException(ex);
                result = new Result(ex);
            }

            return result;
        }

        // GetAllRequestedConnections
        public IEnumerable<Connection> GetAllConnectionRequests(int? position, int? count)
        {
            try
            {
                var user = GetCurrentUser().Result;

                if (IsAuthenticated() && user != null)
                {
                    if (position.HasValue && count.HasValue)
                        return _context.Connections.Where(i => i.User2Id == user.Id && !i.Confirmed).Skip(position.Value).Take(count.Value);
                    else
                       return _context.Connections.Where(i => i.User2Id == user.Id && !i.Confirmed);
                }            
            }
            catch (Exception ex)
            {
                LogException(ex);
            }

            return null;
        }

        // ViewAllProfileConnections
        public IEnumerable<Connection> GetAllUserConnection(int? position, int? count)
        {
            try
            {
                if (IsAuthenticated())
                {
                    var user = GetCurrentUser().Result;

                    var initiator = _context.Connections.Where(i => i.User1Id == user.Id);
                    var requested = _context.Connections.Where(i => i.User2Id == user.Id);

                    return initiator.Union(requested).AsEnumerable();
                }
            }
            catch (Exception ex)
            {
                LogException(ex);
            }

            return null;
        }

        public Result UpdateField(string newField)
        {
            Result result = new Result();
            try
            {
                if (!IsAuthenticated())
                    return new Result(ResultType.Unauthorized);

                var user = GetCurrentUser().Result;
           
                if(user.Field != null && Regex.IsMatch(user.Field, String.Format("/{0}/", newField)))
                    return new Result("You already added this field.", 400);

                if (user.Field != null)
                {
                    MatchCollection matches = Regex.Matches(user.Field, @"<(.+?)>");

                    if (matches.Count >= 5)
                        return new Result("You can't add more than 5 fields.", 400);
                }

                if (user.Field != null)
                    user.Field = user.Field + String.Format("<{0}>", newField);
                else
                    user.Field = String.Format("<{0}>", newField);

                _context.SaveChanges(true);
            }
            catch (Exception ex)
            {
                LogException(ex);
                result = new Result(ex);
            }

            return result;
        }

        public Result RemoveField(string field)
        {
            Result result = new Result();
            try
            {
                if (!IsAuthenticated())
                    return new Result(ResultType.Unauthorized);

                var user = GetCurrentUser().Result;

                if (user.Field == null || !user.Field.Contains(String.Format("<{0}>", field)))
                    return new Result("This field doesn't exist.", 400);

                user.Field = user.Field.Replace(String.Format("<{0}>", field), "");
                _context.SaveChanges(true);
            }
            catch (Exception ex)
            {
                LogException(ex);
                result = new Result(ex);
            }

            return result;
        }

        public IEnumerable<string> GetAllUsersFields(string userId)
        {
            List<string> fields = new List<string>();
            try
            {
                if (!IsAuthenticated())
                    return null;

                var user = _userManager.FindByIdAsync(userId).Result;

                if (user == null)
                    return null;

                if (user.Field == null)
                    return null;

                MatchCollection matches = Regex.Matches(user.Field, @"<(.+?)>");

                foreach (var match in matches)
                {
                    var cut = match.ToString().Remove(0, 1).Remove(match.ToString().Length - 2, 1);
                    fields.Add(cut);
                }
            }
            catch (Exception ex)
            {
                LogException(ex);
            }

            return fields;
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
                Console.WriteLine("Failed to identify authentication: " + ex.Message);
            }

            return false;
        }

        private async Task<ApplicationUser> GetCurrentUser()
        {
            return await _userManager.GetUserAsync(_http.HttpContext.User);
        }

        private void LogException(Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex + " " + _http.HttpContext.Request.Query);
        }
        #endregion

    }
}
