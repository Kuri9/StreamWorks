using System;

namespace StreamWorks.Library.Models.Widgets.Timers.TimerModels;
public class TimerOtherEventsModel
{
    // ==> Donations
    public string DonationEvent { get; set; } = "Donation";
    public int DonationEventCount { get; set; }
    public double DonationTotalAmount { get; set; }
    public TimeSpan SetDonationTime { get; set; } = TimeSpan.FromSeconds(1);
    public TimeSpan TotalDonationTime { get; set; } = TimeSpan.FromSeconds(1);
}
