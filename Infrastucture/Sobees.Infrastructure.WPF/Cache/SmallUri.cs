using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Web;

//using System.Web;

namespace Sobees.Infrastructure.Cache
{
  [DebuggerDisplay("SmallUri: { GetUri() }")]
  public struct SmallUri : IEquatable<SmallUri>
  {
    private static readonly UTF8Encoding s_Encoder = new UTF8Encoding(false /* do not emit BOM */, true
      /* throw on error */);

    private readonly bool _isHttp;
    private readonly byte[] _utf8String;

    public SmallUri(Uri value,ImageKind kind) : this()
    {
      _isHttp = false;
      _utf8String = null;
      Kind = kind;
      if (value == null)
      {
        return;
      }

      if (!value.IsAbsoluteUri)
      {
        throw new ArgumentException("The parameter is not a valid absolute uri", "value");
      }

      string strValue = value.OriginalString;
      if (strValue.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
      {
        _isHttp = true;
        strValue = strValue.Substring(7);
      }

      _utf8String = s_Encoder.GetBytes(strValue);
    }

    public SmallUri(string value,ImageKind kind) : this()
    {
      _isHttp = false;
      _utf8String = null;
      Kind = kind;
      if (string.IsNullOrEmpty(value))
      {
        return;
      }
      try
      {
        value = HttpUtility.UrlPathEncode(value);
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
      }
      if (!Uri.IsWellFormedUriString(value, UriKind.Absolute))
      {
        throw new ArgumentException("The parameter is not a valid uri", "value");
      }

      if (value.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
      {
        _isHttp = true;
        value = value.Substring(7);
      }

      _utf8String = s_Encoder.GetBytes(value);
      
    }

    public ImageKind Kind { get; set; }

    #region Object Overrides

    public override string ToString()
    {
      throw new NotSupportedException(
        "This exception exists to prevent accidental performance penalties.  Call GetString() instead.");
    }

    public override int GetHashCode()
    {
      // Intentionally hashes similarly to the expanded strings.
      return GetString().GetHashCode();
    }

    public override bool Equals(object obj)
    {
      try
      {
        return Equals((SmallUri) obj);
      }
      catch (InvalidCastException)
      {
        return false;
      }
    }

    #endregion

    #region IEquatable<SmallUri> Members

    public bool Equals(SmallUri other)
    {
      if (_utf8String == null)
      {
        return other._utf8String == null;
      }

      if (other._utf8String == null)
      {
        return false;
      }

      if (_isHttp != other._isHttp)
      {
        return false;
      }

      if (_utf8String.Length != other._utf8String.Length)
      {
        return false;
      }

      return MemCmp(_utf8String, other._utf8String, _utf8String.Length);
    }

    #endregion

    public string GetString()
    {
      if (_utf8String == null)
      {
        return "";
      }
      return GetUri().ToString();
    }

    public Uri GetUri()
    {
      if (_utf8String == null)
      {
        return null;
      }
      return new Uri((_isHttp ? "http://" : "") + s_Encoder.GetString(_utf8String), UriKind.Absolute);
    }

    public static bool operator ==(SmallUri left, SmallUri right)
    {
      return left.Equals(right);
    }

    public static bool operator !=(SmallUri left, SmallUri right)
    {
      return !left.Equals(right);
    }

    public static bool MemCmp(byte[] left, byte[] right, int cb)
    {
      // pin this buffer
      GCHandle handleLeft = GCHandle.Alloc(left, GCHandleType.Pinned);
      IntPtr ptrLeft = handleLeft.AddrOfPinnedObject();

      // pin the other buffer
      GCHandle handleRight = GCHandle.Alloc(right, GCHandleType.Pinned);
      IntPtr ptrRight = handleRight.AddrOfPinnedObject();

      bool fRet = _MemCmp(ptrLeft, ptrRight, cb);

      handleLeft.Free();
      handleRight.Free();

      return fRet;
    }

    private static bool _MemCmp(IntPtr left, IntPtr right, long cb)
    {
      int offset = 0;

      for (; offset < (cb - sizeof (Int64)); offset += sizeof (Int64))
      {
        Int64 left64 = Marshal.ReadInt64(left, offset);
        Int64 right64 = Marshal.ReadInt64(right, offset);

        if (left64 != right64)
        {
          return false;
        }
      }

      for (; offset < cb; offset += sizeof (byte))
      {
        byte left8 = Marshal.ReadByte(left, offset);
        byte right8 = Marshal.ReadByte(right, offset);

        if (left8 != right8)
        {
          return false;
        }
      }

      return true;
    }
  }
}