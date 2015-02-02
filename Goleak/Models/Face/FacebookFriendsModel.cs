using System.Collections.Generic;

namespace Goleak.Models
{
    public class FacebookFriendsModel
    {
        public List<FacebookFriend> friendsListing { get; set; }
    }
     
    public class FacebookFriend
    {
        public string name { get; set; }
        public string id { get; set; }
        public string email { get; set; }
        public string source { get; set; }
        
    }
}