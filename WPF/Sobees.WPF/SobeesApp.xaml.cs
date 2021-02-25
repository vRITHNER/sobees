#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml.Linq;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Win32;
using MS.WindowsAPICodePack.Internal;
using Microsoft.WindowsAPICodePack.ApplicationServices;
using Sobees.Cls;
using Sobees.Configuration.BGlobals;
using Sobees.Infrastructure.Cls;
using Sobees.Infrastructure.Helpers;
using Sobees.Library.BUtilities;
using Sobees.Tools.Logging;
using Sobees.Tools.Threading.Extensions;
using Sobees.Tools.Util;
using Sobees.ViewModel;
using System.Windows.Shell;

#endregion

namespace Sobees
{
  /// <summary>
  ///   Interaction logic for App.xaml
  /// </summary>
  public partial class SobeesApplication : Application
  {
    private const string DEFAULT_THEME_NAME = "Classical";

    private static readonly Dictionary<string, Uri> ThemeLookup = new Dictionary<string, Uri>
                                                                    {
                                                                      {
                                                                        "Classical",
                                                                        new Uri("/Themes/ThemeClassical.xaml",
                                                                                UriKind.Relative)
                                                                        },
                                                                    };

    private static readonly List<string> ThemeNames = new List<string>(ThemeLookup.Keys);
    private static string recoveryFile = string.Empty;
    private ResourceDictionary _currentThemeDictionary;
    private double _fontSize = SobeesSettingsLocator.SobeesSettingsStatic.FontSizeValue;
    private MainWindow _mainWindow;

    public static bool IsFirstRun
    {
      get { return Sobees.Properties.Settings.Default.IsFirstRun; }
      set { Sobees.Properties.Settings.Default.IsFirstRun = value; }
    }

    public static string ThemeName
    {
      get { return Sobees.Properties.Settings.Default.ThemeName; }
      set
      {
        if (value != Sobees.Properties.Settings.Default.ThemeName)
        {
          Sobees.Properties.Settings.Default.ThemeName = value;
          ((SobeesApplication)Current).SwitchTheme(value);
        }
      }
    }

    public double FontSize
    {
      get { return _fontSize; }
      set
      {
        if (value == _fontSize) return;
        _fontSize = value;
        Resources.Remove("MyFontSize");
        Resources.Add("MyFontSize", _fontSize);
        Resources.Remove("MyFontSizeBigger");
        Resources.Add("MyFontSizeBigger", _fontSize + 2);
        Resources.Remove("MyFontSizeSmaller");
        Resources.Add("MyFontSizeSmaller", _fontSize - 2);
      }
    }

    /// <summary>
    ///   Whether the client is currently Tier 2 capable which is required for hardware-accelerated effects.
    /// </summary>
    public static bool IsShaderEffectSupported => RenderCapability.Tier == 0x00020000 && RenderCapability.IsPixelShaderVersionSupported(2, 0);

    public static bool AreUpdatesEnabled
    {
      get { return Sobees.Properties.Settings.Default.AreUpdatesEnabled; }
      set { Sobees.Properties.Settings.Default.AreUpdatesEnabled = value; }
    }
   
    public static bool DeleteCacheOnShutdown { get; set; }

    protected override void OnStartup(StartupEventArgs e)
    {
      // This is a way to get the app back to zero.
      //Settings.Default.Reset();
      base.OnStartup(e);
      NetworkChange.NetworkAvailabilityChanged += NetworkHelper.NetworkChangeNetworkAvailabilityChanged;
      SystemEvents.PowerModeChanged += SystemEventsPowerModeChanged;
      
      AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
      Dispatcher.UnhandledException += Dispatcher_UnhandledException;

      using (var worker = new BackgroundWorker())
      {
        worker.DoWork += delegate(object s,
                                  DoWorkEventArgs args)
                           {
                             if (worker.CancellationPending)
                             {
                               args.Cancel = true;
                               return;
                             }

                             Dispatcher.BeginInvokeIfRequired(() =>
                                                                {
                                                                  if (!CoreHelpers.RunningOnXP)
                                                                  {
                                                                    //Crash management
                                                                    recoveryFile =
                                                                      Path.Combine(
                                                                        Path.GetDirectoryName(AssemblyHelper.GetEntryAssemblyFullName()),"RecoveryData.xml");
                                                                    ApplicationRestartRecoveryManager.RegisterForApplicationRestart(new RestartSettings("/restart",RestartRestrictions.NotOnReboot |RestartRestrictions.NotOnPatch));

                                                                    var data = new RecoveryData(RecoveryProcedure,null);
                                                                    var settings = new RecoverySettings(data, 0);
                                                                    ApplicationRestartRecoveryManager.RegisterForApplicationRecovery(settings);
                                                                  }

                                                                  var jumpListResource =
                                                                    (JumpList)Resources["MainJumpList"];
                                                                  var jumpList = new JumpList(
                                                                    jumpListResource.JumpItems, false, false);
                                                                  //JumpList.SetJumpList(this, jumpList); //TODO:Fixe JumpList
                                                                });
                           };
        var frame = new DispatcherFrame();
        worker.RunWorkerCompleted += delegate { frame.Continue = false; };
        worker.RunWorkerAsync();
        Dispatcher.PushFrame(frame);

        _mainWindow = new MainWindow();
        MainWindow = _mainWindow;

        SwitchTheme(Sobees.Properties.Settings.Default.ThemeName);
        _mainWindow.Show();

      }

      SingleInstance.SingleInstanceActivated += SignalExternalCommandLineArgs;

      if (CoreHelpers.RunningOnXP) return;
      //Crash management
      if (!CheckCrashDuringLastSession()) return;
      BViewModelLocator.SettingsViewModelStatic.SupportLogFrom = BGlobals.SUPPORTLOG_CRASH_EMAIL_FROM;
      BViewModelLocator.SettingsViewModelStatic.SupportLogSubmitCommand.Execute(null);
      var doc = new XDocument(
        new XDeclaration("1.0", "UTF-8", "yes"),
        new XElement("root",
          new XElement("Name", "Sobees"),
          new XElement("Action", "")));
      doc.Save(recoveryFile, SaveOptions.DisableFormatting);
    }

