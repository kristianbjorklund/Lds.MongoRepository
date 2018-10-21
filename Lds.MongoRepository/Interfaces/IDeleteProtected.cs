using System;

namespace Lds.MongoRepository.Interfaces
{
  public interface IDeleteProtected {
    DateTime? Deleted { get; set; }
  }
}