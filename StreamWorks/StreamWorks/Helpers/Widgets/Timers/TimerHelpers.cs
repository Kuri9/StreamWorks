using StreamWorks.Library.DataAccess.MongoDB.StreamWorks.Widgets.Timers;
using StreamWorks.Library.Models.Widgets.Timers;
using StreamWorks.Models.Widgets.Timers;
using TwitchLib.Api.Helix.Models.Charity;

namespace StreamWorks.Helpers.Widgets.Timers;

public static class TimerHelpers
{
    public static TimeSpan ConvertToTimeSpan(double time)
    {
        return TimeSpan.FromSeconds(time);
    }

    public static double ConvertToSeconds(TimeSpan time)
    {
        return time.TotalSeconds;
    }

    public static StreamTimerModel AddTime(TimeSpan time, StreamTimerModel timerData)
    {
        if (time < TimeSpan.Zero)
        {
            time.Multiply(-1);
        }
        if (timerData is not null)
        {
            timerData.AddTime = time;
            timerData.CurrentTime += timerData.AddTime;
            timerData.TimerSettings.TotalTime += timerData.AddTime;

            return timerData;
        }
        else
        {
            //Logger.LogError("Timer Data is null. Couldn't add time from Chat");

            return null;
        }
    }

    public static StreamTimerModel RemoveTime(TimeSpan time, StreamTimerModel timerData)
    {
        if (time < TimeSpan.Zero)
        {
            time.Multiply(-1);
        }
        if (timerData is not null)
        {
            timerData.AddTime = time;
            timerData.CurrentTime -= timerData.AddTime;
            timerData.TimerSettings.TotalTime -= timerData.AddTime;

            return timerData;
        }
        else
        {
            // Logger.LogError("Timer Data is null. Couldn't add time from Chat");
            return null;
        }
    }

    public static StreamTimerModel AddTimeWithAmount(string eventName, int amount, StreamTimerModel timerData)
    {
        if (amount < 0)
        {
            amount *= -1;
        }

        if (timerData is not null)
        {
            if (eventName == timerData.OtherEvents.DonationEvent)
            {
                timerData.OtherEvents.DonationEventCount++;
                timerData.AddTime = timerData.OtherEvents.SetDonationTime * amount;

                return timerData;
            }
            else
            {
                // Logger.LogError("Could not find Twitch Event");
                return null;
            }
        }
        else
        {
            // Logger.LogError("Timer Data is null. Couldn't add time with amount.");
            return null;
        }
    }

    public static StreamTimerModel RemoveTimeWithAmount(string eventName, int amount, StreamTimerModel timerData)
    {
        if (amount < 0)
        {
            amount *= -1;
        }

        if (timerData is not null)
        {
            if (eventName == timerData.OtherEvents.DonationEvent)
            {
                timerData.OtherEvents.DonationEventCount--;
                timerData.AddTime = timerData.OtherEvents.SetDonationTime * -(amount);

                return timerData;
            }
            else
            {
                // Logger.LogError("Could not find Twitch Event");
                return null;
            }
        }
        else
        {
            // Logger.LogError("Timer Data is null. Couldn't add time with amount.");
            return null;
        }
    }

    public static StreamTimerModel AddTimeTwitch(string twitchEvent, StreamTimerModel timerData)
    {
        if (timerData is not null)
        {
            timerData.AddTime = TimeSpan.Zero;

            if (twitchEvent == timerData.TwitchEvents.TwitchFollowEvent)
            {
                timerData.TwitchEvents.TwitchFollowEventCount++;
                timerData.AddTime = timerData.TwitchEvents.SetTwitchFollowTime;
            }
            else if (twitchEvent == timerData.TwitchEvents.TwitchTier1Event)
            {
                timerData.TwitchEvents.TwitchTier1EventCount++;
                timerData.AddTime = timerData.TwitchEvents.SetTwitchTier1Time;
            }
            else if (twitchEvent == timerData.TwitchEvents.TwitchTier2Event)
            {
                timerData.TwitchEvents.TwitchTier2EventCount++;
                timerData.AddTime = timerData.TwitchEvents.SetTwitchTier2Time;
            }
            else if (twitchEvent == timerData.TwitchEvents.TwitchTier3Event)
            {
                timerData.TwitchEvents.TwitchTier3EventCount++;
                timerData.AddTime = timerData.TwitchEvents.SetTwitchTier3Time;
            }
            else
            {
                // Logger.LogError("Could not find Twitch Event");
            }

            return timerData;
        }
        else
        {
            // Logger.LogError("Timer Data is null. Couldn't add time.");
            return null;
        }
    }

