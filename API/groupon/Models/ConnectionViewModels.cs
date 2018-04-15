using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace groupon.Models
{
    public class ConnectionViewModel
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }

        // This one for viewing all the connections
        public ConnectionViewModel(Connection conn, string currentUserId)
        {
            if (conn.User1Id == currentUserId)
            {
                UserId = conn.User2Id;
                Name = conn.User2.Name;
                Title = conn.User2.Title;
            }
            else
            {
                UserId = conn.User1Id;
                Name = conn.User1.Name;
                Title = conn.User1.Title;
            }
        }

        // This one for viewing all unaccepted requests
        public ConnectionViewModel(Connection conn)
        {
            UserId = conn.User1Id;
            Name = conn.User1.Name;
            Title = conn.User1.Title;
        }
    }
}
