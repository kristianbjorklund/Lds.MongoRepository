using System;
using Lds.MongoRepository;
using Lds.MongoRepository.Interfaces;

namespace Lds.MongoRepositoryTests
{
  public sealed class Stuff : Entity, IEntity, IHasDateAuditing
  {
    private Stuff()
    {

    }

    public Stuff(string id, string name)
    {
      Id = id;
      Name = name;
    }

    public string Name { get; set; }

    public override string ToString()
    {
      return $"{Name}, {Id}";
    }

    public DateTime Created { get; set; }
    public DateTime LastModified { get; set; }
  }
}
