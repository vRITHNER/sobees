#region

using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using Sobees.Glass.Native;
using Sobees.Library.BUtilities;
using Sobees.Tools.Logging;
using Sobees.Tools.Threading;
using Sobees.Windows;
using MINMAXINFO = Sobees.Glass.Native.MINMAXINFO;
using MONITORINFO = Sobees.Glass.Native.MONITORINFO;
using Rect = System.Windows.Rect;
using ResizeDirection = Sobees.Glass.Native.ResizeDirection;
using WM = Sobees.Glass.Native.WM;

#endregion

namespace Sobees.Glass
{
  public class GlassWindow : BWindowBase
  {
    private static readonly DependencyPropertyKey IsWindowActivePropertyKey =
      DependencyProperty.RegisterReadOnly("IsWindowActive", typeof (bool), typeof (GlassWindow),
                                          new FrameworkPropertyMetadata(false,
                                                                        FrameworkPropertyMetadataOptions.AffectsRender,
                                                                        WindowActivePropertyChanged));

    public static readonly DependencyProperty IsWindowActiveProperty = IsWindowActivePropertyKey.DependencyProperty;

    private static readonly DependencyPropertyKey WindowTitleBrushPropertyKey =
      DependencyProperty.RegisterReadOnly("WindowTitleBrush", typeof (Brush), typeof (GlassWindow), new UIPropertyMetadata(null));

    public static DependencyProperty WindowTitleBrushProperty = WindowTitleBrushPropertyKey.DependencyProperty;

    public static readonly DependencyProperty ActiveTitleBrushProperty =
      DependencyProperty.Register("ActiveTitleBrush", typeof (Brush), typeof (GlassWindow), new UIPropertyMetadata(null));

    public static readonly DependencyProperty InactiveTitleBrushProperty =
      DependencyProperty.Register("InactiveTitleBrush", typeof (Brush), typeof (GlassWindow), new UIPropertyMetadata(null));

    private bool IsSourceInitialized;
    private Border _contentWindowBorder;

    #region BisXPMode

    /// <summary>
    ///   BisXPMode Dependency Property
    /// </summary>
    public static readonly DependencyProperty BisXPModeProperty =
      DependencyProperty.Register("BisXPMode", typeof (bool), typeof (GlassWindow),
                                  new FrameworkPropertyMetadata(false,
                                                                FrameworkPropertyMetadataOptions.None,
                                                                OnBisXPModeChanged));

    /// <summary>
    ///   Gets or sets the BisXPMode property.  This dependency property
    ///   indicates ....
    /// </summary>
    public bool BisXPMode
    {
      get { return (bool) GetValue(BisXPModeProperty); }
      set { SetValue(BisXPModeProperty, value); }
    }

    /// <summary>
    ///   Handles changes to the BisXPMode property.
    /// </summary>
    private static void OnBisXPModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((GlassWindow) d).OnBisXPModeChanged(e);
    }

    /// <summary>
    ///   Provides derived classes an opportunity to handle changes to the BisXPMode property.
    /// </summary>
    protected virtual void OnBisXPModeChanged(DependencyPropertyChangedEventArgs e)
    {
      //
    }

    #endregion

    #region Content Property

    public new static DependencyProperty ContentProperty;

    /// <summary>
    ///   Window content
    ///   <remarks>
    ///     Hides base window class 'Content' property
    ///   </remarks>
    /// </summary>
    public new object Content
    {
      get { return GetValue(ContentProperty); }
      set
      {
        SetValue(ContentProperty,
                 value);
      }
    }

    #endregion

    static GlassWindow()
    {
      if (Assembly.GetEntryAssembly() != null)
      {
        ContentProperty = DependencyProperty.Register("Content",
                                                      typeof (object),
                                                      typeof (GlassWindow),
                                                      new UIPropertyMetadata(null, ContentChangedCallback));
      }

      DefaultStyleKeyProperty.OverrideMetadata(typeof (GlassWindow), new FrameworkPropertyMetadata(typeof (GlassWindow)));
    }

    public GlassWindow()
    {
      //InitializeContentControls();
      Loaded += GlassWindow_Loaded;
    }

    /// <summary>
    ///   Gets whether the window is active.
    ///   This is a dependency property.
    /// </summary>
    public bool IsWindowActive
    {
      get { return (bool) GetValue(IsWindowActiveProperty); }
      private set { SetValue(IsWindowActivePropertyKey, value); }
    }

