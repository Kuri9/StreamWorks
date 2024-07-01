using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using StreamWorks.Library.DataAccess.MongoDB.Identity;
using TwitchLib.EventSub.Core.SubscriptionTypes.Channel;

namespace StreamWorks.Library.DataAccess.MongoDB.StreamWorks.StreamEventsData;
public class MongoTwitchSubscribeData : ITwitchSubscribeData
{
    private readonly ILogger<MongoTwitchSubscribeData> Logger;
    private readonly IDbStreamWorksConnection _db;
    private readonly IStreamWorksUserData _userData;
    private readonly IMemoryCache _cache;
    private readonly IMongoCollection<ChannelSubscribe> _twitchSubscribeData;
    private const string CacheName = "TwitchSubscribeData";

    public MongoTwitchSubscribeData(ILogger<MongoTwitchSubscribeData> logger, IDbStreamWorksConnection db, IStreamWorksUserData userData, IMemoryCache cache)
    {
        Logger = logger;
        _db = db;
        _userData = userData;
        _cache = cache;
        _twitchSubscribeData = db.TwitchSubscribeDataCollection;
    }

    public async Task<List<ChannelSubscribe>> GetAllTwitchSubData()
    {
        var output = _cache?.Get<List<ChannelSubscribe>>(CacheName);
        if (output == null)
        {
            var results = await _twitchSubscribeData.FindAsync(_ => true);
            output = results.ToList();

            _cache.Set(CacheName, output, TimeSpan.FromMinutes(1));
        }
        return output;
    }

    public async Task<List<ChannelSubscribe>> GetTwitchSubDataByBroadcasterId(string userId)
    {
        var output = _cache?.Get<List<ChannelSubscribe>>(CacheName);
        if (output is not null)
        {
            output = output?.Where(o => o.BroadcasterUserId == userId).ToList();
        }

        if (output is null)
        {
            var results = await _twitchSubscribeData.FindAsync(o => o.BroadcasterUserId == userId);
            output = results.ToList();
        }

        return output;
    }

    public async Task CreateTwitchSubData(ChannelSubscribe streamEvent)
    {
        var client = _db.Client;
        using var session = await client.StartSessionAsync();
        session.StartTransaction();

        try
        {
            var db = client.GetDatabase(_db.DbName);
            var contentInTransaction = db.GetCollection<ChannelSubscribe>(_db.TwitchSubscribeDataCollectionName);
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
