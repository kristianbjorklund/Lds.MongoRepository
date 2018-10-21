
using MongoDB.Driver;
namespace Lds.MongoRepository
{
  public interface IMongoCreator
  {
    string ConnectionString { get;  }
    IMongoDatabase CreateDatabase();

    void DestroyDatabase();
  }

}
