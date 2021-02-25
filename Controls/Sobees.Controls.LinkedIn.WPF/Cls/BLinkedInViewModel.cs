#region

using System.Collections.ObjectModel;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Sobees.Controls.LinkedIn.ViewModel;
using Sobees.Infrastructure.Controls;
using Sobees.Infrastructure.ViewModelBase;
using Sobees.Library.BLinkedInLib;
using Sobees.Tools.Threading.Extensions;

#endregion

namespace Sobees.Controls.LinkedIn.Cls
{
  public class BLinkedInViewModel : BWorkspaceViewModel
  {
    protected readonly LinkedInViewModel _linkedInViewModel;

    protected BLinkedInViewModel(LinkedInViewModel model, Messenger messenger)
    {
      MessengerInstance = messenger;
      MessengerInstance.Register<string>(this, DoAction);
      _linkedInViewModel = model;
    }

    public LinkedInSettings Settings => _linkedInViewModel.Settings as LinkedInSettings;

    public ObservableCollection<LinkedInUser> Friends
    {
      get
      {
        return _linkedInViewModel.Friends ?? (_linkedInViewModel.Friends = new ObservableCollection<LinkedInUser>());
      }
      set { _linkedInViewModel.Friends = value; }
    }

    public OAuthLinkedInV2 LinkedInLibV2 => _linkedInViewModel.LinkedInLibV2;

    #region Commands

    public RelayCommand<string> OpenProfileCommand { get; protected set; }

    #endregion Commands

    #region Methods

    protected void OpenProfile(string id)
    {
      MessengerInstance.Send(new BMessage("ShowProfile", id));
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

    public override void StartWaiting()
    {
      _linkedInViewModel.StartWaiting();
      IsWaiting = true;
    }

    public override void StopWaiting()
    {
      _linkedInViewModel.EndUpdateAll();
      IsWaiting = false;
    }

    #endregion Methods

    protected override void InitCommands()
    {
      OpenProfileCommand = new RelayCommand<string>(OpenProfile);
      base.InitCommands();
    }
  }
}