using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using StreamWorks.Library.DataAccess.MongoDB.Identity;

namespace StreamWorks.Library.DataAccess.MongoDB.StreamWorks.StreamEventsData;
public class MongoStreamEventLogData : IStreamEventLogData
{
    private readonly ILogger<MongoStreamEventLogData> Logger;
    private readonly IDbStreamWorksConnection _db;
    private readonly IStreamWorksUserData _userData;
    private readonly IMemoryCache _cache;
    private readonly IMongoCollection<StreamEventLogModel> _streamEventLogData;
    private const string CacheName = "StreamEventLogData";

    public MongoStreamEventLogData(ILogger<MongoStreamEventLogData> logger, IDbStreamWorksConnection db, IStreamWorksUserData userData, IMemoryCache cache)
    {
        Logger = logger;
        _db = db;
        _userData = userData;
        _cache = cache;
        _streamEventLogData = db.StreamEventLogDataCollection;
    }

    public async Task<StreamEventLogModel> GetOrCreateLogData(Guid userId)
    {
        StreamEventLogModel newLog = new StreamEventLogModel();
        var result = await GetEventLogDataByUserId(userId);

        if (result is null || result.Count() <= 0)
        {
            newLog.UserId = userId;
            await CreateEventLogData(newLog);
            return newLog;
        }
        else
        {
            newLog = result.First();
            return newLog;
        }
    }

    public async Task<List<StreamEventLogModel>> GetAllEventLogData()
    {
        var output = _cache?.Get<List<StreamEventLogModel>>(CacheName);
        if (output == null)
        {
            var results = await _streamEventLogData.FindAsync(_ => true);
            output = results.ToList();

            _cache.Set(CacheName, output, TimeSpan.FromMinutes(1));
        }
        return output;
    }

    public async Task<List<StreamEventLogModel>> GetEventLogDataByUserId(Guid userId)
    {
        var output = _cache?.Get<List<StreamEventLogModel>>(CacheName);
        if (output is not null)
        {
            output = output?.Where(o => o.UserId == userId).ToList();
        }

        if (output is null)
        {
            var results = await _streamEventLogData.FindAsync(o => o.UserId == userId);
            output = results.ToList();
        }

        return output;
    }

    public async Task CreateEventLogData(StreamEventLogModel streamEventLog)
    {
        var client = _db.Client;
        using var session = await client.StartSessionAsync();
        session.StartTransaction();

        try
        {
            var db = client.GetDatabase(_db.DbName);
            var contentInTransaction = db.GetCollection<StreamEventLogModel>(_db.StreamEventLogDataCollectionName);
            await contentInTransaction.InsertOneAsync(session, streamEventLog);

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

    public async Task UpdateEventLogData(StreamEventLogModel streamEventLog)
    {
        await _streamEventLogData.ReplaceOneAsync(t => t.UserId == streamEventLog.UserId, streamEventLog);

        _cache.Remove(CacheName);
    }
}
