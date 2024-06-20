namespace StreamWorks.Library.DataAccess;

public interface IDbStreamWorksConnection
{
    MongoClient Client { get; }
    string DbName { get; }

    IMongoCollection<UserAppStateModel> UserAppStateDataCollection { get; }
    IMongoCollection<StreamTimerModel> StreamTimerCollection { get; }
    IMongoCollection<StreamEventLogModel> StreamEventLogDataCollection { get; }

    string UserAppStateDataCollectionName { get; }
    string StreamEventLogDataCollectionName { get; }
    string StreamTimerCollectionName { get; }

}