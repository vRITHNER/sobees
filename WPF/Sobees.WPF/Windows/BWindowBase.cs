#region

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using Sobees.Glass.Native;
using Sobees.Library.BUtilities;
using Sobees.Tools.Threading;
using Point = System.Windows.Point;
using WM = Sobees.Library.BUtilities.WM;

#endregion

namespace Sobees.Windows
{
  public class BWindowBase : Window
  {
    private static bool? _isInDesignMode;

    #region Properties

    public bool BisClosing { get; set; }

    /// <summary>
    ///   Can be assigned on initialization if the window.
    ///   Changing value doesn't make effect on already opened window.
    /// </summary>
    public int CaptionHeight { get; set; }

    /// <summary>
    ///   Can be assigned on initialization if the window.
    ///   Changing value doesn't make effect on already opened window.
    /// </summary>
    public int WindowDragZoneHeight { get; set; }

    public bool CenterInParentWindow { get; set; }

    /// <summary>
    ///   Property used to Minimize Window in Tray all Times
    /// </summary>
    public virtual bool BisMinimizeWindowInTray { get; set; }

    /// <summary>
    ///   Property used for menu to Minimize Window in Tray Once Time
    /// </summary>
    public virtual bool BisMinimizeWindowOnceInTray { get; set; }

    #region Constructor

    private WindowLocation _windowLocation;


    static BWindowBase()
    {
    }


    public BWindowBase(string windowName, bool bRestoreFromXml, WindowLocation windowLocation)
    {
      _windowLocation = windowLocation;

      //InitializeContentControls();
      WindowStartupLocation = WindowStartupLocation.Manual;

      CaptionHeight = (int) SystemParameters.CaptionHeight;

      //WindowName = windowName;
      Name = windowName;
      BRestoreFromXml = bRestoreFromXml;

      if (!IsInDesignMode)
        Loaded += BWindowBase_Loaded;
    }


    protected BWindowBase()
    {
      //
    }

    #endregion

    /// <summary>
    ///   Display the system menu at a specified location.
    /// </summary>
    /// <param name="screenLocation">
    ///   The location to display the system menu, in logical screen coordinates.
    /// </param>
    public void ShowSystemMenu(Point screenLocation)
    {
      var physicalScreenLocation = DpiHelper.LogicalPixelsToDevice(screenLocation);
      _ShowSystemMenu(physicalScreenLocation);
    }

    private void _ShowSystemMenu(Point physicalScreenLocation)
    {
      const uint TPM_RETURNCMD = 0x0100;
      const uint TPM_LEFTBUTTON = 0x0;

      // Use whether we can get an HWND to determine if the Window has been loaded.
      _hwnd = new WindowInteropHelper(this).Handle;

      var hmenu = NativeMethods.GetSystemMenu(_hwnd, false);

      var cmd = NativeMethods.TrackPopupMenuEx(hmenu,
                                               TPM_LEFTBUTTON | TPM_RETURNCMD,
                                               (int) physicalScreenLocation.X,
                                               (int) physicalScreenLocation.Y,
                                               _hwnd,
                                               IntPtr.Zero);
      if (0 != cmd)
      {
        NativeMethods.PostMessage(_hwnd, WM.SYSCOMMAND, new IntPtr(cmd), IntPtr.Zero);
      }
    }

    #endregion

    /// <summary>
    ///   Underlying HWND for the _window.
    /// </summary>
    private IntPtr _hwnd;

    private WINDOWPLACEMENT _normalWindowPlacement;

    //private string WindowName { get; set; }
    private bool BRestoreFromXml { get; set; }

    /// <summary>
    ///   Gets a value indicating whether the control is in design mode (running under Blend
    ///   or Visual Studio).
    /// </summary>
    [SuppressMessage(
      "Microsoft.Performance",
      "CA1822:MarkMembersAsStatic",
      Justification = "Non static member needed for data binding")]
    public bool IsInDesignMode => IsInDesignModeStatic;

