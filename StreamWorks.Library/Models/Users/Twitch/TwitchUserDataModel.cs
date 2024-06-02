using System.Text.Json.Serialization;

namespace StreamWorks.Library.Models.Users.Twitch;
public class TwitchUserDataModel
{
    public string? Aud { get; set; }
    public int Exp { get; set; }
    public int Iat { get; set; }
    public string? Iss { get; set; }
    public string? Sub { get; set; }
    public string? Email { get; set; }
    [JsonPropertyName("email_verified")]
    public bool EmailVerified { get; set; }
    public string? Picture { get; set; }
    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; set; }
}

