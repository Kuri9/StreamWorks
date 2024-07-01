using TwitchLib.EventSub.Core.SubscriptionTypes.Channel;

namespace StreamWorks.Library.DataAccess;
public class DbStreamWorksConnection : IDbStreamWorksConnection
{
    // Create a single connection that will stay open at all times. 
    // Can change to a Scoped Singleton after to make seperate connections for multiple users if needed.
    private readonly IConfiguration _config;
    private readonly IMongoDatabase _db;
    private string _connectionGroup = "MongoDbMainConfig";
    private string _connectionId = "MongoDBMain";

    public MongoClient Client { get; private set; }
    public string DbName { get; private set; }

    // Collections are roughly similar to a table in SQL
    // GENERAL
    public string UserAppStateDataCollectionName { get; private set; } = "App User Data";
    public string StreamEventLogDataCollectionName { get; private set; } = "Stream Event Log Data";

    public IMongoCollection<UserAppStateModel> UserAppStateDataCollection { get; private set; }
    public IMongoCollection<StreamEventLogModel> StreamEventLogDataCollection { get; private set; }

    // STREAMWORKS COLLECTIONS
    public string StreamTimerCollectionName { get; private set; } = "Stream Timer";

    public IMongoCollection<StreamTimerModel> StreamTimerCollection { get; private set; }


    // TWITCH EVENTS
    public string TwitchFollowDataCollectionName { get; private set; } = "Twitch Follows";
    public string TwitchSubscribeDataCollectionName { get; private set; } = "Twitch Subscriptions";
    public string TwitchEndSubscribeDataCollectionName { get; private set; } = "Twitch Subscription Ends";
    public string TwitchSubscriptionGiftDataCollectionName { get; private set; } = "Twitch Subscription Gifts";
    public string TwitchCheerDataCollectionName { get; private set; } = "Twitch Cheers";
    public string TwitchRaidCollectionName { get; private set; } = "Twitch Raids";
    public string TwitchMessageDataCollectionName { get; private set; } = "Twitch Messages";

    public IMongoCollection<ChannelFollow> TwitchFollowDataCollection { get; private set; }
    public IMongoCollection<ChannelSubscribe> TwitchSubscribeDataCollection { get; private set; }
    public IMongoCollection<ChannelSubscriptionEnd> TwitchEndSubscribeDataCollection { get; private set; }
    public IMongoCollection<ChannelSubscriptionGift> TwitchSubscriptionGiftDataCollection { get; private set; }
    public IMongoCollection<ChannelCheer> TwitchCheerDataCollection { get; private set; }
    public IMongoCollection<ChannelRaid> TwitchRaidCollection { get; private set; }
    public IMongoCollection<ChannelChatMessage> TwitchMessageDataCollection { get; private set; }


    public DbStreamWorksConnection(IConfiguration config)
    {
        _config = config;

        var mongoDbSettings = _config.GetSection(_connectionGroup);
        Client = new MongoClient(_config.GetConnectionString(_connectionId))
            ?? throw new Exception($"Missing Connection String at {_connectionId}");
        DbName = mongoDbSettings["DatabaseName"];
        _db = Client.GetDatabase(DbName);

        // GENERAL
        UserAppStateDataCollection = _db.GetCollection<UserAppStateModel>(UserAppStateDataCollectionName);

        // STREAMWORKS DATA
        StreamEventLogDataCollection = _db.GetCollection<StreamEventLogModel>(StreamEventLogDataCollectionName);
        StreamTimerCollection = _db.GetCollection<StreamTimerModel>(StreamTimerCollectionName);

        // TWITCH EVENTS
        TwitchFollowDataCollection = _db.GetCollection<ChannelFollow>(TwitchFollowDataCollectionName);
        TwitchSubscribeDataCollection = _db.GetCollection<ChannelSubscribe>(TwitchSubscribeDataCollectionName);
        TwitchEndSubscribeDataCollection = _db.GetCollection<ChannelSubscriptionEnd>(TwitchEndSubscribeDataCollectionName);
        TwitchSubscriptionGiftDataCollection = _db.GetCollection<ChannelSubscriptionGift>(TwitchSubscriptionGiftDataCollectionName);
        TwitchCheerDataCollection = _db.GetCollection<ChannelCheer>(TwitchCheerDataCollectionName);
        TwitchRaidCollection = _db.GetCollection<ChannelRaid>(TwitchRaidCollectionName);
        TwitchMessageDataCollection = _db.GetCollection<ChannelChatMessage>(TwitchMessageDataCollectionName);
    }
}
