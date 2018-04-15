using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using groupon.Data;
using groupon.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Extensions.Internal;
using Microsoft.EntityFrameworkCore.Internal;

namespace groupon.Services
{
    public interface IMessagingServices
    {
        Task<Result> SendMessage(string recipientId, string text);
        IEnumerable<Message> GetAllMessages(string recipientId, int type);
        IEnumerable<ApplicationUser> GetAllChats();
    }

    public class MessagingServices : IMessagingServices
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _http;

        public MessagingServices(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContext)
        {
            _context = context;
            _userManager = userManager;
            _http = httpContext;
        }

        // Send message
        public async Task<Result> SendMessage(string recipientId, string text)
        {
            Result result = new Result();

            try
            {
                if (!IsAuthenticated())
                    return new Result(ResultType.Unauthorized);

                var cUser = GetCurrentUser().Result;

                if(recipientId == null || text == null)
                    return new Result("Bad request.", 400);

                if (_userManager.FindByIdAsync(recipientId).Result == null)
                    return new Result("Message recipient doesn't exist.", 400);

                if (_userManager.FindByIdAsync(recipientId).Result == cUser)
                    return new Result("You can't send message to yourself.", 400);

                var message = new Message
                {
                    Date = DateTime.Now,
                    RecipientId = recipientId,
                    SenderId = cUser.Id,
                    Text = text
                };

                _context.Add(message);
                _context.SaveChanges(true);
            }
            catch (Exception ex)
            {
                LogException(ex);
                result = new Result(ex);
            }

            return result;
        }

        // GetAllMessagesWithSomeone
        // Type 0 = all, Type 1 - inbox only, Type 2 - sent only
        public IEnumerable<Message> GetAllMessages(string recipientId, int type)
        {
            try
            {
                if (IsAuthenticated() && _userManager.FindByIdAsync(recipientId) != null)
                {
                    var user = GetCurrentUser().Result;

                    if (type == 0)
                        return _context.Messages.Where(i => (i.RecipientId == recipientId && i.SenderId == user.Id)
                                                            || (i.RecipientId == user.Id && i.SenderId == recipientId));
                    else if (type == 0)
                        return _context.Messages.Where(i => i.RecipientId == user.Id && i.SenderId == recipientId);
                    else
                        return _context.Messages.Where(i => i.RecipientId == recipientId && i.SenderId == user.Id);
                }
            }
            catch (Exception ex)
            {
                LogException(ex);
            }

            return null;
        }

        // GetAllWhoYouHadMessagesWith
        // Pratestint sita gerai
        public IEnumerable<ApplicationUser> GetAllChats()
        {
            List<ApplicationUser> allChatters = new List<ApplicationUser>();
            try
            {
                if (IsAuthenticated())
                {
                    var user = GetCurrentUser().Result;

                    var asSender = _context.Messages.Where(i => i.SenderId == user.Id).Select(i => i.RecipientId).Distinct();
                    var asRecipient = _context.Messages.Where(i => i.RecipientId == user.Id).Select(i => i.SenderId).Distinct();

                    var distinctUsers = asSender.Union(asRecipient);

                    foreach (var id in distinctUsers)
                    {
                        var usr = _context.Users.FirstOrDefault(i => i.Id == id);
                        allChatters.Add(usr);
                    }

                    return allChatters;
                }
            }
            catch (Exception ex)
            {
                LogException(ex);
            }

            return null;
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
