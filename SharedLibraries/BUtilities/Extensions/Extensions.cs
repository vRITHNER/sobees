#region

using System;
using System.Runtime.InteropServices;

#endregion

namespace Sobees.Library.BUtilities.Extensions
{
  public static class Extensions
  {
    #region Structs

    public static TStruct? AsNullable<TStruct>(this TStruct value) where TStruct : struct
    {
      return value;
    }

    public static TStruct ToStruct<TStruct>(this IntPtr value) where TStruct : struct
    {
      return (TStruct) Marshal.PtrToStructure(value, typeof (TStruct));
    }

    public static void WriteStruct(this IntPtr ptr, ValueType value)
    {
      Marshal.StructureToPtr(value, ptr, true);
    }

    #endregion
  }
}