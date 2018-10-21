using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Lds.MongoRepository
{
  /// <inheritdoc />
  /// <summary>
  /// IRepository definition.
  /// </summary>
  /// <typeparam name="T">The type contained in the repository.</typeparam>
  /// <typeparam name="TKey">The type used for the entity's Id.</typeparam>
  public interface IRepository<T, in TKey> : IQueryable<T>
      where T : IEntity<TKey>
  {
    /// <summary>
    /// Gets the Mongo collection (to perform advanced operations).
    /// </summary>
    /// <remarks>
    /// One can argue that exposing this property (and with that, access to it's Database property for instance
    /// (which is a "parent")) is not the responsibility of this class. Use of this property is highly discouraged;
    /// for most purposes you can use the MongoRepositoryManager&lt;T&gt;
    /// </remarks>
    /// <value>The Mongo collection (to perform advanced operations).</value>
    IMongoCollection<T> Collection { get; }

    /// <summary>
    /// Returns the T by its given id.
    /// </summary>
    /// <param name="id">The value representing the ObjectId of the entity to retrieve.</param>
    /// <returns>The Entity T.</returns>
    T GetById(TKey id);
    Task<T> GetByIdAsync(TKey id);

    /// <summary>
    /// Adds the new entity in the repository.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <returns>The added entity including its new ObjectId.</returns>
    void Add(T entity);
    Task AddAsync(T entity);

    /// <summary>
    /// Adds the new entities in the repository.
    /// </summary>
    /// <param name="entities">The entities of type T.</param>
    void Add(IEnumerable<T> entities);
    Task AddAsync(IEnumerable<T> entities);

    /// <summary>
    /// Replaces an entity.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <returns>The Replaced entity.</returns>
    bool Replace(T entity);
    Task<bool> ReplaceAsync(T entity);

    /// <summary>
    /// Replaces the entities.
    /// </summary>
    /// <param name="entities">The entities to Replace.</param>
    bool Replace(IEnumerable<T> entities);
    Task<bool> ReplaceAsync(IEnumerable<T> entities);

    /// <summary>
    /// Deletes an entity from the repository by its id.
    /// </summary>
    /// <param name="id">The entity's id.</param>
    void Delete(TKey id);
    Task DeleteAsync(TKey id);

    /// <summary>
    /// Restores a deleted entity from the repository by its id.
    /// </summary>
    /// <param name="id">The entity's id.</param>
    bool Restore(TKey id);
    Task<bool> RestoreAsync(TKey id);

    void Update(FilterDefinition<T> filter, UpdateDefinition<T> update);
    Task UpdateAsync(FilterDefinition<T> filter, UpdateDefinition<T> update);

    /// <summary>
    /// Deletes the given entity.
    /// </summary>
    /// <param name="entity">The entity to delete.</param>
    void Delete(T entity);
    Task DeleteAsync(T entity);

    /// <summary>
    /// Deletes the entities matching the predicate.
    /// </summary>
    /// <param name="predicate">The expression.</param>
    void Delete(Expression<Func<T, bool>> predicate);
    Task DeleteAsync(Expression<Func<T, bool>> predicate);

    /// <summary>
    /// Deletes all entities in the repository.
    /// </summary>
    void DeleteAll();
    Task DeleteAllAsync();

    /// <summary>
    /// Truncate collection
    /// </summary>
    void Truncate();
    Task TruncateAsync();

    /// <summary>
    /// Counts the total entities in the repository.
    /// </summary>
    /// <returns>Count of entities in the repository.</returns>
    long Count();
    Task<long> CountAsync();

    /// <summary>
    /// Checks if the entity exists for given predicate.
    /// </summary>
    /// <param name="predicate">The expression.</param>
    /// <returns>True when an entity matching the predicate exists, false otherwise.</returns>
    bool Exists(Expression<Func<T, bool>> predicate);
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
    /*
        /// <summary>
        /// Lets the server know that this thread is about to begin a series of related operations that must all occur
        /// on the same connection. The return value of this method implements IDisposable and can be placed in a using
        /// statement (in which case RequestDone will be called automatically when leaving the using statement). 
        /// </summary>
        /// <returns>A helper object that implements IDisposable and calls RequestDone() from the Dispose method.</returns>
        /// <remarks>
        /// Sometimes a series of operations needs to be performed on the same connection in order to guarantee correct
        /// results. This is rarely the case, and most of the time there is no need to call RequestStart/RequestDone.
        /// An example of when this might be necessary is when a series of Inserts are called in rapid succession with
        /// SafeMode off, and you want to query that data in a consistent manner immediately thereafter (with SafeMode
        /// off the writes can queue up at the server and might not be immediately visible to other connections). Using
        /// RequestStart you can force a query to be on the same connection as the writes, so the query won't execute
        /// until the server has caught up with the writes.
        /// A thread can temporarily reserve a connection from the connection pool by using RequestStart and
        /// RequestDone. You are free to use any other databases as well during the request. RequestStart increments a
        /// counter (for this thread) and RequestDone decrements the counter. The connection that was reserved is not
        /// actually returned to the connection pool until the count reaches zero again. This means that calls to
        /// RequestStart/RequestDone can be nested and the right thing will happen.
        /// </remarks>
        IDisposable RequestStart();

        /// <summary>
        /// Lets the server know that this thread is done with a series of related operations.
        /// </summary>
        /// <remarks>
        /// Instead of calling this method it is better to put the return value of RequestStart in a using statement.
        /// </remarks>
        void RequestDone();*/
  }

  /// <inheritdoc>
  ///   <cref></cref>
  /// </inheritdoc>
  /// <summary>
  /// IRepository definition.
  /// </summary>
  /// <typeparam name="T">The type contained in the repository.</typeparam>
  /// <remarks>Entities are assumed to use strings for Id's.</remarks>
  public interface IRepository<T> : IQueryable<T>, IRepository<T, string>
      where T : IEntity<string>
  { }
}
