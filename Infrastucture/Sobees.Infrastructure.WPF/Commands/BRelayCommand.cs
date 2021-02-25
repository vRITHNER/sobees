#region Includes

using System;
using System.Diagnostics;
using System.Windows.Input;
using Sobees.Tools.Logging;

#endregion

namespace Sobees.Infrastructure.Commands
{
  /// <summary>
  /// A command whose sole purpose is to 
  /// relay its functionality to other
  /// objects by invoking delegates. The
  /// default return value for the CanExecute
  /// method is 'true'.
  /// </summary>
  public class BRelayCommand : ICommand
  {
    #region Fields

    private readonly Action<object> _execute;
    private readonly Predicate<object> _canExecute;

    #endregion // Fields

    #region Properties

    public object EventInfo { get; set; }

    #endregion

    #region Constructors

    /// <summary>
    /// Creates a new command that can always execute.
    /// </summary>
    /// <param name="execute">The execution logic.</param>
    public BRelayCommand(Action<object> execute)
      : this(execute, null)
    {
    }

    /// <summary>
    /// Creates a new command.
    /// </summary>
    /// <param name="execute">The execution logic.</param>
    /// <param name="canExecute">The execution status logic.</param>
    public BRelayCommand(Action<object> execute, Predicate<object> canExecute)
    {
      if (execute == null)
        throw new ArgumentNullException("execute");

      _execute = execute;
      _canExecute = canExecute;
    }

    #endregion // Constructors

    #region ICommand Members

    [DebuggerStepThrough]
    public bool CanExecute(object parameter)
    {
      return _canExecute == null ? true : _canExecute(parameter);
    }


#if SILVERLIGHT
    public event EventHandler CanExecuteChanged;
#else
    public event EventHandler CanExecuteChanged {

      add { CommandManager.RequerySuggested += value; }
      remove { CommandManager.RequerySuggested -= value; }
    }
#endif

    public void Execute(object parameter)
    {
      try
      {
        _execute(parameter);
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }

    #endregion // ICommand Members

    public static object[] CheckParams(object obj)
    {
      try
      {
        if (obj == null || (obj as object[]) == null)
        {
          TraceHelper.Trace("RelayCommand::CheckParams::", "Command parameter was NULL!");
          return null;
        }

        var objs = obj as object[];
        if (objs.Length <= 2)
        {
          TraceHelper.Trace("RelayCommand::CheckParams::", "Command parameters were not complete!");
          return null;
        }

        return objs;
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("RelayCommand:CheckParams",
                          ex);
      }
      return new object[] { };
    }
  }
}