using System.Dynamic;

namespace Sobees.Library.BTwitterLib.Response
{
  public class BTwitterResponseResult<T> where T : class
  {
    public static BTwitterResponseResult<T> CreateInstance()
    {
      return new BTwitterResponseResult<T>();
    }

    public T DataResult { get; set; }
    public bool BisSuccess { get; set; }
    public string ErrorMessage { get; set; }
  }
}