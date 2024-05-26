namespace StreamWorks.Hubs.Interfaces;

public interface ITwitchCommands
{
    Task SendMessage(string user, string message);
}
