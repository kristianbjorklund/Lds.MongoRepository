using Lds.MongoRepository;

namespace Lds.MongoRepositoryTests
{
  public sealed class Thing : Entity, IEntity
  {
    private Thing()
    {

    }

    public Thing(string id, string name)
    {
      Id = id;
      Name = name;
    }

    public string Name { get; set; }

    public override string ToString()
    {
      return $"{Name}, {Id}";
    }
  }
}