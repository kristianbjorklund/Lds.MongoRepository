
using MongoDB.Driver;
namespace Lds.MongoRepository
{
  public abstract class DefaultCreator : IMongoCreator
  {
    public string ConnectionString { get; protected set; }

    protected DefaultCreator(string connectionString)
    {
      ConnectionString = connectionString;
    }
    public IMongoDatabase CreateDatabase()
    {
      var url = new MongoUrl(ConnectionString);
      var client = new MongoClient(url);
      var database = client.GetDatabase(url.DatabaseName);
      Setup(database);
      return database;
    }

    public void DestroyDatabase()
    {
      throw new System.NotImplementedException();
    }

    protected abstract void Setup(IMongoDatabase database);
    /* 
      * database.GetCollection<InventoryChange>("InventoryChange").Indexes.CreateOne(Builders<InventoryChange>.IndexKeys.Ascending(_ => _.Sku));
      database.GetCollection<Employee>("Employee").Indexes.CreateOne(Builders<Employee>.IndexKeys.Ascending(_ => _.SiteCaseId));
      var orders = database.GetCollection<Order>("Order");
      orders.Indexes.CreateOne(Builders<Order>.IndexKeys.Ascending(_ => _.SiteCaseId));
      orders.Indexes.CreateOne(Builders<Order>.IndexKeys.Text(_ => _.Email).Text(_ => _.Name).Text(_ => _.Phone).Text(_ => _.PostalCode));
      */

  }
}