    public static StreamTimerModel AddTimeWithAmountTwitch(string twitchEvent, int amount, StreamTimerModel timerData)
    {
        if (amount < 0)
        {
            amount *= -1;
        }

        if (timerData is not null)
        {
            if (twitchEvent == timerData.TwitchEvents.TwitchTier1SubGiftEvent)
            {
                timerData.TwitchEvents.TwitchSubGiftEventCount++;
                timerData.TwitchEvents.TwitchTotalSubGiftCount += amount;
                timerData.TwitchEvents.TwitchTotalT1SubGiftCount += amount;
                timerData.AddTime = timerData.TwitchEvents.SetTwitchTier1SubGiftTime * amount;
            }
            else if (twitchEvent == timerData.TwitchEvents.TwitchTier2SubGiftEvent)
            {
                timerData.TwitchEvents.TwitchSubGiftEventCount++;
                timerData.TwitchEvents.TwitchTotalSubGiftCount += amount;
                timerData.TwitchEvents.TwitchTotalT2SubGiftCount += amount;
                timerData.AddTime = timerData.TwitchEvents.SetTwitchTier2SubGiftTime * amount;
            }
            else if (twitchEvent == timerData.TwitchEvents.TwitchTier3SubGiftEvent)
            {
                timerData.TwitchEvents.TwitchSubGiftEventCount++;
                timerData.TwitchEvents.TwitchTotalSubGiftCount += amount;
                timerData.TwitchEvents.TwitchTotalT3SubGiftCount += amount;
                timerData.AddTime = timerData.TwitchEvents.SetTwitchTier3SubGiftTime * amount;
            }
            else if (twitchEvent == timerData.TwitchEvents.TwitchCheerEvent)
            {
                timerData.TwitchEvents.TwitchCheerEventCount++;
                timerData.TwitchEvents.TwitchTotalCheerAmount += amount;
                timerData.AddTime = timerData.TwitchEvents.SetTwitchCheerTime * amount;
            }
            else if (twitchEvent == timerData.TwitchEvents.TwitchRaidEvent)
            {
                timerData.TwitchEvents.TwitchRaidEventCount++;
                timerData.TwitchEvents.TwitchTotalRaidAmount += amount;
                timerData.AddTime = timerData.TwitchEvents.SetTwitchRaidTime * amount;
            }
            else
            {
                //Logger.LogError("Could not find Twitch Event");
            }

            return timerData;
        }
        else
        {
            //Logger.LogError("Timer Data is null. Couldn't add time with amount.");
            return null;
        }
    }

    public static StreamTimerModel RemoveTimeTwitch(string twitchEvent, StreamTimerModel timerData)
    {
        if (timerData is not null)
        {
            if (twitchEvent == timerData.TwitchEvents.TwitchFollowEvent)
            {
                timerData.TwitchEvents.TwitchFollowEventCount--;
                timerData.AddTime = timerData.TwitchEvents.SetTwitchFollowTime * -1;
            }
            else if (twitchEvent == timerData.TwitchEvents.TwitchTier1Event)
            {
                timerData.TwitchEvents.TwitchTier1EventCount--;
                timerData.AddTime = timerData.TwitchEvents.SetTwitchTier1Time * -1;
            }
            else if (twitchEvent == timerData.TwitchEvents.TwitchTier2Event)
            {
                timerData.TwitchEvents.TwitchTier2EventCount--;
                timerData.AddTime = timerData.TwitchEvents.SetTwitchTier2Time * -1;
            }
            else if (twitchEvent == timerData.TwitchEvents.TwitchTier3Event)
            {
                timerData.TwitchEvents.TwitchTier3EventCount--;
                timerData.AddTime = timerData.TwitchEvents.SetTwitchTier3Time * -1;
            }
            else
            {
                // Logger.LogError("Could not find Twitch Event");
            }

            return timerData;
        }
        else
        {
            // Logger.LogError("Timer Data is null. Couldn't add time with amount.");
            return null;
        }
    }

