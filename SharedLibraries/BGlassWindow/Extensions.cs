using System;
using System.Runtime.InteropServices;

namespace BGlassWindow
{
  internal static class Extensions
  {
    internal static TStruct? AsNullable<TStruct>(this TStruct Value) where TStruct : struct
    { return new Nullable<TStruct>(Value); }
    internal static TStruct ToStruct<TStruct>(this IntPtr Value) where TStruct : struct
    { return (TStruct)Marshal.PtrToStructure(Value, typeof(TStruct)); }
    internal static void WriteStruct(this IntPtr Ptr, ValueType Value)
    { Marshal.StructureToPtr(Value, Ptr, true); }
  }
}


