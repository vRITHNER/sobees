using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Runtime.Serialization.Formatters;
using System.Threading;
using System.Windows;
using Sobees.Library;
using Sobees.Library.BUtilities;
using Sobees.Tools.Logging;
using Sobees.Tools.Threading;
using Sobees.Tools.Util;

namespace Sobees.Cls
{
  public class SingleInstanceEventArgs : EventArgs
  {
    public IList<string> Args { get; internal set; }
  }

  public static class SingleInstance
  {
    private const string _RemoteServiceName = "SingleInstanceApplicationService";
    private static Mutex _singleInstanceMutex;
    private static IpcServerChannel _channel;
    public static event EventHandler<SingleInstanceEventArgs> SingleInstanceActivated;

    public static bool InitializeAsFirstInstance(string applicationName)
    {
      IList<string> commandLineArgs = Environment.GetCommandLineArgs() ?? new string[0];

      // Build a repeatable machine unique name for the channel.
      string appId = applicationName + Environment.UserName;
      string channelName = appId + ":SingleInstanceIPCChannel";

      bool isFirstInstance;
      _singleInstanceMutex = new Mutex(true, appId, out isFirstInstance);
      if (isFirstInstance)
      {
        _CreateRemoteService(channelName);
      }
      else
      {
        _SignalFirstInstance(channelName, commandLineArgs);
      }

      return isFirstInstance;
    }

    public static void Cleanup()
    {
      Utility.SafeDispose(ref _singleInstanceMutex);

      if (_channel != null)
      {
        ChannelServices.UnregisterChannel(_channel);
        _channel = null;
      }
    }

    private static void _CreateRemoteService(string channelName)
    {
      _channel = new IpcServerChannel(
        new Dictionary<string, string>
          {
            {"name", channelName},
            {"portName", channelName},
            {"exclusiveAddressUse", "false"},
          },
        new BinaryServerFormatterSinkProvider {TypeFilterLevel = TypeFilterLevel.Full});

      ChannelServices.RegisterChannel(_channel, true);
      RemotingServices.Marshal(new _IpcRemoteService(), _RemoteServiceName);
    }

    private static void _SignalFirstInstance(string channelName, IList<string> args)
    {
      var secondInstanceChannel = new IpcClientChannel();
      ChannelServices.RegisterChannel(secondInstanceChannel, true);

      string remotingServiceUrl = "ipc://" + channelName + "/" + _RemoteServiceName;

      // Obtain a reference to the remoting service exposed by the first instance of the application
      var firstInstanceRemoteServiceReference =
        (_IpcRemoteService) RemotingServices.Connect(typeof (_IpcRemoteService), remotingServiceUrl);

      // Pass along the current arguments to the first instance if it's up and accepting requests.
      if (firstInstanceRemoteServiceReference != null)
      {
        try
        {
          firstInstanceRemoteServiceReference.InvokeFirstInstance(args);
        }
        catch (Exception ex)
        {
          //MessageBox.Show("Please close any instance of sobees before launching a new one!");
          try
          {
            _startLoop:
            var processId = ProcessHelper.GetCurrentProcessId();
            foreach (var process in Process.GetProcesses(AssemblyHelper.GetEntryAssemblyName()))
            {
              if (process.Id != processId)
              {
                TraceHelper.Trace("Sobees", "SingleInstance::FirstInstance:Found an existing Sobees instance ! It needs to close it before lauchning a new one !");
                
                //var hwnd = Process.GetProcessById(process.Id).MainWindowHandle;
                process.Kill();
                TraceHelper.Trace("Sobees", "SingleInstance::FirstInstance:Process has been killed !");
                goto _startLoop; //Restart to examin all the processes to see if another sobees instance still run

              }
            }

           firstInstanceRemoteServiceReference.InvokeFirstInstance(args);
          }
          catch (Exception)
          {
            TraceHelper.Trace("SingleInstance::FirstInstance:error when trying to kill process",
                          (ex));
          }
        }
      }
    }

    private static void _ActivateFirstInstance(IList<string> args)
    {
      if (Application.Current != null && !Application.Current.Dispatcher.HasShutdownStarted)
      {
        EventHandler<SingleInstanceEventArgs> handler = SingleInstanceActivated;
        if (handler != null)
        {
          handler(Application.Current, new SingleInstanceEventArgs {Args = args});
        }
      }
    }

    #region Nested type: _IpcRemoteService

    private class _IpcRemoteService : MarshalByRefObject
    {
      /// <summary>Activate the first instance of the application.</summary>
      /// <param name="args">Command line arguemnts to proxy.</param>
      public void InvokeFirstInstance(IList<string> args)
      {
        if (Application.Current != null && !Application.Current.Dispatcher.HasShutdownStarted)
        {
          Application.Current.Dispatcher.BeginInvoke(
            (Action<object>) ((arg) => _ActivateFirstInstance((IList<string>) arg)), args);
        }
      }

      /// <summary>Overrides the default lease lifetime of 5 minutes so that it never expires.</summary>
      public override object InitializeLifetimeService()
      {
        return null;
      }
    }

    #endregion
  }
}