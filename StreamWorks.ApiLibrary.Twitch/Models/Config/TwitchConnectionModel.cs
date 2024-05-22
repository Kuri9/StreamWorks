namespace StreamWorks.ApiLibrary.Twitch.Models.Config
{
    public class TwitchConnectionModel
    {
        public TwitchConnectionModel() { 
        
        }

        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string RedirectUri { get; set; }
        public string[] Scopes { get; set; }
        public string RefreshToken { get; set; }
        public string Code { get; set; }
    }
}
