using System.Collections.Generic;

namespace Carsharing.Interfaces
{
  public interface ILoadEntity<T>
  {
    List<T> LoadData();
  }
}