#region

using System;
using System.ComponentModel;
using System.Configuration;
using System.Windows;
using System.Windows.Threading;
using BUtility;

#endregion

namespace Sobees.Windows
{
  /// <summary>
  ///   Persists a Window's Size, Location and WindowState to UserScopeSettings
  /// </summary>
  public class WindowSettings
  {
    #region WindowApplicationSettings Helper Class

    private const string APPNAME = "WindowApplicationSettings";

    public class WindowApplicationSettings : ApplicationSettingsBase
    {
      private WindowSettings _windowSettings;

      public WindowApplicationSettings(WindowSettings windowSettings)
        : base(windowSettings.window.Name)
      {
        _windowSettings = windowSettings;
      }

      [UserScopedSetting]
      public Rect Location
      {
        get
        {
          if (this["Location"] != null)
          {
            return ((Rect) this["Location"]);
          }
          return Rect.Empty;
        }
        set { this["Location"] = value; }
      }

      [UserScopedSetting]
      public WindowState WindowState
      {
        get
        {
          if (this["WindowState"] != null)
          {
            return (WindowState) this["WindowState"];
          }
          return WindowState.Normal;
        }
        set { this["WindowState"] = value; }
      }
    }

    #endregion

    #region Constructor

    private readonly Window window;

    public WindowSettings(Window window)
    {
      this.window = window;
    }

    #endregion

    #region Attached "Save" Property Implementation

    /// <summary>
    ///   Register the "Save" attached property and the "OnSaveInvalidated" callback
    /// </summary>
    public static readonly DependencyProperty SaveProperty
      = DependencyProperty.RegisterAttached("Save",
                                            typeof (bool),
                                            typeof (WindowSettings),
                                            new FrameworkPropertyMetadata(OnSaveInvalidated));

    public static void SetSave(DependencyObject dependencyObject, bool enabled)
    {
      dependencyObject.SetValue(SaveProperty, enabled);
    }

    /// <summary>
    ///   Called when Save is changed on an object.
    /// </summary>
    private static void OnSaveInvalidated(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
    {
      var window = dependencyObject as Window;
      if (window != null)
      {
        if (!((bool) e.NewValue)) return;
        var settings = new WindowSettings(window);
        settings.Attach();
      }
    }

    #endregion

    #region Protected Methods

    /// <summary>
    ///   Load the Window Size Location and State from the settings object
    /// </summary>
    protected virtual void LoadWindowState()
    {
      Settings.Reload();
      if (Settings.Location != Rect.Empty)
      {
        window.Left = Settings.Location.Left;
        window.Top = Settings.Location.Top;
        window.Width = Settings.Location.Width;
        window.Height = Settings.Location.Height;
      }

      if (Settings.WindowState != WindowState.Maximized)
      {
        window.WindowState = Settings.WindowState;
      }
    }


    /// <summary>
    ///   Save the Window Size, Location and State to the settings object
    /// </summary>
    protected virtual void SaveWindowState()
    {
      BLogManager.LogEntry(APPNAME, "SaveWindowState", "START", true);
      Settings.WindowState = window.WindowState;
      Settings.Location = window.RestoreBounds;
      Settings.Save();
      BLogManager.LogEntry(APPNAME, "SaveWindowState", "END", true);
    }

    #endregion

    #region Private Methods

    private void Attach()
    {
      if (window != null)
      {
        window.Closing += WindowClosing;
        window.Initialized += WindowInitialized;
        window.Loaded += WindowLoaded;
      }
    }

    private void WindowLoaded(object sender, RoutedEventArgs e)
    {
      if (Settings.WindowState == WindowState.Maximized)
      {
        window.WindowState = Settings.WindowState;
      }
    }

    private void WindowInitialized(object sender, EventArgs e)
    {
      var dispatcher = Dispatcher.CurrentDispatcher;
      Action mainAction = LoadWindowState;
      dispatcher.Invoke(mainAction);
    }

    private void WindowClosing(object sender, CancelEventArgs e)
    {
      var dispatcher = Dispatcher.CurrentDispatcher;
      Action mainAction = SaveWindowState;
      dispatcher.Invoke(mainAction);
    }

    #endregion

    #region Settings Property Implementation

    private WindowApplicationSettings _windowApplicationSettings;

    [Browsable(false)]
    public WindowApplicationSettings Settings => _windowApplicationSettings ?? (_windowApplicationSettings = CreateWindowApplicationSettingsInstance());

    protected virtual WindowApplicationSettings CreateWindowApplicationSettingsInstance()
    {
      return new WindowApplicationSettings(this);
    }

    #endregion
  }
}