using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Lds.MongoRepository.Structure
{
  public static class SurrogateKeyHelper
  {
    public static int GetNext<TDocument>(IMongoCollection<TDocument> collection) {
      var query = Builders<BsonDocument>.Filter.Eq("_id", collection.CollectionNamespace.CollectionName);
      var idSequenceCollection = collection.Database
        .GetCollection<BsonDocument>("IdInt32");
      return ConvertToInt(idSequenceCollection.FindOneAndUpdate(
        query,
        CreateUpdateDefinition(),
        new FindOneAndUpdateOptions<BsonDocument>()
        {
          IsUpsert = true,
          ReturnDocument = ReturnDocument.After
        })["seq"]);
    }

    private static UpdateDefinition<BsonDocument> CreateUpdateDefinition()
    {
      return Builders<BsonDocument>.Update.Inc(x => x["seq"], 1);
    }

    private static int ConvertToInt(BsonValue value)
    {
      return value.AsInt32;
    }
  }
}
