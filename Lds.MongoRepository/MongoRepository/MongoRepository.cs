using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Lds.MongoRepository.Interfaces;
using MongoDB.Bson;

namespace Lds.MongoRepository
{
  /// <summary>
  /// Deals with entities in MongoDb.
  /// </summary>
  /// <typeparam name="T">The type contained in the repository.</typeparam>
  /// <typeparam name="TKey">The type used for the entity's Id.</typeparam>
  public class MongoRepository<T, TKey> : IRepository<T, TKey>
      where T : IEntity<TKey>
  {
    /// <summary>
    /// MongoCollection field.
    /// </summary>
    protected internal IMongoCollection<T> collection;

    #region Constructors
    /*
    /// <summary>
    /// Initializes a new instance of the MongoRepository class.
    /// Uses the Default App/Web.Config ConnectionString to fetch the connectionString and Database name.
    /// </summary>
    /// <remarks>Default constructor defaults to "MongoServerSettings" key for ConnectionString.</remarks>
    public MongoRepository()
      : this(Util<TKey>.GetDefaultConnectionString()) {
    }
    */

    /// <summary>
    /// Initializes a new instance of the MongoRepository class.
    /// </summary>
    /// <param name="connectionString">ConnectionString to use for connecting to MongoDB.</param>
    public MongoRepository(string connectionString)
    {
      this.collection = Util<TKey>.GetCollectionFromConnectionString<T>(connectionString);
    }

    /// <summary>
    /// Initializes a new instance of the MongoRepository class.
    /// </summary>
    /// <param name="connectionString">ConnectionString to use for connecting to MongoDB.</param>
    /// <param name="collectionName">The name of the collection to use.</param>
    public MongoRepository(string connectionString, string collectionName)
    {
      this.collection = Util<TKey>.GetCollectionFromConnectionString<T>(connectionString, collectionName);
    }

    /// <summary>
    /// Initializes a new instance of the MongoRepository class.
    /// </summary>
    /// <param name="url">Url to use for connecting to MongoDB.</param>
    public MongoRepository(MongoUrl url)
    {
      this.collection = Util<TKey>.GetCollectionFromUrl<T>(url);
    }

    /// <summary>
    /// Initializes a new instance of the MongoRepository class.
    /// </summary>
    /// <param name="database">Database to use for connecting to MongoDB.</param>
    public MongoRepository(IMongoDatabase database)
    {
      this.collection = Util<TKey>.GetCollectionFromDatabase<T>(database);
    }

    /// <summary>
    /// Initializes a new instance of the MongoRepository class.
    /// </summary>
    /// <param name="url">Url to use for connecting to MongoDB.</param>
    /// <param name="collectionName">The name of the collection to use.</param>
    public MongoRepository(MongoUrl url, string collectionName)
    {
      this.collection = Util<TKey>.GetCollectionFromUrl<T>(url, collectionName);
    }

    #endregion

    #region Collection
    /// <inheritdoc />
    /// <summary>
    /// Gets the Mongo collection (to perform advanced operations).
    /// </summary>
    /// <remarks>
    /// One can argue that exposing this property (and with that, access to it's Database property for instance
    /// (which is a "parent")) is not the responsibility of this class. Use of this property is highly discouraged;
    /// for most purposes you can use the MongoRepositoryManager&lt;T&gt;
    /// </remarks>
    /// <value>The Mongo collection (to perform advanced operations).</value>
    public IMongoCollection<T> Collection => collection;

    /// <summary>
    /// Gets the name of the collection
    /// </summary>
    public string CollectionName => collection.CollectionNamespace.CollectionName;
    #endregion

    #region Single Get

    /// <inheritdoc />
    /// <summary>
    /// Returns the T by its given id.
    /// </summary>
    /// <param name="id">The Id of the entity to retrieve.</param>
    /// <returns>The Entity T.</returns>
    public T GetById(TKey id)
    {
      return GetById(id, false);
    }

    /// <summary>
    /// Returns the T by its given id.
    /// </summary>
    /// <param name="id">The Id of the entity to retrieve.</param>
    /// <returns>The Entity T.</returns>
    public async Task<T> GetByIdAsync(TKey id)
    {
      return await GetByIdAsync(id, false);
    }

    /// <summary>
    /// Returns the T by its given id.
    /// </summary>
    /// <param name="id">The Id of the entity to retrieve.</param>
    /// <param name="includeDeleted">True to also look for deleted</param>
    /// <returns>The Entity T.</returns>
    public T GetById(TKey id, bool includeDeleted )
    {
      if (includeDeleted || !IsDeleteProtected)
        return collection.Find(p => p.Id.Equals(id)).FirstOrDefault();

      return collection.Find(p => p.Id.Equals(id) && ((IDeleteProtected)p).Deleted==null).FirstOrDefault();
    }

    public async Task<T> GetByIdAsync(TKey id, bool includeDeleted)
    {
      /*if (typeof(T).IsSubclassOf(typeof(Entity))) {
        return this.GetById(new ObjectId(id as string));
      }*/
    /*  var r = await collection.FindAsync(p => p.Id.Equals(id));
      return r.FirstOrDefault();
      */
      if (includeDeleted || !IsDeleteProtected)
      {
        var rAsyncCursor = await collection.FindAsync(p => p.Id.Equals(id));
        return rAsyncCursor.FirstOrDefault();
      }
      else
      {
        var rAsyncCursor = await collection.FindAsync(p => p.Id.Equals(id) && ((IDeleteProtected)p).Deleted == null);
          return rAsyncCursor.FirstOrDefault();
      }
      
    }


    /// <summary>
    /// Returns the T by its given id.
    /// </summary>
    /// <param name="id">The Id of the entity to retrieve.</param>
    /// <returns>The Entity T.</returns>
    public virtual T GetById(ObjectId id)
    {
      return collection.Find(p => p.Id.Equals(id)).FirstOrDefault();
      //return this.collection.FindOneByIdAs<T>(id);
    }

    #endregion

    #region Add 

    /// <inheritdoc />
    /// <summary>
    /// Adds the new entity in the repository.
    /// </summary>
    /// <param name="entity">The entity T.</param>
    /// <returns>The added entity including its new ObjectId.</returns>
    public virtual void Add(T entity)
    {
      var evtBase = entity as IWithEvents;
      evtBase?.OnBeforeSave();
      SetDateAudit(entity);
      collection.InsertOne(entity);
    }

    /// <summary>
    /// Adds the new entity in the repository.
    /// </summary>
    /// <param name="entity">The entity T.</param>
    public virtual async Task AddAsync(T entity)
    {
      var evtBase = entity as IWithEvents;
      evtBase?.OnBeforeSave();
      SetDateAudit(entity);
      await collection.InsertOneAsync(entity);
    }

    /// <inheritdoc />
    /// <summary>
    /// Adds the new entities in the repository.
    /// </summary>
    /// <param name="entities">The entities of type T.</param>
    public virtual void Add(IEnumerable<T> entities)
    {
      if (IsWithEvents) {
        var entityList = entities.ToList();
        foreach (T entity in entityList) {
          SetDateAudit(entity);
          var evtBase = entity as IWithEvents;
          evtBase?.OnBeforeSave();
        }
        this.collection.InsertMany(entityList);
      } else {
        this.collection.InsertMany(entities);
      }


    }
    public virtual async Task AddAsync(IEnumerable<T> entities)
    {
      if (IsWithEvents) {
        var entityList = entities.ToList();
        foreach (T entity in entityList) {
          SetDateAudit(entity);
          var evtBase = entity as IWithEvents;
          evtBase?.OnBeforeSave();
        }
        await collection.InsertManyAsync(entityList);
      } else {
        await collection.InsertManyAsync(entities);
      }
    }

    #endregion

    #region Replace
    /// <inheritdoc />
    /// <summary>
    /// Updates an entity.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <returns>The updated entity.</returns>
    public virtual bool Replace(T entity)
    {
      var evtBase = entity as IWithEvents;
      evtBase?.OnBeforeSave();
      SetDateAudit(entity);
      var filter = Builders<T>.Filter.Eq(s => s.Id, entity.Id);
      var result = collection.ReplaceOne(filter, entity);
      return (result.ModifiedCount == 1);
    }


    /// <summary>
    /// Replaces an entity.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <returns>The updated entity.</returns>
    public virtual async Task<bool> ReplaceAsync(T entity)
    {
      var evtBase = entity as IWithEvents;
      evtBase?.OnBeforeSave();
      SetDateAudit(entity);
      var filter = Builders<T>.Filter.Eq(s => s.Id, entity.Id);
      var result = await collection.ReplaceOneAsync(filter, entity);
      return (result.ModifiedCount == 1);
    }

    /// <inheritdoc />
    /// <summary>
    /// Updates the entities.
    /// </summary>
    /// <param name="entities">The entities to update.</param>
    public virtual bool Replace(IEnumerable<T> entities)
    {
      var ok = true;
      foreach (T entity in entities) {
        var evtBase = entity as IWithEvents;
        evtBase?.OnBeforeSave();
        SetDateAudit(entity);
        var filter = Builders<T>.Filter.Eq(s => s.Id, entity.Id);
        var result = collection.ReplaceOne(filter, entity);
        if (result.ModifiedCount == 1)
          ok = false;
      }

      return ok;
    }

    /// <summary>
    /// Updates the entities.
    /// </summary>
    /// <param name="entities">The entities to update.</param>
    public virtual async Task<bool> ReplaceAsync(IEnumerable<T> entities)
    {
      var ok = true;
      foreach (T entity in entities) {
        var evtBase = entity as IWithEvents;
        evtBase?.OnBeforeSave();
        SetDateAudit(entity);
        var filter = Builders<T>.Filter.Eq(s => s.Id, entity.Id);
        var result = await collection.ReplaceOneAsync(filter, entity);
        if (result.ModifiedCount == 1)
          ok = false;
      }

      return ok;
    }
    #endregion

    #region Entity Handling
    private void SetDateAudit(IHasDateAuditing dateAudited)
    {
      if (dateAudited == null) return;
      if (dateAudited.Created == default(DateTime)) {
        dateAudited.Created = DateTime.UtcNow;
      }
      dateAudited.LastModified = DateTime.UtcNow;
    }

    private void SetDateAudit(object mightBeDateAudited)
    {
      SetDateAudit(mightBeDateAudited as IHasDateAuditing);
    }

    public bool IsWithEvents => typeof(T).GetInterfaces().Contains(typeof(IWithEvents));
    public bool IsDeleteProtected => typeof(T).GetInterfaces().Contains(typeof(IDeleteProtected));
    public bool IsDateAuditing => typeof(T).GetInterfaces().Contains(typeof(IHasDateAuditing));
    #endregion

    #region Update
    public virtual void Update(FilterDefinition<T> filter, UpdateDefinition<T> update)
    {
      if (IsDateAuditing) {
        var setUpdated = Builders<T>.Update.CurrentDate("LastModified");
        var c = Builders<T>.Update.Combine(update, setUpdated);
        collection.UpdateOne(filter, c);
      } else
        collection.UpdateOne(filter, update);
    }

    public virtual async Task UpdateAsync(FilterDefinition<T> filter, UpdateDefinition<T> update)
    {
      if (IsDateAuditing) {
        var setUpdated = Builders<T>.Update.CurrentDate("LastModified");
        var c = Builders<T>.Update.Combine(update, setUpdated);
        await collection.UpdateOneAsync(filter, c);
      } else
        await collection.UpdateOneAsync(filter, update);
    } 
    #endregion


    #region Delete

    /// <inheritdoc />
    /// <summary>
    /// Deletes an entity from the repository by its id.
    /// </summary>
    /// <param name="id">The entity's id.</param>
    public virtual void Delete(TKey id)
    {
      var isWithEvents = IsWithEvents;
      var isDeleteProtected = IsDeleteProtected;
      var filter = Builders<T>.Filter.Eq(s => s.Id, id);

      if (isWithEvents) {
        var doc = GetById(id);
        if (doc == null) return;
        var evtBase = doc as IWithEvents;
        evtBase?.OnBeforeDelete();
      }
      if (isDeleteProtected) {
        //var update = Builders<T>.Update.Set("cuisine", "American (New)").CurrentDate("Deleted");
        var update = Builders<T>.Update.CurrentDate("Deleted");
        collection.UpdateOne(filter, update);
      } else {
        collection.DeleteOne(filter);
      }

    }

    /// <summary>
    /// Deletes an entity from the repository by its id.
    /// </summary>
    /// <param name="id">The entity's id.</param>
    public virtual async Task DeleteAsync(TKey id)
    {
      var isWithEvents = IsWithEvents;
      var isDeleteProtected = IsDeleteProtected;
      var filter = Builders<T>.Filter.Eq(s => s.Id, id);

      if (isWithEvents) {
        var doc = GetById(id);
        if (doc == null) return;
        var evtBase = doc as IWithEvents;
        evtBase?.OnBeforeDelete();
      }
      if (isDeleteProtected) {
        //var update = Builders<T>.Update.Set("cuisine", "American (New)").CurrentDate("Deleted");
        var update = Builders<T>.Update.CurrentDate("Deleted");
        await collection.UpdateOneAsync(filter, update);
      } else {
        await collection.DeleteOneAsync(filter);
      }

    }

    public virtual bool Restore(TKey id)
    {
      if (!IsDeleteProtected) throw new NotSupportedException($"Entity {typeof(T)} is not delete protected");
      var filter = Builders<T>.Filter.Eq(s => s.Id, id);
      var update = Builders<T>.Update.Set(p=> ((IDeleteProtected)p).Deleted, null);
      var result = collection.UpdateOne(filter, update);
      return result.ModifiedCount == 1;
    }
    public virtual async Task<bool> RestoreAsync(TKey id)
    {
      if (!IsDeleteProtected) throw new NotSupportedException($"Entity {typeof(T)} is not delete protected");
      var filter = Builders<T>.Filter.Eq(s => s.Id, id);
      var update = Builders<T>.Update.Set(p => ((IDeleteProtected)p).Deleted, null);
      var result = await collection.UpdateOneAsync(filter, update);
      return result.ModifiedCount == 1;
    }


    /// <inheritdoc />
    /// <summary>
    /// Deletes the given entity.
    /// </summary>
    /// <param name="entity">The entity to delete.</param>
    public virtual void Delete(T entity)
    {

      var isWithEvents = IsWithEvents;
      var isDeleteProtected = IsDeleteProtected;
      var filter = Builders<T>.Filter.Eq(s => s.Id, entity.Id);

      if (isWithEvents) {
        var evtBase = entity as IWithEvents;
        evtBase?.OnBeforeDelete();
      }
      if (isDeleteProtected) {
        var update = Builders<T>.Update.CurrentDate("Deleted");
        collection.UpdateOne(filter, update);
      } else {
        collection.DeleteOne(filter);
      }
    }

    /// <summary>
    /// Deletes the given entity.
    /// </summary>
    /// <param name="entity">The entity to delete.</param>
    public virtual async Task DeleteAsync(T entity)
    {

      var isWithEvents = IsWithEvents;
      var isDeleteProtected = IsDeleteProtected;
      var filter = Builders<T>.Filter.Eq(s => s.Id, entity.Id);

      if (isWithEvents) {
        var evtBase = entity as IWithEvents;
        evtBase?.OnBeforeDelete();
      }
      if (isDeleteProtected) {
        var update = Builders<T>.Update.CurrentDate("Deleted");
        await collection.UpdateOneAsync(filter, update);
      } else {
        await collection.DeleteOneAsync(filter);
      }
    }

    /// <summary>
    /// Deletes the entities matching the predicate.
    /// </summary>
    /// <param name="predicate">The expression.</param>
    public virtual void Delete(Expression<Func<T, bool>> predicate)
    {
      foreach (T entity in this.collection.AsQueryable<T>().Where(predicate)) {
        this.Delete(entity);
      }
    }

    /// <summary>
    /// Deletes the entities matching the predicate.
    /// </summary>
    /// <param name="predicate">The expression.</param>
    public virtual async Task DeleteAsync(Expression<Func<T, bool>> predicate)
    {
      foreach (T entity in this.collection.AsQueryable<T>().Where(predicate)) {
        await this.DeleteAsync(entity);
      }
    }

    /// <summary>
    /// Deletes all entities in the repository.
    /// </summary>
    public virtual void DeleteAll()
    {
      if (IsDeleteProtected) {
        var filter = Builders<T>.Filter.Where(p => ((IDeleteProtected)p).Deleted == null);
        var update = Builders<T>.Update.CurrentDate("Deleted");
        collection.UpdateMany(filter, update);
        return;
        /* - if the filter does not work
        foreach (T entity in this.collection.AsQueryable<T>()) {
          this.Delete(entity);
        }
        return;*/
      } else {
        this.collection.DeleteMany(new BsonDocument());
      }
    }

    /// <summary>
    /// Deletes all entities in the repository.
    /// </summary>
    public virtual async Task DeleteAllAsync()
    {
      if (IsDeleteProtected) {
        var filter = Builders<T>.Filter.Where(p => ((IDeleteProtected)p).Deleted == null);
        var update = Builders<T>.Update.CurrentDate("Deleted");
        await collection.UpdateManyAsync(filter, update);
        /* - if the filter does not work
        foreach (T entity in this.collection.AsQueryable<T>()) {
          this.Delete(entity);
        }
        return;*/
      } else {
        await this.collection.DeleteManyAsync(new BsonDocument());
      }
    }

    /// <summary>
    /// Truncate collection
    /// </summary>
    public virtual void Truncate()
    {
      this.collection.Database.DropCollection(this.CollectionName);
    }

    /// <summary>
    /// Truncate collection
    /// </summary>
    public virtual async Task TruncateAsync()
    {
      await this.collection.Database.DropCollectionAsync(this.CollectionName);
    }
    #endregion


    #region Counts
    /// <inheritdoc />
    /// <summary>
    /// Counts the total entities in the repository.
    /// </summary>
    /// <returns>Count of entities in the collection.</returns>
    public virtual long Count()
    {
      return collection.CountDocuments(new BsonDocument());
    }

    /// <summary>
    /// Counts the total entities in the repository.
    /// </summary>
    /// <returns>Count of entities in the collection.</returns>
    public virtual async Task<long> CountAsync()
    {
      return await collection.CountDocumentsAsync(new BsonDocument());
    }
    #endregion

    #region Exists
    /// <inheritdoc />
    /// <summary>
    /// Checks if the entity exists for given predicate.
    /// </summary>
    /// <param name="predicate">The expression.</param>
    /// <returns>True when an entity matching the predicate exists, false otherwise.</returns>
    public virtual bool Exists(Expression<Func<T, bool>> predicate)
    {
      return this.collection.AsQueryable<T>().Any(predicate);
    }

    /// <summary>
    /// Checks if the entity exists for given predicate.
    /// </summary>
    /// <param name="predicate">The expression.</param>
    /// <returns>True when an entity matching the predicate exists, false otherwise.</returns>
    public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
    {
      var n = await collection.FindAsync(predicate);
      return n.Any();
    }
    #endregion

    /// <summary>
    /// Lets the server know that this thread is about to begin a series of related operations that must all occur
    /// on the same connection. The return value of this method implements IDisposable and can be placed in a using
    /// statement (in which case RequestDone will be called automatically when leaving the using statement). 
    /// </summary>
    /// <returns>A helper object that implements IDisposable and calls RequestDone() from the Dispose method.</returns>
    /// <remarks>
    ///     <para>
    ///         Sometimes a series of operations needs to be performed on the same connection in order to guarantee correct
    ///         results. This is rarely the case, and most of the time there is no need to call RequestStart/RequestDone.
    ///         An example of when this might be necessary is when a series of Inserts are called in rapid succession with
    ///         SafeMode off, and you want to query that data in a consistent manner immediately thereafter (with SafeMode
    ///         off the writes can queue up at the server and might not be immediately visible to other connections). Using
    ///         RequestStart you can force a query to be on the same connection as the writes, so the query won't execute
    ///         until the server has caught up with the writes.
    ///     </para>
    ///     <para>
    ///         A thread can temporarily reserve a connection from the connection pool by using RequestStart and
    ///         RequestDone. You are free to use any other databases as well during the request. RequestStart increments a
    ///         counter (for this thread) and RequestDone decrements the counter. The connection that was reserved is not
    ///         actually returned to the connection pool until the count reaches zero again. This means that calls to
    ///         RequestStart/RequestDone can be nested and the right thing will happen.
    ///     </para>
    ///     <para>
    ///         Use the connectionstring to specify the readpreference; add "readPreference=X" where X is one of the following
    ///         values: primary, primaryPreferred, secondary, secondaryPreferred, nearest.
    ///         See http://docs.mongodb.org/manual/applications/replication/#read-preference
    ///     </para>
    /// </remarks>
    /*  public virtual IDisposable RequestStart() {
      this.collection.Database
      return this.collection.Database.RequestStart();
    }

    /// <summary>
    /// Lets the server know that this thread is done with a series of related operations.
    /// </summary>
    /// <remarks>
    /// Instead of calling this method it is better to put the return value of RequestStart in a using statement.
    /// </remarks>
    public virtual void RequestDone() {
      this.collection.Database.RequestDone();
    }
    */

    #region IQueryable<T>
    public virtual IQueryable<T> SafeList()
    {
      if (IsDeleteProtected)
        return this.collection.AsQueryable<T>().Where(p => ((IDeleteProtected)p).Deleted == null);
      return this.collection.AsQueryable<T>();
    }
    public virtual IQueryable<T> DeletedList()
    {
      if (IsDeleteProtected)
        return this.collection.AsQueryable<T>().Where(p => ((IDeleteProtected)p).Deleted != null);
      return new List<T>().AsQueryable();
    }

    /// <summary>
    /// Returns an enumerator that iterates through a collection.
    /// </summary>
    /// <returns>An IEnumerator&lt;T&gt; object that can be used to iterate through the collection.</returns>
    public virtual IEnumerator<T> GetEnumerator()
    {
      /*if (IsDeleteProtected)
        return this.collection.AsQueryable<T>().Where(p => ((IDeleteProtected)p).Deleted == null).GetEnumerator();
      */
      return this.collection.AsQueryable<T>().GetEnumerator();
    }

    /// <summary>
    /// Returns an enumerator that iterates through a collection.
    /// </summary>
    /// <returns>An IEnumerator object that can be used to iterate through the collection.</returns>
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      /*if (IsDeleteProtected)
        return this.collection.AsQueryable<T>().Where(p => ((IDeleteProtected)p).Deleted == null).GetEnumerator();
     */
      return this.collection.AsQueryable<T>().GetEnumerator();
    }