    /// <summary>
    ///   Gets a value indicating whether the control is in design mode (running in Blend
    ///   or Visual Studio).
    /// </summary>
    [SuppressMessage(
      "Microsoft.Security",
      "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands",
      Justification = "The security risk here is neglectible.")]
    public static bool IsInDesignModeStatic
    {
      get
      {
        if (_isInDesignMode.HasValue) return _isInDesignMode.Value;
        var prop = DesignerProperties.IsInDesignModeProperty;
        _isInDesignMode
          = (bool) DependencyPropertyDescriptor
            .FromProperty(prop,
              typeof (FrameworkElement))
            .Metadata.DefaultValue;

        // Just to be sure
        if (!_isInDesignMode.Value
            && Process.GetCurrentProcess().ProcessName.StartsWith("devenv",
              StringComparison.Ordinal))
        {
          _isInDesignMode = true;
        }

        return _isInDesignMode.Value;
      }
    }

    //private object _currentContent = null;

    protected override void OnStateChanged(EventArgs e)
    {
      var hwnd = Helpers.GetWindowHandle(this).Handle;
      var wp = NativeMethods.GetWindowPlacement(hwnd);

      switch (WindowState)
      {
        case WindowState.Maximized:
          _normalWindowPlacement = wp;
          break;
        case WindowState.Normal:
          var bMustReposition = false;

          var r = wp.rcNormalPosition;
          if (r.Left < 0)
          {
            r.Left = 0;
            bMustReposition = true;
          }
          if (r.Top < 0)
          {
            r.Top = 0;
            bMustReposition = true;
          }

          if (bMustReposition)
          {
            if (_normalWindowPlacement != null)
              NativeMethods.SetWindowPos(hwnd,
                                         (IntPtr) 0,
                                         r.Left,
                                         r.Top,
                                         _normalWindowPlacement.rcNormalPosition.Width,
                                         _normalWindowPlacement.rcNormalPosition.Height,
                                         SWP.SHOWWINDOW);
            ThreadHelper.DoEvents();
          }
          break;
      }
      base.OnStateChanged(e);
    }


    private void BWindowBase_Loaded(object sender, RoutedEventArgs e)
    {
    }

    protected override void OnClosing(CancelEventArgs e)
    {
      Opacity = 0;
      BisClosing = true;
      base.OnClosing(e);
    }

    #region DependencyProperties

    #region CaptionBackgroundForAeroModeForActiveWindow

    /// <summary>
    ///   CaptionBackgroundForAeroModeForActiveWindow Dependency Property
    /// </summary>
    public static readonly DependencyProperty CaptionBackgroundForAeroModeForActiveWindowProperty =
      DependencyProperty.Register("CaptionBackgroundForAeroModeForActiveWindow",
                                  typeof (Brush),
                                  typeof (BWindowBase),
                                  new FrameworkPropertyMetadata(Brushes.Transparent,
                                                                FrameworkPropertyMetadataOptions.None,
                                                                OnCaptionBackgroundForAeroModeForActiveWindowChanged));

    /// <summary>
    ///   Gets or sets the CaptionBackgroundForAeroModeForActiveWindow property.  This dependency property
    ///   indicates ....
    /// </summary>
    public Brush CaptionBackgroundForAeroModeForActiveWindow
    {
      get { return (Brush) GetValue(CaptionBackgroundForAeroModeForActiveWindowProperty); }
      set { SetValue(CaptionBackgroundForAeroModeForActiveWindowProperty, value); }
    }

    /// <summary>
    ///   Handles changes to the CaptionBackgroundForAeroModeForActiveWindow property.
    /// </summary>
    private static void OnCaptionBackgroundForAeroModeForActiveWindowChanged(DependencyObject d,
                                                                             DependencyPropertyChangedEventArgs e)
    {
      ((BWindowBase) d).OnCaptionBackgroundForAeroModeForActiveWindowChanged(e);
    }

    /// <summary>
    ///   Provides derived classes an opportunity to handle changes to the CaptionBackgroundForAeroModeForActiveWindow property.
    /// </summary>
    protected virtual void OnCaptionBackgroundForAeroModeForActiveWindowChanged(DependencyPropertyChangedEventArgs e)
    {
    }

    #endregion

    #region CaptionBackgroundForAeroModeForInactiveWindow

    /// <summary>
    ///   CaptionBackgroundForAeroModeForInactiveWindow Dependency Property
    /// </summary>
    public static readonly DependencyProperty CaptionBackgroundForAeroModeForInactiveWindowProperty =
      DependencyProperty.Register("CaptionBackgroundForAeroModeForInactiveWindow",
                                  typeof (Brush),
                                  typeof (BWindowBase),
                                  new FrameworkPropertyMetadata(Brushes.Transparent,
                                                                FrameworkPropertyMetadataOptions.None,
                                                                OnCaptionBackgroundForAeroModeForInactiveWindowChanged));

