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
  class DateAuditingTests
  {

    [Test]
    public async Task Can_Set_Created_On_Stuff()
    {
      var repo = Repository.For<Stuff>();
      var stuff = new Stuff(null, "No-name");
      await repo.AddAsync(stuff);

      Assert.IsNotNull(stuff.Id);
      Assert.Greater(stuff.Created, DateTime.UtcNow.AddMinutes(-5));
      Assert.Greater(stuff.LastModified, DateTime.UtcNow.AddMinutes(-5));
    }


    [Test]
    public async Task Can_Set_Modified_On_Stuff()
    {
      var repo = Repository.For<Stuff>();
      var stuff = new Stuff(null, "No-name2");
      await repo.AddAsync(stuff);

      Assert.IsNotNull(stuff.Id);

      stuff.Name = "No-name3";
      Thread.Sleep(1000);

      await repo.ReplaceAsync(stuff);
      Assert.Greater(stuff.LastModified, stuff.Created);

    }

  }
}