    public static StreamTimerModel RemoveTimeWithAmountTwitch(string twitchEvent, int amount, StreamTimerModel timerData)
    {
        if (amount < 0)
        {
            amount *= -1;
        }

        if (timerData is not null)
        {
            if (twitchEvent == timerData.TwitchEvents.TwitchTier1SubGiftEvent)
            {
                timerData.TwitchEvents.TwitchSubGiftEventCount--;
                timerData.TwitchEvents.TwitchTotalSubGiftCount -= amount;

                timerData.TwitchEvents.TwitchTotalT1SubGiftCount -= amount;
                timerData.AddTime = timerData.TwitchEvents.SetTwitchTier1SubGiftTime * -(amount);
            }
            else if (twitchEvent == timerData.TwitchEvents.TwitchTier2SubGiftEvent)
            {
                timerData.TwitchEvents.TwitchSubGiftEventCount--;
                timerData.TwitchEvents.TwitchTotalSubGiftCount -= amount;

                timerData.TwitchEvents.TwitchTotalT2SubGiftCount -= amount;
                timerData.AddTime = timerData.TwitchEvents.SetTwitchTier2SubGiftTime * -(amount);
            }
            else if (twitchEvent == timerData.TwitchEvents.TwitchTier3SubGiftEvent)
            {
                timerData.TwitchEvents.TwitchSubGiftEventCount--;
                timerData.TwitchEvents.TwitchTotalSubGiftCount -= amount;

                timerData.TwitchEvents.TwitchTotalT3SubGiftCount -= amount;
                timerData.AddTime = timerData.TwitchEvents.SetTwitchTier3SubGiftTime * -(amount);
            }
            else if (twitchEvent == timerData.TwitchEvents.TwitchCheerEvent)
            {
                timerData.TwitchEvents.TwitchCheerEventCount--;
                timerData.TwitchEvents.TwitchTotalCheerAmount -= amount;
                timerData.AddTime = timerData.TwitchEvents.SetTwitchCheerTime * -(amount);
            }
            else if (twitchEvent == timerData.TwitchEvents.TwitchRaidEvent)
            {
                timerData.TwitchEvents.TwitchRaidEventCount--;
                timerData.TwitchEvents.TwitchTotalRaidAmount -= amount;
                timerData.AddTime = timerData.TwitchEvents.SetTwitchRaidTime * -(amount);
            }
            else
            {
                // Logger.LogError("Could not find Twitch Event");
            }

            return timerData;
        }
        else
        {
            // Logger.LogError("Timer Data is null. Couldn't add time with amount.");
            return null;
        }
    }

    public static StreamTimerModel AddTimeYoutube(string youtubeEvent, StreamTimerModel timerData)
    {
        if (timerData is not null)
        {
            switch (youtubeEvent)
            {
                case "YoutubeLike":
                    timerData.YoutubeEvents.YoutubeLikeEventCount++;
                    timerData.AddTime = timerData.YoutubeEvents.SetYouTubeLikeTime;
                    break;

                case "YoutubeSubscribe":
                    timerData.YoutubeEvents.YoutubeSubEventCount++;
                    timerData.AddTime = timerData.YoutubeEvents.SetYouTubeSubTime;
                    break;

                default:
                    timerData.AddTime = timerData.TimerSettings.DefaultTime;
                    break;
            }

            return timerData;
        }
        else
        {
            // Logger.LogError("Timer Data is null. Couldn't add time with amount.");
            return null;
        }
    }

