using System.Linq;
using Lds.MongoRepository;
using NUnit.Framework;

namespace Lds.MongoRepositoryTests
{
  [TestFixture]
  public class MainTests
  {
   

    [Test]
    public void CanDoNothing()
    {
      Assert.IsNull(null);
    }

    [Test]
    public void Can_Get_Repo()
    {
      var repo = Repository.For<Thing>();

      Assert.IsNotNull(repo);
      Assert.IsInstanceOf<MongoRepository<Thing>>(repo);
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

      Assert.IsNotNull(thing.Id);
    }


    [Test]
    public void Can_Load_Thing()
    {
      var repo = Repository.For<Thing>();
      repo.Add(new Thing(null,"new stuff"));

      var thing = repo.First();

      Assert.IsNotNull(thing);
      Assert.IsInstanceOf<Thing>(thing);
    }

  }
}
