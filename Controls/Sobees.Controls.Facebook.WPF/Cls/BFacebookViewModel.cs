#region

using System.Windows;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Sobees.Controls.Facebook.ViewModel;
using Sobees.Infrastructure.ViewModelBase;
using Sobees.Library.BFacebookLibV2.Login;
using Sobees.Library.BGenericLib;
using Sobees.Tools.Threading.Extensions;

#endregion

namespace Sobees.Controls.Facebook.Cls
{
  public class BFacebookViewModel : BWorkspaceViewModel
  {
    #region Fields

    protected readonly FacebookViewModel FacebookViewModel;

    #endregion

    #region Properties

    public FacebookSettings Settings
    {
      get { return FacebookViewModel.Settings as FacebookSettings; }
      set { FacebookViewModel.Settings = value; }
    }

    public DesktopSession CurrentSession => FacebookViewModel.CurrentSession;

    #endregion

    #region Constructors

    public BFacebookViewModel()
    {
    }

    protected BFacebookViewModel(FacebookViewModel viewModel, Messenger messenger)
    {
      //Messenger.Default = messenger;
      FacebookViewModel = viewModel;
      Messenger.Default.Register<string>(this, DoAction);
    }

    #endregion

    #region Commands

    public RelayCommand<User> ShowUserProfileCommand { get; set; }

    #endregion

    #region Methods

    public override void StartWaiting()
    {
      FacebookViewModel.StartWaiting();
      IsWaiting = true;
    }

    public override void StopWaiting()
    {
      FacebookViewModel.EndUpdateAll();
      IsWaiting = false;
    }

    protected override void InitCommands()
    {
      ShowUserProfileCommand = new RelayCommand<User>(user =>
      {
        FacebookViewModel?.ShowUser(user);
      });
      base.InitCommands();
    }

    public override void DoAction(string param)
    {
      switch (param)
      {
        case "UpdateView":
          Application.Current.Dispatcher.BeginInvokeIfRequired(UpdateView);
          break;
      }
      base.DoAction(param);
    }

    #endregion
  }
}