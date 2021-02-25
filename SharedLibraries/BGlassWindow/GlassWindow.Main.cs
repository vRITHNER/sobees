using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BGlassWindow.Native;
using WPF_Aero_Window;
using WPF_Aero_Window.Native;

namespace BGlassWindow
{
  public partial class GlassWindow : System.Windows.Window
  {
    static GlassWindow()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(GlassWindow), new FrameworkPropertyMetadata(typeof(GlassWindow)));
    }

    #region Dependency Properties
    public bool IsCompositionEnabled
    {
      get { return (bool)GetValue(IsCompositionEnabledProperty); }
      private set { SetValue(IsCompositionEnabledPropertyKey, value); }
    }

    public static readonly DependencyPropertyKey IsCompositionEnabledPropertyKey =
      DependencyProperty.RegisterReadOnly("IsCompositionEnabled", typeof(bool), typeof(GlassWindow), new UIPropertyMetadata(DwmApi.DwmEnabled));
    public static readonly DependencyProperty IsCompositionEnabledProperty = IsCompositionEnabledPropertyKey.DependencyProperty;

    public Thickness? FrameThickness
    {
      get { return (Thickness?)GetValue(FrameThicknessProperty); }
      set { SetValue(FrameThicknessProperty, value); }
    }
    public static readonly DependencyProperty FrameThicknessProperty =
      DependencyProperty.Register("FrameThickness", typeof(Thickness?), typeof(GlassWindow),
                                  new UIPropertyMetadata(new Thickness(
                                                           SystemParameters.ResizeFrameVerticalBorderWidth, SystemParameters.ResizeFrameHorizontalBorderHeight + 20,
                                                           SystemParameters.ResizeFrameVerticalBorderWidth, SystemParameters.ResizeFrameHorizontalBorderHeight).AsNullable(), 
                                                         FrameThicknessPropertyChangedCallback));

    private static void FrameThicknessPropertyChangedCallback(object sender, DependencyPropertyChangedEventArgs e)
    {
      var Window = sender as GlassWindow;
      if (Window == null) return;
      if (Window.IsSourceInitialized)
        DwmApi.SetGlassMargin(Window, Window.FrameThickness);
    }

    public double CaptionHeight
    {
      get { return (double)GetValue(CaptionHeightProperty); }
      set { SetValue(CaptionHeightProperty, value); }
    }
    public static readonly DependencyProperty CaptionHeightProperty =
      DependencyProperty.Register("CaptionHeight", typeof(double), typeof(GlassWindow), new UIPropertyMetadata(20.0));

    public Thickness ResizeMargin
    {
      get { return (Thickness)GetValue(ResizeMarginProperty); }
      private set { SetValue(ResizeMarginPropertyKey, value); }
    }
    private static readonly DependencyPropertyKey ResizeMarginPropertyKey =
      DependencyProperty.RegisterReadOnly("ResizeMargin", typeof(Thickness), typeof(GlassWindow),
                                          new UIPropertyMetadata(GetSystemResizeMargins()));
    public static readonly DependencyProperty ResizeMarginProperty = ResizeMarginPropertyKey.DependencyProperty;
    private static Thickness GetSystemResizeMargins()
    {
      return new Thickness(
        SystemParameters.ResizeFrameVerticalBorderWidth, SystemParameters.ResizeFrameHorizontalBorderHeight,
        SystemParameters.ResizeFrameVerticalBorderWidth, SystemParameters.ResizeFrameHorizontalBorderHeight);
    }

    #endregion

    #region Resize and Drag Handlers

    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();
      ApplyDragHandler();
    }

    private void ApplyDragHandler()
    {
      foreach (string str in "top,bottom,left,right,topleft,topright,bottomleft,bottomright".Split(new char[] { ',' }))
      {
        FrameworkElement element = this.Template.FindName("PART_" + str, this) as FrameworkElement;
        if (element != null)
        {
          element.MouseDown += new MouseButtonEventHandler(this.Resize);
        }
      }
      Control control = this.Template.FindName("PART_captionbar", this) as Control;
      if (control != null)
      {
        control.MouseDown += delegate(object sender, MouseButtonEventArgs e) { BeginDrag(); };
        control.MouseDoubleClick += delegate(object sender, MouseButtonEventArgs e) { ToggleMaximize(); };
      }
      this.AddClickHandlerSafe("PART_MinBtn", delegate(object sender, RoutedEventArgs e) { this.Minimize(); });
      this.AddClickHandlerSafe("PART_MaxBtn", delegate(object sender, RoutedEventArgs e) { this.ToggleMaximize(); });
      this.AddClickHandlerSafe("PART_CloseBtn", delegate(object sender, RoutedEventArgs e) { this.Close(); });
    }

    private void AddClickHandlerSafe(string Name, RoutedEventHandler Action)
    {
      Button button = this.Template.FindName(Name, this) as Button;
      if (button != null)
      {
        button.Click += Action;
      }
    }

    private void Resize(object sender, MouseButtonEventArgs e)
    {
      FrameworkElement element = sender as FrameworkElement;
      string str = element.Name.Substring(5);
      if (this.WindowState != WindowState.Maximized)
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
    { WindowState = (WindowState == WindowState.Normal) ? WindowState.Maximized : WindowState.Normal; }

    private void Minimize()
    { this.WindowState = WindowState.Minimized; }

    private void BeginDrag()
    {
      if ((this.WindowState == WindowState.Normal) && (Mouse.LeftButton == MouseButtonState.Pressed))
        User32.BeginDragWindow(this);
    }

    #endregion

  }
}


