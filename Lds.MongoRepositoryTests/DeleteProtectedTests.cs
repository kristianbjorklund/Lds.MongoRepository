using System;
using System.Threading.Tasks;
using Lds.MongoRepository;
using NUnit.Framework;

namespace Lds.MongoRepositoryTests
{
  internal class DeleteProtectedTests
  {
    private readonly MongoRepository<Stubborn> _repo;

    public DeleteProtectedTests()
    {
      _repo = Repository.For<Stubborn>();
    }

    [Test]
    public async Task Can_Delete_Item()
    {
      var entity = new Stubborn(null, "Will not die");
      await _repo.AddAsync(entity);

      Assert.That(entity.Id, Is.Not.Null);
      Assert.That(entity.Deleted, Is.Null);

      await _repo.DeleteAsync(entity);
    }

    [Test]
    public async Task Can_Not_Get_Deleted_Item()
    {
      var entity = new Stubborn(null, "Will not die");
      await _repo.AddAsync(entity);

      Assert.That(entity.Id, Is.Not.Null);
      Assert.That(entity.Deleted, Is.Null);

      await _repo.DeleteAsync(entity);

      var tryToGet = await _repo.GetByIdAsync(entity.Id);
      Assert.That(tryToGet, Is.Null);
    }


    [Test]
    public async Task Can_Force_Get_Deleted_Item()
    {
      var entity = new Stubborn(null, "Will not die");
      await _repo.AddAsync(entity);

      Assert.That(entity.Id, Is.Not.Null);
      Assert.That(entity.Deleted,Is.Null);

      await _repo.DeleteAsync(entity);

      var tryToGet = await _repo.GetByIdAsync(entity.Id, true);
      Assert.That(tryToGet, Is.Not.Null);
      Assert.That(tryToGet.Deleted, Is.Not.Null);
    }

    [Test]
    public async Task Can_Restore_Deleted_Item()
    {
      var entity = new Stubborn(null, "Will not die");
      await _repo.AddAsync(entity);

      Assert.That(entity.Id, Is.Not.Null);
      Assert.That(entity.Deleted, Is.Null);
      await _repo.DeleteAsync(entity);

      var tryToGet = await _repo.GetByIdAsync(entity.Id);
      Assert.That(tryToGet, Is.Null);
      await _repo.RestoreAsync(entity.Id);

      var tryToGetAgain = await _repo.GetByIdAsync(entity.Id);
      Assert.That(tryToGetAgain, Is.Not.Null);
      Assert.That(tryToGetAgain.Deleted, Is.Null);

    }


    [Test]
    public void Can_Not_Restore_Any_Thing()
    {
      var thingRepo = Repository.For<Thing>();

      Assert.Throws<NotSupportedException>(() => { thingRepo.Restore("some"); });

    }
  }
}
