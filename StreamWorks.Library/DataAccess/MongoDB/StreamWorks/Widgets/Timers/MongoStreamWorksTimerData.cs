using Amazon.Runtime.Internal.Util;
using Microsoft.Extensions.Caching.Memory;
using StreamWorks.Library.DataAccess.MongoDB.Identity;
using StreamWorks.Library.Models.Users.Identity;
using StreamWorks.Library.Models.Users.Twitch.Widgets.Timers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamWorks.Library.DataAccess.MongoDB.StreamWorks.Widgets.Timers;
public class MongoStreamWorksTimerData : IStreamWorksTimerData
{
    private readonly IDbStreamWorksConnection _db;
    private readonly IStreamWorksUserData _userData;
    private readonly IMemoryCache _cache;
    private readonly IMongoCollection<StreamTimerModel> _streamTimer;
    private const string CacheName = "StreamTimerData";

    public MongoStreamWorksTimerData(IDbStreamWorksConnection db, IStreamWorksUserData userData, IMemoryCache cache)
    {
        _db = db;
        _userData = userData;
        _cache = cache;
        _streamTimer = db.StreamTimerCollection;
    }

    public async Task<List<StreamTimerModel>> GetAllTimerData()
    {
        var output = _cache?.Get<List<StreamTimerModel>>(CacheName);
        if (output == null)
        {
            var results = await _streamTimer.FindAsync(_ => true);
            output = results.ToList();

            _cache.Set(CacheName, output, TimeSpan.FromMinutes(1));
        }
        return output;
    }

    public async Task<StreamTimerModel> GetTimerDataById(string timerId)
    {
        var output = _cache?.Get<List<StreamTimerModel>>(CacheName);
        if (output is not null)
        {
            output = output?.Where(o => o.Id == timerId).ToList();
        }

        if (output is null)
        {
            var results = await _streamTimer.FindAsync(o => o.Id == timerId);
            output = results.ToList();
        }

        return output.First();
    }

    public async Task<List<StreamTimerModel>> GetTimerDataByUserId(Guid userId)
    {
        var output = _cache?.Get<List<StreamTimerModel>>(CacheName);
        if (output is not null)
        {
            output = output?.Where(o => o.UserId == userId).ToList();
        }

        if (output is null)
        {
            var results = await _streamTimer.FindAsync(o => o.UserId == userId);
            output = results.ToList();
        }

        return output;
    }

    public async Task CreateTimerData(StreamTimerModel timer)
    {
        var client = _db.Client;
        using var session = await client.StartSessionAsync();
        session.StartTransaction();

        try
        {
            var db = client.GetDatabase(_db.DbName);
            var contentInTransaction = db.GetCollection<StreamTimerModel>(_db.StreamTimerCollectionName);
            await contentInTransaction.InsertOneAsync(session, timer);

            await session.CommitTransactionAsync();
        }
        catch (Exception ex)
        {
            //TODO: Logging
            await session.AbortTransactionAsync();
            throw;
        }

        _cache.Remove(CacheName);
    }

    public async Task UpdateTimerData(StreamTimerModel timer)
    {
        await _streamTimer.ReplaceOneAsync(t => t.Id == timer.Id, timer);

        _cache.Remove(CacheName);
    }
}