    /// <summary>
    ///   Gets or sets the CaptionBackgroundForAeroModeForInactiveWindow property.  This dependency property
    ///   indicates ....
    /// </summary>
    public Brush CaptionBackgroundForAeroModeForInactiveWindow
    {
      get { return (Brush) GetValue(CaptionBackgroundForAeroModeForInactiveWindowProperty); }
      set { SetValue(CaptionBackgroundForAeroModeForInactiveWindowProperty, value); }
    }

    /// <summary>
    ///   Handles changes to the CaptionBackgroundForAeroModeForInactiveWindow property.
    /// </summary>
    private static void OnCaptionBackgroundForAeroModeForInactiveWindowChanged(DependencyObject d,
                                                                               DependencyPropertyChangedEventArgs e)
    {
      ((BWindowBase) d).OnCaptionBackgroundForAeroModeForInactiveWindowChanged(e);
    }

    /// <summary>
    ///   Provides derived classes an opportunity to handle changes to the CaptionBackgroundForAeroModeForInactiveWindow property.
    /// </summary>
    protected virtual void OnCaptionBackgroundForAeroModeForInactiveWindowChanged(DependencyPropertyChangedEventArgs e)
    {
    }

    #endregion

    #region CaptionBackgroundForNonAeroModeForInactiveWindow

    /// <summary>
    ///   CaptionBackgroundForNonAeroModeForInactiveWindow Dependency Property
    /// </summary>
    public static readonly DependencyProperty CaptionBackgroundForNonAeroModeForInactiveWindowProperty =
      DependencyProperty.Register("CaptionBackgroundForNonAeroModeForInactiveWindow",
                                  typeof (Brush),
                                  typeof (BWindowBase),
                                  new FrameworkPropertyMetadata(SystemColors.ActiveCaptionBrush,
                                                                FrameworkPropertyMetadataOptions.None,
                                                                OnCaptionBackgroundForNonAeroModeForInactiveWindowChanged));

    /// <summary>
    ///   Gets or sets the CaptionBackgroundForNonAeroModeForInactiveWindow property.  This dependency property
    ///   indicates ....
    /// </summary>
    public Brush CaptionBackgroundForNonAeroModeForInactiveWindow
    {
      get { return (Brush) GetValue(CaptionBackgroundForNonAeroModeForInactiveWindowProperty); }
      set { SetValue(CaptionBackgroundForNonAeroModeForInactiveWindowProperty, value); }
    }

    /// <summary>
    ///   Handles changes to the CaptionBackgroundForNonAeroModeForInactiveWindow property.
    /// </summary>
    private static void OnCaptionBackgroundForNonAeroModeForInactiveWindowChanged(DependencyObject d,
                                                                                  DependencyPropertyChangedEventArgs e)
    {
      ((BWindowBase) d).OnCaptionBackgroundForNonAeroModeForInactiveWindowChanged(e);
    }

    /// <summary>
    ///   Provides derived classes an opportunity to handle changes to the CaptionBackgroundForNonAeroModeForInactiveWindow property.
    /// </summary>
    protected virtual void OnCaptionBackgroundForNonAeroModeForInactiveWindowChanged(
      DependencyPropertyChangedEventArgs e)
    {
    }

    #endregion

    #region CaptionBackgroundForNonAeroModeForActiveWindow

    /// <summary>
    ///   CaptionBackgroundForNonAeroModeForActiveWindow Dependency Property
    /// </summary>
    public static readonly DependencyProperty CaptionBackgroundForNonAeroModeForActiveWindowProperty =
      DependencyProperty.Register("CaptionBackgroundForNonAeroModeForActiveWindow",
                                  typeof (Brush),
                                  typeof (BWindowBase),
                                  new FrameworkPropertyMetadata(SystemColors.InactiveCaptionBrush,
                                                                FrameworkPropertyMetadataOptions.None,
                                                                OnCaptionBackgroundForNonAeroModeForActiveWindowChanged));

