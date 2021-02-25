#region

using System;
using System.Security.Cryptography;
using System.Text;

#endregion

namespace Sobees.Tools.Crypto
{
  public class MD5Helper
  {
    /// <summary>
    ///   Calculates a MD5 hash from the given string and uses the given
    ///   encoding.
    /// </summary>
    /// <param name = "Input">Input string</param>
    /// <param name = "UseEncoding">Encoding method</param>
    /// <returns>MD5 computed string</returns>
    public static string CalculateMD5(string Input,
                                      Encoding UseEncoding)
    {
      MD5CryptoServiceProvider CryptoService;
      CryptoService = new MD5CryptoServiceProvider();

      var InputBytes = UseEncoding.GetBytes(Input);
      InputBytes = CryptoService.ComputeHash(InputBytes);
      return BitConverter.ToString(InputBytes).Replace("-", "");
    }

    /// <summary>
    ///   Calculates a MD5 hash from the given string. 
    ///   (By using the default encoding)
    /// </summary>
    /// <param name = "Input">Input string</param>
    /// <returns>MD5 computed string</returns>
    public static string CalculateMD5(string Input)
    {
      // That's just a shortcut to the base method
      return CalculateMD5(Input, Encoding.Default);
    }
  }
}