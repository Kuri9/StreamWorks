using System.Collections.Generic;
using TwitchLib.Api.Core.Enums;

namespace StreamWorks.ApiLibrary.Twitch.Models.Config
{
    public class TwitchScope
    {
        public List<string> Scopes { get; set; }

        public TwitchScope()
        {
            AuthScopes[] twitchScopes = new AuthScopes[]
            {
                   AuthScopes.OpenId,
                   AuthScopes.Helix_Analytics_Read_Extensions,
                   AuthScopes.Helix_Analytics_Read_Games,
                   AuthScopes.Helix_Bits_Read,
                   AuthScopes.Helix_Channel_Edit_Commercial,
                   AuthScopes.Helix_Channel_Manage_Broadcast,
                   AuthScopes.Helix_Channel_Manage_Extensions,
                   AuthScopes.Helix_Channel_Manage_Moderators,
                   AuthScopes.Helix_Channel_Manage_Polls,
                   AuthScopes.Helix_Channel_Manage_Predictions,
                   AuthScopes.Helix_Channel_Manage_Redemptions,
                   AuthScopes.Helix_Channel_Manage_Schedule,
                   AuthScopes.Helix_Channel_Manage_VIPs,
                   AuthScopes.Helix_Channel_Manage_Videos,
                   AuthScopes.Helix_Channel_Read_Charity,
                   AuthScopes.Helix_Channel_Read_Editors,
                   AuthScopes.Helix_Channel_Read_Goals,
                   AuthScopes.Helix_Channel_Read_Hype_Train,
                   AuthScopes.Helix_Channel_Read_Polls,
                   AuthScopes.Helix_Channel_Read_Predictions,
                   AuthScopes.Helix_Channel_Read_Redemptions,
                   AuthScopes.Helix_Channel_Read_Stream_Key,
                   AuthScopes.Helix_Channel_Read_Subscriptions,
                   AuthScopes.Helix_Channel_Read_VIPs,
                   AuthScopes.Helix_User_Edit,
                   AuthScopes.Helix_User_Edit_Broadcast,
                   AuthScopes.Helix_User_Edit_Follows,
                   AuthScopes.Helix_User_Manage_BlockedUsers,
                   AuthScopes.Helix_User_Manage_Chat_Color,
                   AuthScopes.Helix_User_Manage_Whispers,
                   AuthScopes.Helix_User_Read_BlockedUsers,
                   AuthScopes.Helix_User_Read_Broadcast,
                   AuthScopes.Helix_User_Read_Email,
                   AuthScopes.Helix_User_Read_Follows,
                   AuthScopes.Helix_User_Read_Subscriptions,
                   AuthScopes.Helix_moderator_Manage_Chat_Messages
             };

            foreach (var scope in twitchScopes)
            {
                scopes.Add(scope.ToString());
            }
        }
    }
}
