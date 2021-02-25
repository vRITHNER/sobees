using System;
using System.Text;

namespace Sobees.Library.BGoogleLib.Core
{
  public class GapiException : Exception
  {
    public GapiException(string message)
      : base(message)
    {
    }
  }
}