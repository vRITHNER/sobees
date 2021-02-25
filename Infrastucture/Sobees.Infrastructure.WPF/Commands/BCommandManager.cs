#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Sobees.Tools.Logging;

#endregion

namespace Sobees.Infrastructure.Commands
{
  public static class BCommandManager
  {
    #region DependencyProperty declarations

    public static readonly DependencyProperty CommandEventNameProperty =
      DependencyProperty.RegisterAttached("CommandEventName",
                                          typeof (String),
                                          typeof (BCommandManager),
                                          new PropertyMetadata(OnCommandEventNameChanged));

    public static readonly DependencyProperty CommandParameterProperty =
      DependencyProperty.RegisterAttached("CommandParameter",
                                          typeof (object),
                                          typeof (BCommandManager),
                                          new PropertyMetadata(OnCommandParameterChanged));

    public static readonly DependencyProperty CommandProperty =
      DependencyProperty.RegisterAttached("Command",
                                          typeof (BRelayCommand),
                                          typeof (BCommandManager),
                                          new PropertyMetadata(OnCommandChanged));

    #endregion

    #region DependencyProperty Get and Set methods

    public static BRelayCommand GetCommand(DependencyObject obj)
    {
      return (BRelayCommand) obj.GetValue(CommandProperty);
    }

    public static void SetCommand(DependencyObject obj, BRelayCommand command)
    {
      obj.SetValue(CommandProperty,
                   command);
    }

    public static string GetCommandEventName(DependencyObject obj)
    {
      return (string) obj.GetValue(CommandEventNameProperty);
    }

    public static void SetCommandEventName(DependencyObject obj, string commandEventName)
    {
      obj.SetValue(CommandEventNameProperty,
                   commandEventName);
    }

    public static string GetCommandParameter(DependencyObject obj)
    {
      return (string) obj.GetValue(CommandParameterProperty);
    }

    public static void SetCommandParameter(DependencyObject obj, string commandParameter)
    {
      obj.SetValue(CommandParameterProperty,
                   commandParameter);
    }

    #endregion

    static BCommandManager()
    {
      _commandDeclarations = new Dictionary<DependencyObject, CommandDeclaration>();
    }

    #region DependencyProperty Changed Callbacks

    private static void OnCommandChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
    {
      GetCommandDeclaration(obj).Command = (BRelayCommand) e.NewValue;
    }

    private static void OnCommandEventNameChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
    {
      GetCommandDeclaration(obj).EventName = (string) e.NewValue;
    }

    private static void OnCommandParameterChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
    {
      GetCommandDeclaration(obj).CommandParameters[0] = e.NewValue;
    }

    #endregion

    #region CommandDeclaration dictionary

    private static readonly Dictionary<DependencyObject, CommandDeclaration> _commandDeclarations;

    private static CommandDeclaration GetCommandDeclaration(DependencyObject obj)
    {
      if (_commandDeclarations.ContainsKey(obj)) return _commandDeclarations[obj];

      //nbCommand++;
      //TraceHelper.Trace("commandes:", nbCommand.ToString());
      var decl = new CommandDeclaration(obj);
      _commandDeclarations.Add(obj,
                               decl);
      var control = obj as Control;
      if (control != null)
      {
        control.Unloaded += ControlUnloaded;
      }
      return _commandDeclarations[obj];
    }

    private static void ControlUnloaded(object sender, RoutedEventArgs e)
    {
      if (sender == null)
      {
        return;
      }
      var control = sender as Control;
      if (control != null)
      {
        _commandDeclarations.Remove(control);
        control.Unloaded -= ControlUnloaded;
      }
    }

    public static void ClearCommands()
    {
      var lst2Remove = new List<DependencyObject>();
      foreach (var key in _commandDeclarations.Keys.Where(key => !InTree((FrameworkElement) key)))
      {
        lst2Remove.Add(key);
        ((FrameworkElement) key).Unloaded -= ControlUnloaded;
      }
      foreach (var dependencyObject in lst2Remove)
      {
        _commandDeclarations.Remove(dependencyObject);
      }
      TraceHelper.Trace("bCommandManager -->", lst2Remove.Count.ToString());
      lst2Remove.Clear();
#if !SILVERLIGHT
      GC.Collect(2);
#else
      GC.Collect();
#endif
    }

    #endregion

    #region Nested type: CommandDeclaration

    private class CommandDeclaration
    {
      private readonly DependencyObject _object;
      private BRelayCommand _cmd;
      private object[] _commandParameters;
      private string _eventName = String.Empty;

      public CommandDeclaration(DependencyObject obj)
      {
        _object = obj;
      }

      public DependencyObject Object => _object;

      public object[] CommandParameters
      {
        get
        {
          if (_commandParameters == null)
            _commandParameters = new object[3];

          return _commandParameters;
        }
        set { _commandParameters = value; }
      }

      public BRelayCommand Command
      {
        get { return _cmd; }
        set
        {
          if (_cmd == value)
            return;

          if (_cmd != null)
            DisconnectHandler();

          _cmd = value;

          if (_cmd != null)
            ConnectHandler();
        }
      }

      public string EventName
      {
        get { return _eventName; }
        set
        {
          if (_eventName == value)
            return;

          if (!string.IsNullOrEmpty(_eventName))
            DisconnectHandler();

          _eventName = value;

          if (string.IsNullOrEmpty(_eventName))
            return;

          if (!String.IsNullOrEmpty(_eventName))
            ConnectHandler();
        }
      }

      private void ConnectHandler()
      {
        if (Command != null && !String.IsNullOrEmpty(EventName))
        {
          var ev = GetEventInfo(_eventName);
          if (ev == null)
            TraceHelper.Trace(this, "Error in Commands");

          Command.EventInfo = ev;
          if (ev != null)
            ev.AddEventHandler(_object,
                               GetDelegate());
        }
      }

      private void DisconnectHandler()
      {
        if (!String.IsNullOrEmpty(EventName))
        {
          var ev = GetEventInfo(EventName);
          if (EventName != null)
          {
            if (ev != null)
              ev.RemoveEventHandler(_object,
                                    GetDelegate());
          }
        }
      }

      public void Handler(object sender, EventArgs e)
      {
        CommandParameters[1] = sender;
        CommandParameters[2] = e;
        if (Command != null && Command.CanExecute(CommandParameters))
          Command.Execute(CommandParameters);
      }

      private EventInfo GetEventInfo(string eventName)
      {
        var t = _object.GetType();
        var ev = t.GetEvent(eventName);
        return ev;
      }

      private Delegate GetDelegate()
      {
        var ev = GetEventInfo(EventName);
        if (ev != null)
          return Delegate.CreateDelegate(ev.EventHandlerType,
                                         this,
                                         GetType().GetMethod("Handler"),
                                         true);
        return null;
      }
    }

    #endregion

    #region unload SL

    private static bool InTree(FrameworkElement element)
    {
      var rootElement = Application.Current.MainWindow as FrameworkElement;

      while (element != null)
      {
        if (element == rootElement)
          return true;

        element = VisualTreeHelper.GetParent(element) as FrameworkElement;
      }

      return false;
    }

    #endregion
  }
}