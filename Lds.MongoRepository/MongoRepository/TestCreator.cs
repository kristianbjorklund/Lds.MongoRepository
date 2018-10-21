using System;
using MongoDB.Driver;

namespace Lds.MongoRepository
{
  public class TestCreator : IMongoCreator
  {
    private MongoUrl url;

    public TestCreator()
    {
      ConnectionString = $"mongodb://localhost:27017/Test-{Guid.NewGuid()}";
    }

    public string ConnectionString { get; }

    public IMongoDatabase CreateDatabase()
    {
      url = new MongoUrl(ConnectionString);
      var client = new MongoClient(url);
      var database = client.GetDatabase(url.DatabaseName);
      return database;
    }

    public void DestroyDatabase()
    {
      var client = new MongoClient(url);
      client.DropDatabase(url.DatabaseName);
    }
  }
}