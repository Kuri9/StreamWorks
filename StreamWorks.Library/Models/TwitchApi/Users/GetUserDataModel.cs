using System.Text.Json.Serialization;

namespace StreamWorks.Library.Models.TwitchApi.Users;
public class GetUserDataModel
{
    [JsonPropertyName("broadcaster_type")]
    public string? BroadcasterType { get; set; }
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }
    public string? Description { get; set; }
    [JsonPropertyName("display_name")]
    public string? DisplayName { get; set; }
    public string? Id { get; set; }
    public string? Login { get; set; }
    [JsonPropertyName("offline_image_url")]
    public string? OfflineImageUrl { get; set; }
    [JsonPropertyName("profile_image_url")]
    public string? ProfileImageUrl { get; set; }
    public string? Type { get; set; }
    [JsonPropertyName("view_count")]
    public long ViewCount { get; set; }
}
