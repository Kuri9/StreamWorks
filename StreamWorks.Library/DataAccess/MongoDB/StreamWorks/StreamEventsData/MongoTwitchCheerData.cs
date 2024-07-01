using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using StreamWorks.Library.DataAccess.MongoDB.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.EventSub.Core.SubscriptionTypes.Channel;

namespace StreamWorks.Library.DataAccess.MongoDB.StreamWorks.StreamEventsData;
public class MongoTwitchCheerData : ITwitchCheerData
{
    private readonly ILogger<MongoTwitchCheerData> Logger;
    private readonly IDbStreamWorksConnection _db;
    private readonly IStreamWorksUserData _userData;
    private readonly IMemoryCache _cache;
    private readonly IMongoCollection<ChannelCheer> _twitchCheerData;
    private const string CacheName = "TwitchCheerData";

    public MongoTwitchCheerData(ILogger<MongoTwitchCheerData> logger, IDbStreamWorksConnection db, IStreamWorksUserData userData, IMemoryCache cache)
    {
        Logger = logger;
        _db = db;
        _userData = userData;
        _cache = cache;
        _twitchCheerData = db.TwitchCheerDataCollection;
    }

    public async Task<List<ChannelCheer>> GetAllTwitchCheerData()
    {
        var output = _cache?.Get<List<ChannelCheer>>(CacheName);
        if (output == null)
        {
            var results = await _twitchCheerData.FindAsync(_ => true);
            output = results.ToList();

            _cache.Set(CacheName, output, TimeSpan.FromMinutes(1));
        }
        return output;
    }

    public async Task<List<ChannelCheer>> GetTwitchCheerDataByBroadcasterId(string userId)
    {
        var output = _cache?.Get<List<ChannelCheer>>(CacheName);
        if (output is not null)
        {
            output = output?.Where(o => o.BroadcasterUserId == userId).ToList();
        }

        if (output is null)
        {
            var results = await _twitchCheerData.FindAsync(o => o.BroadcasterUserId == userId);
            output = results.ToList();
        }

        return output;
    }

    public async Task CreateTwitchCheerData(ChannelCheer streamEvent)
    {
        var client = _db.Client;
        using var session = await client.StartSessionAsync();
        session.StartTransaction();

        try
        {
            var db = client.GetDatabase(_db.DbName);
            var contentInTransaction = db.GetCollection<ChannelCheer>(_db.TwitchCheerDataCollectionName);
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
