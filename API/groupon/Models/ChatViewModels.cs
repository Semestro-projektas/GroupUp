using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace groupon.Models
{
    public class TextMessage
    {
        public DateTime Date { get; set; }
        public string SenderId { get; set; }
        public string SenderName { get; set; }
        public string Message { get; set; }

        public TextMessage(Message msg)
        {
            Date = msg.Date;
            SenderId = msg.SenderId;
            SenderName = msg.Sender.Name;
            Message = msg.Text;
        }
    }

    public class Chat
    {
        public string UserId { get; set; }
        public string Name { get; set; }

        public Chat(ApplicationUser usr)
        {
            UserId = usr.Id;
            Name = usr.Name;
        }
    }
}
