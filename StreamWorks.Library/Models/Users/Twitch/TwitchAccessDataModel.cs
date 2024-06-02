using System.Text.Json.Serialization;

namespace StreamWorks.Library.Models.Users.Twitch
{
    public class TwitchAccessDataModel
    {
        [JsonPropertyName("grant_type")]
        public string? GrantType { get; set; }

        [JsonPropertyName("access_token")]
        public string? AccessToken { get; set; }
        
        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }
        public DateTimeOffset? ExpiresAt { get; set; }

        [JsonPropertyName("id_token")]
        public string? IdToken { get; set; }
        
        [JsonPropertyName("refresh_token")]
        public string? RefreshToken { get; set; }
        public string[]? Scope { get; set; }
        
        [JsonPropertyName("token_type")]
        public string? TokenType { get; set; }
    }
}