    void Dispatcher_UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
      TraceHelper.Trace(
     string.Format("Dispatcher_UnhandledException:--> {0}|{1}", sender, Dispatcher.Thread.ManagedThreadId),
     e.Exception);
      
      e.Handled = true;
    }

    void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
      TraceHelper.Trace(
     string.Format("CurrentDomain_UnhandledException:--> {0}|{1}|{2}", sender, Dispatcher.Thread.ManagedThreadId, e.IsTerminating),
     e.ExceptionObject as Exception);
    }

    //
    private int RecoveryProcedure(object state)
    {
      TraceHelper.Trace(this, "RECOVERY: --> APPLICATION IS CRASHING...");
      TraceHelper.Trace(this, "RECOVERY: --> SET RecoveryData file FOR NEXT RESTART");

      var doc = new XDocument(
        new XDeclaration("1.0", "UTF-8", "yes"),
        new XElement("root",
                     new XElement("Name", "Sobees"),
                     new XElement("Action", "SendLogAfterCrash")));
      doc.Save(recoveryFile, SaveOptions.DisableFormatting);

      TraceHelper.Trace(this, "RECOVERY: --> END");

      ApplicationRestartRecoveryManager.ApplicationRecoveryFinished(true);
      return 0;
    }

    /// <summary>
    /// </summary>
    /// <returns></returns>
    private bool CheckCrashDuringLastSession()
    {
      if (!File.Exists(recoveryFile))
      {
        TraceHelper.Trace(this, string.Format("Recovery file {0} does not exist", recoveryFile));
        return false;
      }

      var doc = XDocument.Load(recoveryFile);
      var eleName = doc.Descendants("Name").FirstOrDefault();
      var eleAction = doc.Descendants("Action").FirstOrDefault();

      if (eleAction == null) return false;
      return eleAction.Value != string.Empty;
    }


    private void SystemEventsPowerModeChanged(object sender, PowerModeChangedEventArgs e)
    {
      switch (e.Mode)
      {
        case PowerModes.Resume:
          try
          {
            if (NetworkHelper.CheckConnection())
            {
              Messenger.Default.Send("GoOnline");
              TraceHelper.Trace(this, "Restore from sleep");
            }
          }
          catch (Exception)
          {
            throw;
          }
          break;
        case PowerModes.StatusChange:
          TraceHelper.Trace(this, "Status Change");
          break;
        case PowerModes.Suspend:
          Messenger.Default.Send("GoOffline");
          TraceHelper.Trace(this, "Sleep");
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    private void SignalExternalCommandLineArgs(object sender, SingleInstanceEventArgs e)
    {
      var handledByWindow = false;
      _mainWindow.Activate();
      ProcessCommandLineArgs(e.Args);
    }

    public bool ProcessCommandLineArgs(IList<string> commandLineArgs)
    {
      var ret = false;
      if (commandLineArgs == null || commandLineArgs.Count <= 0) return ret;
      var argIndex = 0;
      while (argIndex < commandLineArgs.Count)
      {
        var commandSwitch = commandLineArgs[argIndex].ToLowerInvariant();
        switch (commandSwitch)
        {
          case "jumplistpoststatus":
            Messenger.Default.Send("ShowMultiPost");
            ret = true;
            break;
        }
        ++argIndex;
      }

      return ret;
    }


    public void SwitchTheme(string themeName)
    {
      if (themeName == null)
        themeName = _GetNextTheme();

      Uri resourceUri = null;
      if (!ThemeLookup.TryGetValue(themeName,
                                   out resourceUri))
      {
        themeName = DEFAULT_THEME_NAME;
        resourceUri = ThemeLookup[themeName];
      }

      ThemeName = themeName;

      if (_currentThemeDictionary != null)
        Resources.MergedDictionaries.Remove(_currentThemeDictionary);

      _currentThemeDictionary = LoadComponent(resourceUri) as ResourceDictionary;
      Resources.MergedDictionaries.Insert(0,
                                          _currentThemeDictionary);
    }

    private string _GetNextTheme()
    {
      var index = ThemeNames.IndexOf(ThemeName);
      if (index == -1)
      {
        return DEFAULT_THEME_NAME;
      }

      return ThemeNames[(index + 1) % ThemeNames.Count];
    }

    internal static void PerformAggressiveCleanup()
    {
      GC.Collect(2);
      try
      {
        NativeMethods.SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, 40000, 80000);
      }
      catch (Exception ex)
      {
        Assert.Fail(ex.Message);
      }
    }

    private void ApplicationDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
      TraceHelper.Trace(
        string.Format("DispatcherUnhandledException:--> {0}|{1}", sender, Dispatcher.Thread.ManagedThreadId),
        e.Exception);

      e.Handled = true;
    }
  }
}