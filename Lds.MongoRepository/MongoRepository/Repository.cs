using System.Collections.Generic;

namespace Lds.MongoRepository
{
  public sealed class Repository
  {

    private static volatile Dictionary<string, object> _repos = new Dictionary<string, object>();
    private static readonly object SyncRoot = new object();

    //public static MongoRepository<T> For<T>() where T : class, IEntity {
    //  if (HttpRuntime.AppDomainAppId != null) {
    //    //is web app
    //    var key = "Repository_" + typeof(T).Name;
    //    return (HttpContext.Current.Items[key] ?? (HttpContext.Current.Items[key] = CreateNew<T>())) as MongoRepository<T>;
    //  } else {
    //    var repos = CreateNew<T>();
    //    return repos;
    //  }
    //}

    public static MongoRepository<T> For<T>() where T : class, IEntity
    {
      object objInstance;
      var key = "Repository_" + typeof(T).Name;

      if (_repos.TryGetValue(key, out objInstance)) {
        var instance = objInstance as MongoRepository<T>;
        if (instance != null) return instance;
      }

      lock (SyncRoot) {

        if (_repos.TryGetValue(key, out objInstance)) {
          var instance = objInstance as MongoRepository<T>;
          if (instance != null) return instance;
        }
        {
          var instance = new MongoRepository<T>(MainDb.Current);
          _repos[key] = instance;
          return instance;
        }
      }

    }


    //private static MongoRepository<T> CreateNew<T>() where T : class, IEntity {
    //  //todo: Lave en metode, som kan tage imod en client i stedet - for at gøre det hurtigere
    //  var repos = new MongoRepository<T>(MainDb.Current);
    //  return repos;
    //}
  }

}
