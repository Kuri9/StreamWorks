using StreamWorks.Library.Models.Widgets.Timers;

namespace StreamWorks.Library.DataAccess;

public interface IDbStreamWorksConnection
{
    MongoClient Client { get; }
    string DbName { get; }

    IMongoCollection<UserAppStateDataModel> UserAppStateDataCollection { get; }
    IMongoCollection<StreamTimerModel> StreamTimerCollection { get; }

    string UserAppStateDataCollectionName { get; }
    string StreamTimerCollectionName { get; }

}