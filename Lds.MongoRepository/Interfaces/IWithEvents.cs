namespace Lds.MongoRepository.Interfaces {
  public interface IWithEvents {
    void OnBeforeSave();
    void OnBeforeDelete();
  }
}