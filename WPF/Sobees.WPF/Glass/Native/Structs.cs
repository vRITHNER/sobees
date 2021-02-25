using System;
using System.Runtime.InteropServices;
using System.Windows;
using Sobees.Library.BUtilities;

namespace Sobees.Glass.Native
{
  //[StructLayout(LayoutKind.Sequential)]
  //internal struct Rect
  //{
  //  public int Left;
  //  public int Top;
  //  public int Right;
  //  public int Bottom;
  //  public Rect(int pLeft, int pTop, int pRight, int pBottom)
  //  {
  //    this = new Rect();
  //    this.Left = pLeft;
  //    this.Top = pTop;
  //    this.Right = pRight;
  //    this.Bottom = pBottom;
  //  }

  //  public static implicit operator System.Windows.Rect(Rect Value)
  //  {
  //    return new System.Windows.Rect((double)Value.Left, (double)Value.Top,
  //                                   (double)(Value.Right - Value.Left), (double)(Value.Bottom - Value.Top));
  //  }
  //}


  /// <summary> Win32 </summary>
  [StructLayout(LayoutKind.Sequential, Pack = 0)]
  public struct Rect
  {
    /// <summary> Win32 </summary>
    public int left;
    /// <summary> Win32 </summary>
    public int top;
    /// <summary> Win32 </summary>
    public int right;
    /// <summary> Win32 </summary>
    public int bottom;

    /// <summary> Win32 </summary>
    public static readonly Rect Empty = new Rect();

    /// <summary> Win32 </summary>
    public int Width => Math.Abs(right - left);

    /// <summary> Win32 </summary>
    public int Height => bottom - top;

    /// <summary> Win32 </summary>
    public Rect(int left, int top, int right, int bottom)
    {
      this.left = left;
      this.top = top;
      this.right = right;
      this.bottom = bottom;
    }


    /// <summary> Win32 </summary>
    public Rect(Rect rcSrc)
    {
      this.left = rcSrc.left;
      this.top = rcSrc.top;
      this.right = rcSrc.right;
      this.bottom = rcSrc.bottom;
    }

    /// <summary> Win32 </summary>
    public Rect (RECT rcSrc)
    {
      this.left = rcSrc.Left;
      this.top = rcSrc.Top;
      this.right = rcSrc.Right;
      this.bottom = rcSrc.Bottom;
    }

    /// <summary> Win32 </summary>
    public bool IsEmpty => left >= right || top >= bottom;

    /// <summary> Return a user friendly representation of this struct </summary>
    public override string ToString()
    {
      if (this == Rect.Empty) { return "RECT {Empty}"; }
      return "RECT { left : " + left + " / top : " + top + " / right : " + right + " / bottom : " + bottom + " }";
    }

    /// <summary> Determine if 2 RECT are equal (deep compare) </summary>
    public override bool Equals(object obj)
    {
      if (!(obj is Rect)) { return false; }
      return (this == (Rect)obj);
    }

    /// <summary>Return the HashCode for this struct (not garanteed to be unique)</summary>
    public override int GetHashCode()
    {
      return left.GetHashCode() + top.GetHashCode() + right.GetHashCode() + bottom.GetHashCode();
    }


    /// <summary> Determine if 2 RECT are equal (deep compare)</summary>
    public static bool operator ==(Rect rect1, Rect rect2)
    {
      return (rect1.left == rect2.left && rect1.top == rect2.top && rect1.right == rect2.right && rect1.bottom == rect2.bottom);
    }

    /// <summary> Determine if 2 RECT are different(deep compare)</summary>
    public static bool operator !=(Rect rect1, Rect rect2)
    {
      return !(rect1 == rect2);
    }


  }


  [StructLayout(LayoutKind.Sequential)]
  internal struct MonitorInfo
  {
    public uint cbSize;
    public Rect rcMonitor;
    public Rect rcWork;
    public uint dwFlags;
  }

  /// <summary>
  /// </summary>
  [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
  public class MONITORINFO
  {
    /// <summary>
    /// </summary>            
    public int cbSize = Marshal.SizeOf(typeof(MONITORINFO));

    /// <summary>
    /// </summary>            
    public Rect rcMonitor = new Rect();

    /// <summary>
    /// </summary>            
    public Rect rcWork = new Rect();

    /// <summary>
    /// </summary>            
    public int dwFlags = 0;
  }

  [StructLayout(LayoutKind.Sequential)]
  public struct MINMAXINFO
  {
    public Point ptReserved;
    public Point ptMaxSize;
    public Point ptMaxPosition;
    public Point ptMinTrackSize;
    public Point ptMaxTrackSize;
  }; 

  [StructLayout(LayoutKind.Sequential)]
  internal struct MinMaxInfo
  {
    public Point ptReserved;
    public Point ptMaxSize;
    public Point ptMaxPosition;
    public Point ptMinTrackSize;
    public Point ptMaxTrackSize;
  }

  [StructLayout(LayoutKind.Sequential)]
  public struct NCCALCSIZE_PARAMS
  {
    public Rect r1, r2, r3;
    public IntPtr lppos;
  } 

  [StructLayout(LayoutKind.Sequential)]
  public struct Point
  {
    public int X;
    public int Y;
    public Point(int X, int Y)
    {
      this = new Point();
      this.X = X;
      this.Y = Y;
    }

    public static implicit operator System.Windows.Point(Point Value)
    {
      return new Point(Value.X, Value.Y);
    }

    public static implicit operator Point(System.Windows.Point Value)
    {
      return new Point((int)Math.Round(Value.X), (int)Math.Round(Value.Y));
    }
  }

  [StructLayout(LayoutKind.Sequential)]
  internal struct MARGINS
  {
    public int Left;
    public int Right;
    public int Top;
    public int Bottom;
    public static explicit operator MARGINS(Thickness? Value)
    {
      return (MARGINS)Value.GetValueOrDefault(new Thickness(-1));
    }
    public static explicit operator Thickness(MARGINS Value)
    {
      return new Thickness { Bottom = Value.Bottom, Left = Value.Left, Right = Value.Right, Top = Value.Top };
    }
    public static explicit operator MARGINS(Thickness Value)
    {
      return new MARGINS
               {
                 Bottom = (int)Math.Round(Value.Bottom),
                 Left = (int)Math.Round(Value.Left),
                 Right = (int)Math.Round(Value.Right),
                 Top = (int)Math.Round(Value.Top)
               };
    }
  }

}


