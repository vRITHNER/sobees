using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace WPF_Aero_Window.Native
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct Rect
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
        public Rect(int pLeft, int pTop, int pRight, int pBottom)
        {
            this = new Rect();
            this.Left = pLeft;
            this.Top = pTop;
            this.Right = pRight;
            this.Bottom = pBottom;
        }

        public static implicit operator System.Windows.Rect(Rect Value)
        {
            return new System.Windows.Rect((double)Value.Left, (double)Value.Top,
                (double)(Value.Right - Value.Left), (double)(Value.Bottom - Value.Top));
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
    internal struct Point
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
