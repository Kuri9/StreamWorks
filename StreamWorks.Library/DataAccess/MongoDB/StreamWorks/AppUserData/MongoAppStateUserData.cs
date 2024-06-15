using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using StreamWorks.Library.DataAccess.MongoDB.Identity;
using StreamWorks.Library.DataAccess.MongoDB.StreamWorks.Widgets.Timers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamWorks.Library.DataAccess.MongoDB.StreamWorks.AppUserData;
public class MongoAppStateUserData : IAppStateUserData
{
    private readonly ILogger<MongoAppStateUserData> Logger;
    private readonly IDbStreamWorksConnection _db;
    private readonly IStreamWorksUserData _userData;
    private readonly IMemoryCache _cache;
    private readonly IMongoCollection<UserAppStateDataModel> _userAppData;
    private const string CacheName = "StreamTimerData";

    public MongoAppStateUserData(ILogger<MongoAppStateUserData> logger, IDbStreamWorksConnection db, IStreamWorksUserData userData, IMemoryCache cache)
    {
        Logger = logger;
        _db = db;
        _userData = userData;
        _cache = cache;
        _userAppData = db.UserAppStateDataCollection;
    }

    public async Task<List<UserAppStateDataModel>> GetAllStateData()
    {
        var output = _cache?.Get<List<UserAppStateDataModel>>(CacheName);
        if (output == null)
        {
            var results = await _userAppData.FindAsync(_ => true);
            output = results.ToList();

            _cache.Set(CacheName, output, TimeSpan.FromMinutes(1));
        }
        return output;
    }

    public async Task<List<UserAppStateDataModel>> GetStateDataByUserId(Guid userId)
    {
        var output = _cache?.Get<List<UserAppStateDataModel>>(CacheName);
        if (output is not null)
        {
            output = output?.Where(o => o.UserId == userId).ToList();
        }

        if (output is null)
        {
            var results = await _userAppData.FindAsync(o => o.UserId == userId);
            output = results.ToList();
        }

        return output;
    }

    public async Task CreateStateData(UserAppStateDataModel userState)
    {
        var client = _db.Client;
        using var session = await client.StartSessionAsync();
        session.StartTransaction();

        try
        {
            var db = client.GetDatabase(_db.DbName);
            var contentInTransaction = db.GetCollection<UserAppStateDataModel>(_db.StreamTimerCollectionName);
            await contentInTransaction.InsertOneAsync(session, userState);

            await session.CommitTransactionAsync();
        }
        catch (Exception? ex)
        {
            //TODO: Logging
            Logger.LogInformation($"Error creating timer data: {ex.Message}");
            await session.AbortTransactionAsync();
            throw;
        }

        _cache.Remove(CacheName);
    }

    public async Task UpdateStateData(UserAppStateDataModel userState)
    {
        await _userAppData.ReplaceOneAsync(t => t.UserId == userState.UserId, userState);

        _cache.Remove(CacheName);
    }
}