    public static StreamTimerModel RemoveTimeYoutube(string youtubeEvent, StreamTimerModel timerData)
    {
        if (timerData is not null)
        {
            switch (youtubeEvent)
            {
                case "YoutubeLike":
                    timerData.YoutubeEvents.YoutubeLikeEventCount--;
                    timerData.AddTime = -(timerData.YoutubeEvents.SetYouTubeLikeTime);
                    break;

                case "YoutubeSubscribe":
                    timerData.YoutubeEvents.YoutubeSubEventCount--;
                    timerData.AddTime = -(timerData.YoutubeEvents.SetYouTubeSubTime);
                    break;

                default:
                    timerData.AddTime = -(timerData.TimerSettings.DefaultTime);
                    break;
            }

            return timerData;
        }
        else
        {
            // Logger.LogError("Timer Data is null. Couldn't add time with amount.");
            return null;
        }
    }

    public static StreamTimerModel ResetEventCounts(StreamTimerModel timerData)
    {
        if (timerData is not null)
        {
            timerData.TwitchEvents.TwitchFollowEventCount = 0;

            timerData.TwitchEvents.TwitchTier1EventCount = 0;
            timerData.TwitchEvents.TwitchTier2EventCount = 0;
            timerData.TwitchEvents.TwitchTier3EventCount = 0;

            timerData.TwitchEvents.TwitchSubGiftEventCount = 0;
            timerData.TwitchEvents.TwitchTotalSubGiftCount = 0;
            timerData.TwitchEvents.TwitchTotalT1SubGiftCount = 0;
            timerData.TwitchEvents.TwitchTotalT2SubGiftCount = 0;
            timerData.TwitchEvents.TwitchTotalT3SubGiftCount = 0;

            timerData.TwitchEvents.TwitchCheerEventCount = 0;
            timerData.TwitchEvents.TwitchTotalCheerAmount = 0;

            timerData.TwitchEvents.TwitchRaidEventCount = 0;
            timerData.TwitchEvents.TwitchTotalRaidAmount = 0;

            timerData.YoutubeEvents.YoutubeLikeEventCount = 0;
            timerData.YoutubeEvents.YoutubeSubEventCount = 0;

            return timerData;
        }

        return null;
    }

    public static StreamTimerModel SetUpdateData(StreamTimerModel timerData, CreateStreamTimerModel updatedTimerModel)
    {
        if (timerData is not null && updatedTimerModel is not null)
        {
            timerData.TimerSettings.StartingTime = TimeSpan.FromSeconds(updatedTimerModel.StartingTime);
            timerData.TimerSettings.ShowTotalCounts = updatedTimerModel.ShowTotalCounts;

            // Twitch
            timerData.TwitchEvents.SetTwitchFollowTime = TimeSpan.FromSeconds(updatedTimerModel.TwitchFollowTime);
            timerData.TwitchEvents.SetTwitchTier1Time = TimeSpan.FromSeconds(updatedTimerModel.TwitchTier1Time);
            timerData.TwitchEvents.SetTwitchTier2Time = TimeSpan.FromSeconds(updatedTimerModel.TwitchTier2Time);
            timerData.TwitchEvents.SetTwitchTier3Time = TimeSpan.FromSeconds(updatedTimerModel.TwitchTier3Time);

            timerData.TwitchEvents.SetTwitchTier1SubGiftTime = TimeSpan.FromSeconds(updatedTimerModel.TwitchTier1SubGiftTime);
            timerData.TwitchEvents.SetTwitchTier2SubGiftTime = TimeSpan.FromSeconds(updatedTimerModel.TwitchTier2SubGiftTime);
            timerData.TwitchEvents.SetTwitchTier3SubGiftTime = TimeSpan.FromSeconds(updatedTimerModel.TwitchTier3SubGiftTime);

            timerData.TwitchEvents.SetTwitchCheerTime = TimeSpan.FromSeconds(updatedTimerModel.TwitchCheerTime);
            timerData.TwitchEvents.SetTwitchRaidTime = TimeSpan.FromSeconds(updatedTimerModel.TwitchRaidTime);

            // Youtube
            timerData.YoutubeEvents.SetYouTubeLikeTime = TimeSpan.FromSeconds(updatedTimerModel.YouTubeLikeTime);
            timerData.YoutubeEvents.SetYouTubeSubTime = TimeSpan.FromSeconds(updatedTimerModel.YouTubeSubTime);

            // Other
            timerData.OtherEvents.SetDonationTime = TimeSpan.FromSeconds(updatedTimerModel.DonationTime);

            return timerData;
        }

        return null;
    }
}
