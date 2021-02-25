#region

using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Sobees.Cls;

#endregion

namespace Sobees
{
  public static class SobeesClientApp
  {
    [STAThread]
    public static void Main()
    {
      //if (!Debugger.IsAttached) Debugger.Launch();
      if (!SingleInstance.InitializeAsFirstInstance("sobees")) return;
      var splash = new SplashScreen("Resources/Images/SplashbDule.png");

      //MessageBox.Show("Attach Debugger now.... and press Enter");

      // Don't show this with the fade-out.  It pops the main window and doesn't look good.
      // Fixed in .Net with the TopMost property...
      try
      {
        splash.Show(false);
      }
      catch (Exception ex)
      {
        //
      }

      var application = new SobeesApplication();

      Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Loaded,
        (Action) (() =>
        {
          Thread.Sleep(1000);
          if (splash != null)
            splash.Close(TimeSpan.Zero);
        }));

      application.InitializeComponent();
      application.Run();

      // Allow single instance code to perform cleanup operations
      SingleInstance.Cleanup();
    }
  }
}