    /// <summary>
    ///   Gets or sets the CaptionBackgroundForNonAeroModeForActiveWindow property.  This dependency property
    ///   indicates ....
    /// </summary>
    public Brush CaptionBackgroundForNonAeroModeForActiveWindow
    {
      get { return (Brush) GetValue(CaptionBackgroundForNonAeroModeForActiveWindowProperty); }
      set { SetValue(CaptionBackgroundForNonAeroModeForActiveWindowProperty, value); }
    }

    /// <summary>
    ///   Handles changes to the CaptionBackgroundForNonAeroModeForActiveWindow property.
    /// </summary>
    private static void OnCaptionBackgroundForNonAeroModeForActiveWindowChanged(DependencyObject d,
                                                                                DependencyPropertyChangedEventArgs e)
    {
      ((BWindowBase) d).OnCaptionBackgroundForNonAeroModeForActiveWindowChanged(e);
    }

    /// <summary>
    ///   Provides derived classes an opportunity to handle changes to the CaptionBackgroundForNonAeroModeForActiveWindow property.
    /// </summary>
    protected virtual void OnCaptionBackgroundForNonAeroModeForActiveWindowChanged(DependencyPropertyChangedEventArgs e)
    {
    }

    #endregion

    #region TitleForegroundForAeroModeForActiveWindow

    /// <summary>
    ///   TitleForegroundForAeroModeForActiveWindow Dependency Property
    /// </summary>
    public static readonly DependencyProperty TitleForegroundForAeroModeForActiveWindowProperty =
      DependencyProperty.Register("TitleForegroundForAeroModeForActiveWindow",
                                  typeof (Brush),
                                  typeof (BWindowBase),
                                  new FrameworkPropertyMetadata(SystemColors.InactiveCaptionTextBrush,
                                                                FrameworkPropertyMetadataOptions.None,
                                                                OnTitleForegroundForAeroModeForActiveWindowChanged));

    /// <summary>
    ///   Gets or sets the TitleForegroundForAeroModeForActiveWindow property.  This dependency property
    ///   indicates ....
    /// </summary>
    public Brush TitleForegroundForAeroModeForActiveWindow
    {
      get { return (Brush) GetValue(TitleForegroundForAeroModeForActiveWindowProperty); }
      set { SetValue(TitleForegroundForAeroModeForActiveWindowProperty, value); }
    }

    /// <summary>
    ///   Handles changes to the TitleForegroundForAeroModeForActiveWindow property.
    /// </summary>
    private static void OnTitleForegroundForAeroModeForActiveWindowChanged(DependencyObject d,
                                                                           DependencyPropertyChangedEventArgs e)
    {
      ((BWindowBase) d).OnTitleForegroundForAeroModeForActiveWindowChanged(e);
    }

    /// <summary>
    ///   Provides derived classes an opportunity to handle changes to the TitleForegroundForAeroModeForActiveWindow property.
    /// </summary>
    protected virtual void OnTitleForegroundForAeroModeForActiveWindowChanged(DependencyPropertyChangedEventArgs e)
    {
    }

    #endregion

    #region TitleForegroundForAeroModeForInactiveWindow

    /// <summary>
    ///   TitleForegroundForAeroModeForInactiveWindow Dependency Property
    /// </summary>
    public static readonly DependencyProperty TitleForegroundForAeroModeForInactiveWindowProperty =
      DependencyProperty.Register("TitleForegroundForAeroModeForInactiveWindow",
                                  typeof (Brush),
                                  typeof (BWindowBase),
                                  new FrameworkPropertyMetadata(SystemColors.ActiveCaptionTextBrush,
                                                                FrameworkPropertyMetadataOptions.None,
                                                                OnTitleForegroundForAeroModeForInactiveWindowChanged));

    /// <summary>
    ///   Gets or sets the TitleForegroundForAeroModeForInactiveWindow property.  This dependency property
    ///   indicates ....
    /// </summary>
    public Brush TitleForegroundForAeroModeForInactiveWindow
    {
      get { return (Brush) GetValue(TitleForegroundForAeroModeForInactiveWindowProperty); }
      set { SetValue(TitleForegroundForAeroModeForInactiveWindowProperty, value); }
    }

