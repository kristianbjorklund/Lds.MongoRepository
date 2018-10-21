
using MongoDB.Driver;
namespace Lds.MongoRepository
{
  public sealed class MainDb
  {
    private static volatile IMongoDatabase _current;
    private static readonly object SyncRoot = new object();


    public static IMongoDatabase Current {
      get {
        if (Creator==null) throw new MongoConfigurationException("Please set the MainDb.Creator to an instance of IMongoCreator");
        if (_current == null) {
          lock (SyncRoot) {
            if (_current == null)
              _current = Creator.CreateDatabase();
          }
        }
        return _current;
      }
    }

    public static IMongoCreator Creator { get; set; }

  }

}
