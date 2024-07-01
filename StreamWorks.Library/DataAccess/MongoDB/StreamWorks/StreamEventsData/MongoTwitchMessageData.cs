using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using StreamWorks.Library.DataAccess.MongoDB.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Api.Helix.Models.Moderation.CheckAutoModStatus;
using TwitchLib.EventSub.Core.Models.Chat;
using TwitchLib.EventSub.Core.SubscriptionTypes.Channel;

namespace StreamWorks.Library.DataAccess.MongoDB.StreamWorks.StreamEventsData;
public class MongoTwitchMessageData : ITwitchMessageData
{
    private readonly ILogger<MongoTwitchMessageData> Logger;
    private readonly IDbStreamWorksConnection _db;
    private readonly IStreamWorksUserData _userData;
    private readonly IMemoryCache _cache;
    private readonly IMongoCollection<ChannelChatMessage> _twitchMessageData;
    private const string CacheName = "TwitchMessageData";

    public MongoTwitchMessageData(ILogger<MongoTwitchMessageData> logger, IDbStreamWorksConnection db, IStreamWorksUserData userData, IMemoryCache cache)
    {
        Logger = logger;
        _db = db;
        _userData = userData;
        _cache = cache;
        _twitchMessageData = db.TwitchMessageDataCollection;
    }

    public async Task<List<ChannelChatMessage>> GetAllTwitchMessageData()
    {
        var output = _cache?.Get<List<ChannelChatMessage>>(CacheName);
        if (output == null)
        {
            var results = await _twitchMessageData.FindAsync(_ => true);
            output = results.ToList();

            _cache.Set(CacheName, output, TimeSpan.FromMinutes(1));
        }
        return output;
    }

    public async Task<List<ChannelChatMessage>> GetTwitchMessageDataByBroadcaster(string userId)
    {
        var output = _cache?.Get<List<ChannelChatMessage>>(CacheName);
        if (output is not null)
        {
            output = output?.Where(o => o.BroadcasterUserId == userId).ToList();
        }

        if (output is null)
        {
            var results = await _twitchMessageData.FindAsync(o => o.BroadcasterUserId == userId);
            output = results.ToList();
        }

        return output;
    }

    public async Task<List<ChannelChatMessage>> GetTwitchMessageDataByChatterId(string userId)
    {
        var output = _cache?.Get<List<ChannelChatMessage>>(CacheName);
        if (output is not null)
        {
            output = output?.Where(o => o.ChatterUserId == userId).ToList();
        }

        if (output is null)
        {
            var results = await _twitchMessageData.FindAsync(o => o.ChatterUserId == userId);
            output = results.ToList();
        }

        return output;
    }

    public async Task<bool> CreateTwitchMessageData(ChannelChatMessage streamEvent)
    {
        var IsComplete = false;

        var client = _db.Client;
        using var session = await client.StartSessionAsync();
        session.StartTransaction();

        try
        {
            var db = client.GetDatabase(_db.DbName);
            var contentInTransaction = db.GetCollection<ChannelChatMessage>(_db.TwitchMessageDataCollectionName);
            await contentInTransaction.InsertOneAsync(session, streamEvent);

            await session.CommitTransactionAsync();
        }
        catch (Exception? ex)
        {
            //TODO: Logging
            Logger.LogInformation($"Error creating message data: {ex.Message}");
            await session.AbortTransactionAsync();
            return IsComplete;
        }

        _cache.Remove(CacheName);
        return IsComplete = true;
    }

    public async Task<bool> RemoveTwitchMessage(string userId)
    {
        var IsComplete = false;

        var client = _db.Client;
        using var session = await client.StartSessionAsync();
        session.StartTransaction();

        try
        {
            var results = await _twitchMessageData.DeleteOneAsync(o => o.MessageId == userId);
        }
        catch (Exception? ex)
        {
            //TODO: Logging
            Logger.LogInformation($"Error removing message data: {ex.Message}");
            await session.AbortTransactionAsync();
            return IsComplete;
        }

        _cache.Remove(CacheName);
        return IsComplete = true;
    }
}
