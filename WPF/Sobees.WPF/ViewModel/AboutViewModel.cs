#region

using System.Reflection;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Sobees.Infrastructure.ViewModelBase;

#endregion

namespace Sobees.ViewModel
{
  public class AboutViewModel : BWorkspaceViewModel
  {
    public AboutViewModel()
    {
      InitCommands();
    }

    public string VersionNumber => Assembly.GetExecutingAssembly().GetName().Version.ToString();

    protected override void InitCommands()
    {
      #region Commands

      CloseCommand = new RelayCommand(() => Messenger.Default.Send("CloseAbout"));

      #endregion

      base.InitCommands();
    }
  }
}