using System;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Lds.MongoRepository.Structure
{
  /// <summary>
  /// Int64 identifier generator.
  /// </summary>
  public sealed class Int64IdGenerator<TDocument> : IntIdGeneratorBase<TDocument> {
    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="Int64IdGenerator{TDocument}"/> class.
    /// </summary>
    /// <param name="idCollectionName">Identifier collection name.</param>
    public Int64IdGenerator(string idCollectionName) : base(idCollectionName) {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Int64IdGenerator{TDocument}"/> class.
    /// </summary>
    public Int64IdGenerator() : base("IdInt64") {
    }
    #endregion

    #region implemented abstract members of IntIdGeneratorBase
    /// <summary>
    /// Creates the update builder.
    /// </summary>
    /// <returns>The update builder.</returns>
    protected override UpdateDefinition<BsonDocument> CreateUpdateDefinition() {
      return Builders<BsonDocument>.Update.Inc(x => x["seq"], 1L);
    }

    /// <summary>
    /// Converts to int.
    /// </summary>
    /// <returns>The to int.</returns>
    /// <param name="value">Value.</param>
    protected override object ConvertToInt(BsonValue value) {
      return value.AsInt64;
    }

    /// <summary>
    /// Tests whether an Id is empty.
    /// </summary>
    /// <param name="id">The Id.</param>
    /// <returns>True if the Id is empty.</returns>
    public override bool IsEmpty(object id) {
      return ((Int64)id) == 0;
    }
    #endregion
  }
}