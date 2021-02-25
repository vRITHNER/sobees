#region

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Sobees.Library.BUtilities;
using Sobees.Tools.Screen;

#endregion

namespace Sobees.Tools.Logging
{
  public static class CaptureScreenHelper
  {
    public static BitmapSource CaptureScreenToFile(string fileName)
    {
      var hwnd = new WindowInteropHelper(Application.Current.MainWindow).Handle;
      var rect = ScreenHelper.GetCurrentMonitorSize(hwnd);

      if (rect.Right == 0)
        NativeMethods.GetWindowRect(hwnd, out rect);

      return CaptureScreenToFile(rect, fileName);
    }

    public static BitmapSource CaptureScreenToFile(RECT area,
                                                   string fileName)
    {
      var result = string.Empty;
      var b = Capture(area);

      var extension = Path.GetExtension(fileName).ToLower();

      BitmapEncoder encoder = null;
      if (extension == ".gif")
        encoder = new GifBitmapEncoder();
      else if (extension == ".png")
        encoder = new PngBitmapEncoder();
      else if (extension == ".jpg")
        encoder = new JpegBitmapEncoder();

      encoder.Frames.Add(BitmapFrame.Create(b));

      using (Stream fs = File.Create(fileName))
      {
        encoder.Save(fs);
        result = fileName;
        fs.Flush();
        fs.Dispose();
      }

      return b;
    }

    public static BitmapSource Capture(RECT area)
    {
      var screenDC = GetDC(IntPtr.Zero);
      var memDC = CreateCompatibleDC(screenDC);

      var hBitmap = CreateCompatibleBitmap(screenDC, area.Right - area.Left, area.Bottom);
      SelectObject(memDC, hBitmap); // Select bitmap from compatible bitmap to memDC

      // TODO: BitBlt may fail horribly
      BitBlt(memDC, 0, 0, area.Right - area.Left, area.Bottom, screenDC, area.Left, area.Top, TernaryRasterOperations.SRCCOPY);

      var bsource = Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());


      DeleteObject(hBitmap);
      ReleaseDC(IntPtr.Zero, screenDC);
      ReleaseDC(IntPtr.Zero, memDC);
      return bsource;
    }

    #region WINAPI DLL Imports

    [DllImport("gdi32.dll", ExactSpelling = true, PreserveSig = true, SetLastError = true)]
    private static extern IntPtr SelectObject(IntPtr hdc,
                                              IntPtr hgdiobj);

    [DllImport("gdi32.dll")]
    private static extern IntPtr CreateCompatibleBitmap(IntPtr hdc,
                                                        int nWidth,
                                                        int nHeight);

    [DllImport("gdi32.dll", SetLastError = true)]
    private static extern IntPtr CreateCompatibleDC(IntPtr hdc);

    [DllImport("gdi32.dll")]
    private static extern bool DeleteObject(IntPtr hObject);

    [DllImport("gdi32.dll")]
    private static extern IntPtr CreateBitmap(int nWidth,
                                              int nHeight,
                                              uint cPlanes,
                                              uint cBitsPerPel,
                                              IntPtr lpvBits);

    [DllImport("user32.dll")]
    private static extern IntPtr GetDC(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern int ReleaseDC(IntPtr hWnd,
                                        IntPtr hDC);

    [DllImport("gdi32.dll")]
    private static extern bool BitBlt(IntPtr hdc,
                                      int nXDest,
                                      int nYDest,
                                      int nWidth,
                                      int nHeight,
                                      IntPtr hdcSrc,
                                      int nXSrc,
                                      int nYSrc,
                                      TernaryRasterOperations dwRop);

    private enum TernaryRasterOperations : uint
    {
      /// <summary>
      ///   dest = source
      /// </summary>
      SRCCOPY = 0x00CC0020,

      /// <summary>
      ///   dest = source OR dest
      /// </summary>
      SRCPAINT = 0x00EE0086,

      /// <summary>
      ///   dest = source AND dest
      /// </summary>
      SRCAND = 0x008800C6,

      /// <summary>
      ///   dest = source XOR dest
      /// </summary>
      SRCINVERT = 0x00660046,

      /// <summary>
      ///   dest = source AND (NOT dest)
      /// </summary>
      SRCERASE = 0x00440328,

      /// <summary>
      ///   dest = (NOT source)
      /// </summary>
      NOTSRCCOPY = 0x00330008,

      /// <summary>
      ///   dest = (NOT src) AND (NOT dest)
      /// </summary>
      NOTSRCERASE = 0x001100A6,

      /// <summary>
      ///   dest = (source AND pattern)
      /// </summary>
      MERGECOPY = 0x00C000CA,

      /// <summary>
      ///   dest = (NOT source) OR dest
      /// </summary>
      MERGEPAINT = 0x00BB0226,

      /// <summary>
      ///   dest = pattern
      /// </summary>
      PATCOPY = 0x00F00021,

      /// <summary>
      ///   dest = DPSnoo
      /// </summary>
      PATPAINT = 0x00FB0A09,

      /// <summary>
      ///   dest = pattern XOR dest
      /// </summary>
      PATINVERT = 0x005A0049,

      /// <summary>
      ///   dest = (NOT dest)
      /// </summary>
      DSTINVERT = 0x00550009,

      /// <summary>
      ///   dest = BLACK
      /// </summary>
      BLACKNESS = 0x00000042,

      /// <summary>
      ///   dest = WHITE
      /// </summary>
      WHITENESS = 0x00FF0062
    }

    #endregion
  }
}