using System.Linq;
using Lds.MongoRepository;
using NUnit.Framework;

namespace Lds.MongoRepositoryTests
{
  [TestFixture]
  public class MainTests
  {
   
    [Test]
    public void Can_Get_Repo()
    {
      var repo = Repository.For<Thing>();

      Assert.That(repo, Is.Not.Null);
      Assert.That(repo, Is.InstanceOf<MongoRepository<Thing>>());
    }

    [Test]
    public void Can_Use_Collection()
    {
      var repo = Repository.For<Thing>();
      repo.Collection.InsertOne(new Thing(null,"Test"));
    }

    [Test]
    public void Can_Save_Thing_Without_Id()
    {
      var repo = Repository.For<Thing>();
      var thing = new Thing(null,"No-name");
      repo.Add(thing);

      Assert.That(thing.Id, Is.Not.Null);
    }

    [Test]
    public void Can_Load_Thing()
    {
      var repo = Repository.For<Thing>();
      repo.Add(new Thing(null,"new stuff"));

      var thing = repo.First();

      Assert.That(thing, Is.Not.Null);
      Assert.That(thing, Is.InstanceOf<Thing>());
    }
  }
}
