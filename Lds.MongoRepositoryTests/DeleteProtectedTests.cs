using System;
using System.Threading.Tasks;
using Lds.MongoRepository;
using NUnit.Framework;

namespace Lds.MongoRepositoryTests
{
  class DeleteProtectedTests
  {
    private MongoRepository<Stubborn> repo;

    public DeleteProtectedTests()
    {
      repo = Repository.For<Stubborn>();
    }

    [Test]
    public async Task Can_Delete_Item()
    {
      var entity = new Stubborn(null, "Will not die");
      await repo.AddAsync(entity);

      Assert.IsNotNull(entity.Id);
      Assert.IsNull(entity.Deleted);

      await repo.DeleteAsync(entity);
    }

    [Test]
    public async Task Can_Not_Get_Deleted_Item()
    {
      var entity = new Stubborn(null, "Will not die");
      await repo.AddAsync(entity);

      Assert.IsNotNull(entity.Id);
      Assert.IsNull(entity.Deleted);

      await repo.DeleteAsync(entity);

      var tryToGet = await repo.GetByIdAsync(entity.Id);
      Assert.IsNull(tryToGet);
    }


    [Test]
    public async Task Can_Force_Get_Deleted_Item()
    {
      var entity = new Stubborn(null, "Will not die");
      await repo.AddAsync(entity);

      Assert.IsNotNull(entity.Id);
      Assert.IsNull(entity.Deleted);

      await repo.DeleteAsync(entity);

      var tryToGet = await repo.GetByIdAsync(entity.Id, true);
      Assert.IsNotNull(tryToGet);
      Assert.IsNotNull(tryToGet.Deleted);
    }

    [Test]
    public async Task Can_Restore_Deleted_Item()
    {
      var entity = new Stubborn(null, "Will not die");
      await repo.AddAsync(entity);

      Assert.IsNotNull(entity.Id);
      Assert.IsNull(entity.Deleted);

      await repo.DeleteAsync(entity);

      var tryToGet = await repo.GetByIdAsync(entity.Id);
      Assert.IsNull(tryToGet);

      await repo.RestoreAsync(entity.Id);

      var tryToGetAgain = await repo.GetByIdAsync(entity.Id);
      Assert.IsNotNull(tryToGetAgain);
      Assert.IsNull(tryToGetAgain.Deleted);

    }


    [Test]
    public void Can_Not_Restore_Any_Thing()
    {
      var thingRepo = Repository.For<Thing>();

      Assert.Throws<NotSupportedException>(() => { thingRepo.Restore("some"); });

    }
  }
}