    /// <summary>
    ///   Handles changes to the TitleForegroundForAeroModeForInactiveWindow property.
    /// </summary>
    private static void OnTitleForegroundForAeroModeForInactiveWindowChanged(DependencyObject d,
                                                                             DependencyPropertyChangedEventArgs e)
    {
      ((BWindowBase) d).OnTitleForegroundForAeroModeForInactiveWindowChanged(e);
    }

    /// <summary>
    ///   Provides derived classes an opportunity to handle changes to the TitleForegroundForAeroModeForInactiveWindow property.
    /// </summary>
    protected virtual void OnTitleForegroundForAeroModeForInactiveWindowChanged(DependencyPropertyChangedEventArgs e)
    {
    }

    #endregion

    #region TitleForegroundForNonAeroModeForActiveWindow

    /// <summary>
    ///   TitleForegroundForNonAeroModeForActiveWindow Dependency Property
    /// </summary>
    public static readonly DependencyProperty TitleForegroundForNonAeroModeForActiveWindowProperty =
      DependencyProperty.Register("TitleForegroundForNonAeroModeForActiveWindow",
                                  typeof (Brush),
                                  typeof (BWindowBase),
                                  new FrameworkPropertyMetadata(SystemColors.ActiveCaptionTextBrush,
                                                                FrameworkPropertyMetadataOptions.None,
                                                                OnTitleForegroundForNonAeroModeForActiveWindowChanged));

    /// <summary>
    ///   Gets or sets the TitleForegroundForNonAeroModeForActiveWindow property.  This dependency property
    ///   indicates ....
    /// </summary>
    public Brush TitleForegroundForNonAeroModeForActiveWindow
    {
      get { return (Brush) GetValue(TitleForegroundForNonAeroModeForActiveWindowProperty); }
      set { SetValue(TitleForegroundForNonAeroModeForActiveWindowProperty, value); }
    }

    /// <summary>
    ///   Handles changes to the TitleForegroundForNonAeroModeForActiveWindow property.
    /// </summary>
    private static void OnTitleForegroundForNonAeroModeForActiveWindowChanged(DependencyObject d,
                                                                              DependencyPropertyChangedEventArgs e)
    {
      ((BWindowBase) d).OnTitleForegroundForNonAeroModeForActiveWindowChanged(e);
    }

    /// <summary>
    ///   Provides derived classes an opportunity to handle changes to the TitleForegroundForNonAeroModeForActiveWindow property.
    /// </summary>
    protected virtual void OnTitleForegroundForNonAeroModeForActiveWindowChanged(DependencyPropertyChangedEventArgs e)
    {
    }

    #endregion

    #region TitleForegroundForNonAeroModeForInactiveWindow

    /// <summary>
    ///   TitleForegroundForNonAeroModeForInactiveWindow Dependency Property
    /// </summary>
    public static readonly DependencyProperty TitleForegroundForNonAeroModeForInactiveWindowProperty =
      DependencyProperty.Register("TitleForegroundForNonAeroModeForInactiveWindow",
                                  typeof (Brush),
                                  typeof (BWindowBase),
                                  new FrameworkPropertyMetadata(SystemColors.InactiveCaptionTextBrush,
                                                                FrameworkPropertyMetadataOptions.None,
                                                                OnTitleForegroundForNonAeroModeForInactiveWindowChanged));

    /// <summary>
    ///   Gets or sets the TitleForegroundForNonAeroModeForInactiveWindow property.  This dependency property
    ///   indicates ....
    /// </summary>
    public Brush TitleForegroundForNonAeroModeForInactiveWindow
    {
      get { return (Brush) GetValue(TitleForegroundForNonAeroModeForInactiveWindowProperty); }
      set { SetValue(TitleForegroundForNonAeroModeForInactiveWindowProperty, value); }
    }

    /// <summary>
    ///   Handles changes to the TitleForegroundForNonAeroModeForInactiveWindow property.
    /// </summary>
    private static void OnTitleForegroundForNonAeroModeForInactiveWindowChanged(DependencyObject d,
                                                                                DependencyPropertyChangedEventArgs e)
    {
      ((BWindowBase) d).OnTitleForegroundForNonAeroModeForInactiveWindowChanged(e);
    }

    /// <summary>
    ///   Provides derived classes an opportunity to handle changes to the TitleForegroundForNonAeroModeForInactiveWindow property.
    /// </summary>
    protected virtual void OnTitleForegroundForNonAeroModeForInactiveWindowChanged(DependencyPropertyChangedEventArgs e)
    {
    }

