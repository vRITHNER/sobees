using System;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace Sobees.Infrastructure.ViewModelBase
{
  public class BSettingsViewModel : BWorkspaceViewModel
  {
    #region Constructors

    protected BSettingsViewModel(IMessenger messenger)
    {
      MessengerInstance = messenger;
    }
    /// <summary>
    /// Returns the command that, when invoked, ask user for saving Settings
    /// </summary>
    public RelayCommand SaveCommand
    {
      get;
      set;
    } 
    #endregion
  }
}