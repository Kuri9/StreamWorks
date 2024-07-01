using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using StreamWorks.Library.DataAccess.MongoDB.Identity;
using TwitchLib.EventSub.Core.SubscriptionTypes.Channel;

namespace StreamWorks.Library.DataAccess.MongoDB.StreamWorks.StreamEventsData;
public class MongoTwitchRaidData : ITwitchRaidData
{
    private readonly ILogger<MongoTwitchRaidData> Logger;
    private readonly IDbStreamWorksConnection _db;
    private readonly IStreamWorksUserData _userData;
    private readonly IMemoryCache _cache;
    private readonly IMongoCollection<ChannelRaid> _twitchRaidData;
    private const string CacheName = "TwitchRaidData";

    public MongoTwitchRaidData(ILogger<MongoTwitchRaidData> logger, IDbStreamWorksConnection db, IStreamWorksUserData userData, IMemoryCache cache)
    {
        Logger = logger;
        _db = db;
        _userData = userData;
        _cache = cache;
        _twitchRaidData = db.TwitchRaidCollection;
    }

    public async Task<List<ChannelRaid>> GetAllTwitchRaidData()
    {
        var output = _cache?.Get<List<ChannelRaid>>(CacheName);
        if (output == null)
        {
            var results = await _twitchRaidData.FindAsync(_ => true);
            output = results.ToList();

            _cache.Set(CacheName, output, TimeSpan.FromMinutes(1));
        }
        return output;
    }

    public async Task<List<ChannelRaid>> GetTwitchRaidDataByRaidingBroadcaster(string userId)
    {
        var output = _cache?.Get<List<ChannelRaid>>(CacheName);
        if (output is not null)
        {
            output = output?.Where(o => o.FromBroadcasterUserId == userId).ToList();
        }

        if (output is null)
        {
            var results = await _twitchRaidData.FindAsync(o => o.FromBroadcasterUserId == userId);
            output = results.ToList();
        }

        return output;
    }

    public async Task<List<ChannelRaid>> GetTwitchRaidDataByBroadcasterId(string userId)
    {
        var output = _cache?.Get<List<ChannelRaid>>(CacheName);
        if (output is not null)
        {
            output = output?.Where(o => o.ToBroadcasterUserId == userId).ToList();
        }

        if (output is null)
        {
            var results = await _twitchRaidData.FindAsync(o => o.ToBroadcasterUserId == userId);
            output = results.ToList();
        }

        return output;
    }

    public async Task CreateTwitchRaidData(ChannelRaid streamEvent)
    {
        var client = _db.Client;
        using var session = await client.StartSessionAsync();
        session.StartTransaction();

        try
        {
            var db = client.GetDatabase(_db.DbName);
            var contentInTransaction = db.GetCollection<ChannelRaid>(_db.TwitchRaidCollectionName);
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