    /// <summary>
    /// Gets the type of the element(s) that are returned when the expression tree associated with this instance of IQueryable is executed.
    /// </summary>
    public virtual Type ElementType {
      get {
        /*  if (IsDeleteProtected)
            return this.collection.AsQueryable<T>().Where(p => ((IDeleteProtected)p).Deleted == null).ElementType;*/
        return this.collection.AsQueryable<T>().ElementType;
      }
    }

    /// <summary>
    /// Gets the expression tree that is associated with the instance of IQueryable.
    /// </summary>
    public virtual Expression Expression {
      get {
        /*   if (IsDeleteProtected)
             return this.collection.AsQueryable<T>().Where(p => ((IDeleteProtected)p).Deleted == null).Expression;*/
        return this.collection.AsQueryable<T>().Expression;
      }
    }

    /// <summary>
    /// Gets the query provider that is associated with this data source.
    /// </summary>
    public virtual IQueryProvider Provider {
      get {
        /* if (IsDeleteProtected) return this.collection.AsQueryable<T>().Where(p => ((IDeleteProtected)p).Deleted == null).Provider;*/
        return this.collection.AsQueryable<T>().Provider;
      }
    }
    #endregion
  }

  /// <summary>
  /// Deals with entities in MongoDb.
  /// </summary>
  /// <typeparam name="T">The type contained in the repository.</typeparam>
  /// <remarks>Entities are assumed to use strings for Id's.</remarks>
  public class MongoRepository<T> : MongoRepository<T, string>, IRepository<T>
      where T : IEntity<string>
  {
    /*
     * /// <summary>
     /// Initializes a new instance of the MongoRepository class.
     /// Uses the Default App/Web.Config connectionstrings to fetch the connectionString and Database name.
     /// </summary>
     /// <remarks>Default constructor defaults to "MongoServerSettings" key for connectionstring.</remarks>
     public MongoRepository()
         : base() { }
     */

    /// <summary>
    /// Initializes a new instance of the MongoRepository class.
    /// Uses the Default App/Web.Config connectionstrings to fetch the connectionString and Database name.
    /// </summary>
    /// <remarks>Default constructor defaults to "MongoServerSettings" key for connectionstring.</remarks>
    public MongoRepository(IMongoDatabase database)
        : base(database) { }

    /// <summary>
    /// Initializes a new instance of the MongoRepository class.
    /// </summary>
    /// <param name="url">Url to use for connecting to MongoDB.</param>
    public MongoRepository(MongoUrl url)
        : base(url) { }

    /// <summary>
    /// Initializes a new instance of the MongoRepository class.
    /// </summary>
    /// <param name="url">Url to use for connecting to MongoDB.</param>
    /// <param name="collectionName">The name of the collection to use.</param>
    public MongoRepository(MongoUrl url, string collectionName)
        : base(url, collectionName) { }

    /// <summary>
    /// Initializes a new instance of the MongoRepository class.
    /// </summary>
    /// <param name="connectionString">Connectionstring to use for connecting to MongoDB.</param>
    public MongoRepository(string connectionString)
        : base(connectionString) { }

    /// <summary>
    /// Initializes a new instance of the MongoRepository class.
    /// </summary>
    /// <param name="connectionString">Connectionstring to use for connecting to MongoDB.</param>
    /// <param name="collectionName">The name of the collection to use.</param>
    public MongoRepository(string connectionString, string collectionName)
        : base(connectionString, collectionName) { }
  }
}
