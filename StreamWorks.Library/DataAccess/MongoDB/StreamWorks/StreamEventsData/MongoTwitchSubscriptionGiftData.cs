using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using StreamWorks.Library.DataAccess.MongoDB.Identity;
using TwitchLib.EventSub.Core.SubscriptionTypes.Channel;

namespace StreamWorks.Library.DataAccess.MongoDB.StreamWorks.StreamEventsData;
public class MongoTwitchSubscriptionGiftData : ITwitchSubscriptionGiftData
{
    private readonly ILogger<MongoTwitchSubscriptionGiftData> Logger;
    private readonly IDbStreamWorksConnection _db;
    private readonly IStreamWorksUserData _userData;
    private readonly IMemoryCache _cache;
    private readonly IMongoCollection<ChannelSubscriptionGift> _twitchSubscriptionGiftData;
    private const string CacheName = "TwitchSubscriptionGiftData";

    public MongoTwitchSubscriptionGiftData(ILogger<MongoTwitchSubscriptionGiftData> logger, IDbStreamWorksConnection db, IStreamWorksUserData userData, IMemoryCache cache)
    {
        Logger = logger;
        _db = db;
        _userData = userData;
        _cache = cache;
        _twitchSubscriptionGiftData = db.TwitchSubscriptionGiftDataCollection;
    }

    public async Task<List<ChannelSubscriptionGift>> GetAllTwitchSubGiftData()
    {
        var output = _cache?.Get<List<ChannelSubscriptionGift>>(CacheName);
        if (output == null)
        {
            var results = await _twitchSubscriptionGiftData.FindAsync(_ => true);
            output = results.ToList();

            _cache.Set(CacheName, output, TimeSpan.FromMinutes(1));
        }
        return output;
    }

    public async Task<List<ChannelSubscriptionGift>> GetTwitchSubGiftDataByBroadcasterId(string userId)
    {
        var output = _cache?.Get<List<ChannelSubscriptionGift>>(CacheName);
        if (output is not null)
        {
            output = output?.Where(o => o.BroadcasterUserId == userId).ToList();
        }

        if (output is null)
        {
            var results = await _twitchSubscriptionGiftData.FindAsync(o => o.BroadcasterUserId == userId);
            output = results.ToList();
        }

        return output;
    }

    public async Task CreateTwitchSubGiftData(ChannelSubscriptionGift streamEvent)
    {
        var client = _db.Client;
        using var session = await client.StartSessionAsync();
        session.StartTransaction();

        try
        {
            var db = client.GetDatabase(_db.DbName);
            var contentInTransaction = db.GetCollection<ChannelSubscriptionGift>(_db.TwitchSubscriptionGiftDataCollectionName);
            await contentInTransaction.InsertOneAsync(session, streamEvent);

            await session.CommitTransactionAsync();
        }
        catch (Exception? ex)
        {
            //TODO: Logging
            Logger.LogInformation($"Error creating Event Log data: {ex.Message}");
            await session.AbortTransactionAsync();
            throw;
        }

        _cache.Remove(CacheName);
    }
}

