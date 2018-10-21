using System;
using Lds.MongoRepository;
using Lds.MongoRepository.Interfaces;

namespace Lds.MongoRepositoryTests
{
  public sealed class Stubborn : Entity, IEntity, IDeleteProtected
  {
    private Stubborn()
    {

    }

    public Stubborn(string id, string name)
    {
      Id = id;
      Name = name;
    }

    public string Name { get; set; }

    public override string ToString()
    {
      return $"{Name}, {Id}";
    }

    public DateTime? Deleted { get; set; }
  }
}