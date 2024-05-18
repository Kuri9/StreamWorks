using System;

namespace StreamWorks.TwitchLibrary.Models
{
    public class TwitchLoginData
    {
        Guid UserId { get; set; } 
        public string ClientId { get; set; }
        public string AccessToken { get; set; }
    }
}