    /// <summary>
    ///   Gets the Brush for the window title.
    ///   This is a dependency property.
    /// </summary>
    public Brush WindowTitleBrush
    {
      get { return (Brush) GetValue(WindowTitleBrushProperty); }
      private set { SetValue(WindowTitleBrushPropertyKey, value); }
    }

    public Brush ActiveTitleBrush
    {
      get { return (Brush) GetValue(ActiveTitleBrushProperty); }
      set { SetValue(ActiveTitleBrushProperty, value); }
    }

    public Brush InactiveTitleBrush
    {
      get { return (Brush) GetValue(InactiveTitleBrushProperty); }
      set { SetValue(InactiveTitleBrushProperty, value); }
    }


    private static void ContentChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var window = (GlassWindow) d;
      if (window._contentWindowBorder != null)
        window._contentWindowBorder.Child = (UIElement) e.NewValue;
    }

    protected override void OnSourceInitialized(EventArgs e)
    {
      base.OnSourceInitialized(e);
      var handle = new WindowInteropHelper(this).Handle;
      HwndSource.FromHwnd(handle).AddHook(WindowProc);
      DwmApi.SetGlassMargin(this, FrameThickness);
      IsSourceInitialized = true;
    }

    protected override Size ArrangeOverride(Size arrangeBounds)
    {
      var child = VisualChildrenCount > 0 ? GetVisualChild(0) as UIElement : null;
      if (child != null)
      {
        child.Arrange(new Rect(arrangeBounds));
      }
      return arrangeBounds;
    }

    private static void WindowActivePropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
      var w = (GlassWindow) o;
      w.SetWindowTitleBrush();
    }


    private void SetWindowTitleBrush()
    {
      if (DwmApi.DwmEnabled)
      {
        WindowTitleBrush = IsWindowActive ? SystemColors.ActiveCaptionTextBrush : SystemColors.InactiveCaptionTextBrush;
      }
      else
      {
        WindowTitleBrush = IsWindowActive ? ActiveTitleBrush : InactiveTitleBrush;
      }
    }


    private IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
      try
      {
        switch (DwmApi.DwmEnabled)
        {
          case true:
            handled = true;
            switch ((WM) msg)
            {
              case WM.NCCALCSIZE:
                handled = true;
                return IntPtr.Zero;
              case WM.NCACTIVATE:
                IsWindowActive = wParam.ToInt32() == 1;
                handled = true;
                return User32.DefWindowProc(hwnd, msg, wParam, new IntPtr(-1));
              case WM.NCHITTEST:
                var result = IntPtr.Zero;
                handled = DwmApi.DwmDefWindowProc(hwnd, msg, wParam, lParam, ref result);

                return result;
              case WM.DWMCOMPOSITIONCHANGED:
                IsCompositionEnabled = DwmApi.DwmEnabled;
                if (IsCompositionEnabled)
                  DwmApi.SetGlassMargin(this, FrameThickness);
                break;
              case WM.GETMINMAXINFO:
                WM_GETMINMAXINFO(hwnd, lParam, this);
                handled = true;
                break;
              case WM.SETICON:
                handled = true;
                return IntPtr.Zero;
              case WM.SETTEXT:
                handled = true;
                InvalidateArrange();
                break;
              case WM.CLOSE:
                handled = false;
                break;
              default:
                handled = false;
                break;
            }
            break;
          case false:
            switch ((WM) msg)
            {
              case WM.NCCALCSIZE:
                handled = true;
                return IntPtr.Zero;
              case WM.DWMCOMPOSITIONCHANGED:
                IsCompositionEnabled = DwmApi.DwmEnabled;
                if (IsCompositionEnabled)
                  DwmApi.SetGlassMargin(this, FrameThickness);
                break;
              case WM.GETMINMAXINFO:
                WM_GETMINMAXINFO(hwnd, lParam, this);
                handled = true;
                break;
            }
            break;
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this,
                          (ex));
      }
      return IntPtr.Zero;
    }

    [DllImport("user32")]
    internal static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);

    private static void WM_GETMINMAXINFO(IntPtr Handle, IntPtr lParam, GlassWindow Window)
    {
      var mmi = (MINMAXINFO) Marshal.PtrToStructure(lParam, typeof (MINMAXINFO));

      // Adjust the maximized size and position to fit the work area of the correct monitor
      var MONITOR_DEFAULTTONEAREST = 0x00000002;
      var monitor = NativeMethods.MonitorFromWindow(Handle, MONITOR_DEFAULTTONEAREST);

      if (monitor != IntPtr.Zero)
      {
        var monitorInfo = new MONITORINFO();
        GetMonitorInfo(monitor, monitorInfo);
        var rcWorkArea = monitorInfo.rcWork;
        var rcMonitorArea = monitorInfo.rcMonitor;
        if (DwmApi.DwmEnabled)
        {
          //Under Vista and Win7: taskbar doesn't count and Left or Top = 0 always
          mmi.ptMaxPosition.X = 0;
          mmi.ptMaxPosition.Y = 0;
        }
        else
        {
          //In XP, we need to calculate the Left or TOP point 
          mmi.ptMaxPosition.X = Math.Abs(rcWorkArea.left - rcMonitorArea.left);
          mmi.ptMaxPosition.Y = Math.Abs(rcWorkArea.top - rcMonitorArea.top);
        }

        mmi.ptMaxSize.X = Math.Abs(rcWorkArea.right - rcWorkArea.left);
        mmi.ptMaxSize.Y = Math.Abs(rcWorkArea.bottom - rcWorkArea.top);
      }

      Marshal.StructureToPtr(mmi, lParam, true);
    }

    protected void SetFrameThickness(FrameworkElement Element)
    {
      if (Element == null) return;
      ThreadHelper.DoEvents();
      UpdateLayout();
      if (!IsAncestorOf(Element)) return;
      var trans =
        Element.TransformToAncestor(this).TransformBounds(new Rect(0,
                                                                   0,
                                                                   (int) Element.ActualWidth,
                                                                   (int) Element.ActualHeight));
      var Margin = new Thickness(trans.Left, trans.Top, ActualWidth - trans.Right, ActualHeight - trans.Bottom);
      FrameThickness = Margin;
    }

    #region Dependency Properties

    public static readonly DependencyPropertyKey IsCompositionEnabledPropertyKey =
      DependencyProperty.RegisterReadOnly("IsCompositionEnabled",
                                          typeof (bool),
                                          typeof (GlassWindow),
                                          new UIPropertyMetadata(DwmApi.DwmEnabled));

    public static readonly DependencyProperty IsCompositionEnabledProperty =
      IsCompositionEnabledPropertyKey.DependencyProperty;

    public static readonly DependencyProperty FrameThicknessProperty =
      DependencyProperty.Register("FrameThickness",
                                  typeof (Thickness?),
                                  typeof (GlassWindow),
                                  new UIPropertyMetadata(new Thickness(
                                                           SystemParameters.ResizeFrameVerticalBorderWidth,
                                                           SystemParameters.ResizeFrameHorizontalBorderHeight + 20,
                                                           SystemParameters.ResizeFrameVerticalBorderWidth,
                                                           SystemParameters.ResizeFrameHorizontalBorderHeight).
                                                           AsNullable(),
                                                         FrameThicknessPropertyChangedCallback));

    public static readonly DependencyProperty CaptionHeightProperty =
      DependencyProperty.Register("CaptionHeight", typeof (double), typeof (GlassWindow), new UIPropertyMetadata(20.0));

    private static readonly DependencyPropertyKey ResizeMarginPropertyKey =
      DependencyProperty.RegisterReadOnly("ResizeMargin",
                                          typeof (Thickness),
                                          typeof (GlassWindow),
                                          new UIPropertyMetadata(GetSystemResizeMargins()));

    public static readonly DependencyProperty ResizeMarginProperty = ResizeMarginPropertyKey.DependencyProperty;

    protected GlassWindow(string mainWindow, bool brestorefromXml, WindowLocation windowLocation)
      : base(mainWindow, brestorefromXml, windowLocation)
    {
      //
    }

    public bool IsCompositionEnabled
    {
      get { return (bool) GetValue(IsCompositionEnabledProperty); }
      private set { SetValue(IsCompositionEnabledPropertyKey, value); }
    }

    /// <summary>
    ///   If true, window content is extended into caption area.
    ///   Cannot be changed when window is already loaded.
    /// </summary>
    public bool ContentExtend
    {
      get { return _contentExtend; }
      set { _contentExtend = value; }
    }

    public Thickness? FrameThickness
    {
      get { return (Thickness?) GetValue(FrameThicknessProperty); }
      set { SetValue(FrameThicknessProperty, value); }
    }

    public double CaptionHeight
    {
      get { return (double) GetValue(CaptionHeightProperty); }
      set { SetValue(CaptionHeightProperty, value); }
    }

    public Thickness ResizeMargin
    {
      get { return (Thickness) GetValue(ResizeMarginProperty); }
      private set { SetValue(ResizeMarginPropertyKey, value); }
    }

    private static void FrameThicknessPropertyChangedCallback(object sender, DependencyPropertyChangedEventArgs e)
    {
      var Window = sender as GlassWindow;
      if (Window == null) return;
      if (Window.IsSourceInitialized)
        DwmApi.SetGlassMargin(Window, Window.FrameThickness);
    }

    private static Thickness GetSystemResizeMargins()
    {
      return new Thickness(
        SystemParameters.ResizeFrameVerticalBorderWidth,
        SystemParameters.ResizeFrameHorizontalBorderHeight,
        SystemParameters.ResizeFrameVerticalBorderWidth,
        SystemParameters.ResizeFrameHorizontalBorderHeight);
    }

    #endregion

    #region Resize and Drag Handlers

    private bool _canResize;


    private Border _captionControl;
    private bool _contentExtend;
    private Border _contentWindowBackgroundBorder;
    private double _lastMouseCaptionClick;
    private Border _windowdragzoneControl;

    private void GlassWindow_Loaded(object sender, RoutedEventArgs e)
    {
      if (BisXPMode) ResizeMode = ResizeMode.NoResize;
      _captionControl.Height = CaptionHeight;
      _windowdragzoneControl.Height = WindowDragZoneHeight;

      //var t = new TextBlock();
      //t.Text = Title;
      //t.Margin = new Thickness(2, 1, 0, 0);
      //_captionControl.Child = t;

      _contentWindowBorder.Margin = _contentExtend ? new Thickness(0) : new Thickness(0, CaptionHeight, 0, 0);
    }

    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();
      _canResize = ResizeMode != ResizeMode.NoResize;

      ApplyDragHandler();
    }

    private void ApplyDragHandler()
    {
      if (_canResize)
      {
        foreach (var str in "top,bottom,left,right,topleft,topright,bottomleft,bottomright".Split(new[] {','}))
        {
          var element = Template.FindName("PART_" + str, this) as FrameworkElement;
          if (element != null)
          {
            element.MouseDown += Resize;
          }
        }
      }
      var control = Template.FindName("PART_captionbar", this) as Control;
      if (control != null)
      {
        control.MouseDown += delegate { BeginDrag(); };
        if (_canResize)
          control.MouseDoubleClick += delegate { ToggleMaximize(); };
      }
      AddClickHandlerSafe("PART_MinBtn", delegate { Minimize(); });
      AddClickHandlerSafe("PART_MaxBtn", delegate { ToggleMaximize(); });
      AddClickHandlerSafe("PART_CloseBtn", delegate { Close(); });
    }

    private void AddClickHandlerSafe(string Name, RoutedEventHandler Action)
    {
      var button = Template.FindName(Name, this) as Button;
      if (button != null)
      {
        button.Click += Action;
      }
    }

    private void Resize(object sender, MouseButtonEventArgs e)
    {
      var element = sender as FrameworkElement;
      var str = element.Name.Substring(5);
      if (WindowState != WindowState.Maximized)
      {
        switch (str)
        {
          case "top":
            User32.BeginResizeWindow(this, ResizeDirection.Top);
            break;
          case "bottom":
            User32.BeginResizeWindow(this, ResizeDirection.Bottom);
            break;
          case "left":
            User32.BeginResizeWindow(this, ResizeDirection.Left);
            break;
          case "right":
            User32.BeginResizeWindow(this, ResizeDirection.Right);
            break;
          case "topleft":
            User32.BeginResizeWindow(this, ResizeDirection.TopLeft);
            break;
          case "topright":
            User32.BeginResizeWindow(this, ResizeDirection.TopRight);
            break;
          case "bottomleft":
            User32.BeginResizeWindow(this, ResizeDirection.BottomLeft);
            break;
          case "bottomright":
            User32.BeginResizeWindow(this, ResizeDirection.BottomRight);
            break;
        }
      }
    }

    private void ToggleMaximize()
    {
      WindowState = (WindowState == WindowState.Normal) ? WindowState.Maximized : WindowState.Normal;
    }

    private void Minimize()
    {
      WindowState = WindowState.Minimized;
    }

    private void BeginDrag()
    {
      if ((WindowState == WindowState.Normal) && (Mouse.LeftButton == MouseButtonState.Pressed))
        User32.BeginDragWindow(this);
    }

    #endregion
  }
}