    #endregion

    #region BackgroundForAeroModeForActiveWindow

    /// <summary>
    ///   BackgroundForAeroModeForActiveWindow Dependency Property
    /// </summary>
    public static readonly DependencyProperty BackgroundForAeroModeForActiveWindowProperty =
      DependencyProperty.Register("BackgroundForAeroModeForActiveWindow",
                                  typeof (Brush),
                                  typeof (BWindowBase),
                                  new FrameworkPropertyMetadata(Brushes.Transparent,
                                                                FrameworkPropertyMetadataOptions.None,
                                                                OnBackgroundForAeroModeForActiveWindowChanged));

    /// <summary>
    ///   Gets or sets the BackgroundForAeroModeForActiveWindow property.  This dependency property
    ///   indicates ....
    /// </summary>
    public Brush BackgroundForAeroModeForActiveWindow
    {
      get { return (Brush) GetValue(BackgroundForAeroModeForActiveWindowProperty); }
      set { SetValue(BackgroundForAeroModeForActiveWindowProperty, value); }
    }

    /// <summary>
    ///   Handles changes to the BackgroundForAeroModeForActiveWindow property.
    /// </summary>
    private static void OnBackgroundForAeroModeForActiveWindowChanged(DependencyObject d,
                                                                      DependencyPropertyChangedEventArgs e)
    {
      ((BWindowBase) d).OnBackgroundForAeroModeForActiveWindowChanged(e);
    }

    /// <summary>
    ///   Provides derived classes an opportunity to handle changes to the BackgroundForAeroModeForActiveWindow property.
    /// </summary>
    protected virtual void OnBackgroundForAeroModeForActiveWindowChanged(DependencyPropertyChangedEventArgs e)
    {
      //
    }

    #endregion

    #region BackgroundForNonAeroModeForActiveWindow

    /// <summary>
    ///   BackgroundForNonAeroModeForActiveWindow Dependency Property
    /// </summary>
    public static readonly DependencyProperty BackgroundForNonAeroModeForActiveWindowProperty =
      DependencyProperty.Register("BackgroundForNonAeroModeForActiveWindow",
                                  typeof (Brush),
                                  typeof (BWindowBase),
                                  new FrameworkPropertyMetadata(SystemColors.WindowBrush,
                                                                FrameworkPropertyMetadataOptions.None,
                                                                OnBackgroundForNonAeroModeForActiveWindowChanged));


    /// <summary>
    ///   Gets or sets the BackgroundForNonAeroModeForActiveWindow property.  This dependency property
    ///   indicates ....
    /// </summary>
    public Brush BackgroundForNonAeroModeForActiveWindow
    {
      get { return (Brush) GetValue(BackgroundForNonAeroModeForActiveWindowProperty); }
      set { SetValue(BackgroundForNonAeroModeForActiveWindowProperty, value); }
    }

    /// <summary>
    ///   Handles changes to the BackgroundForNonAeroModeForActiveWindow property.
    /// </summary>
    private static void OnBackgroundForNonAeroModeForActiveWindowChanged(DependencyObject d,
                                                                         DependencyPropertyChangedEventArgs e)
    {
      ((BWindowBase) d).OnBackgroundForNonAeroModeForActiveWindowChanged(e);
    }

    /// <summary>
    ///   Provides derived classes an opportunity to handle changes to the BackgroundForNonAeroModeForActiveWindow property.
    /// </summary>
    protected virtual void OnBackgroundForNonAeroModeForActiveWindowChanged(DependencyPropertyChangedEventArgs e)
    {
    }

    #endregion

    #endregion
  }


  public class WindowLocation
  {
    public WindowLocation()
    {
    }

    public WindowLocation(string name)
    {
      Name = name;
    }

    public WindowLocation(string name, double leftPosition, double topPosition)
    {
      Name = name;
      Left = leftPosition;
      Top = topPosition;
    }

    public WindowLocation(string name, double leftPosition, double topPosition, double widthSize, double heightSize)
    {
      Name = name;
      Left = leftPosition;
      Top = topPosition;
      Width = widthSize;
      Height = heightSize;
    }

    public string Name { get; set; }
    public double Left { get; set; }
    public double Top { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
  }
}