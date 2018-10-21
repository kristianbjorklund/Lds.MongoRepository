using System;

namespace Lds.MongoRepository.Interfaces {
  public interface IHasDateAuditing {
    DateTime Created { get; set; }
    DateTime LastModified { get; set; }
  }
}