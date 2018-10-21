using Lds.MongoRepository;
using NUnit.Framework;

namespace Lds.MongoRepositoryTests
{
  [SetUpFixture]
  public class TestSetup
  {
    [OneTimeSetUp]
    public void RunBeforeAnyTests()
    {
      MainDb.Creator = new TestCreator();
    }

    [OneTimeTearDown]
    public void RunAfterAnyTests()
    {
      MainDb.Creator.DestroyDatabase();
    }
  }
}
