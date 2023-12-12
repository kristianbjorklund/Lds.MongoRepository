using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Lds.MongoRepository;
using NUnit.Framework;

namespace Lds.MongoRepositoryTests
{
  internal class DateAuditingTests
  {

    [Test]
    public async Task Can_Set_Created_On_Stuff()
    {
      var repo = Repository.For<Stuff>();
      var stuff = new Stuff(null, "No-name");
      await repo.AddAsync(stuff);

      Assert.That(stuff.Id, Is.Not.Null);

      Assert.That(stuff.Created, Is.GreaterThan(DateTime.UtcNow.AddMinutes(-5)));
      Assert.That(stuff.LastModified, Is.GreaterThan(DateTime.UtcNow.AddMinutes(-5)));
    }


    [Test]
    public async Task Can_Set_Modified_On_Stuff()
    {
      var repo = Repository.For<Stuff>();
      var stuff = new Stuff(null, "No-name2");
      await repo.AddAsync(stuff);

      Assert.That(stuff.Id, Is.Not.Null);

      stuff.Name = "No-name3";
      Thread.Sleep(1000);

      await repo.ReplaceAsync(stuff);
      Assert.That(stuff.LastModified, Is.GreaterThan(stuff.Created));

    }

  }
}
