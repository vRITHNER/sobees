#region

using GalaSoft.MvvmLight.Messaging;
using Sobees.Controls.Twitter.ViewModel;
using Sobees.Infrastructure.Cls;
using Sobees.Infrastructure.Controls;
using Sobees.Infrastructure.ViewModelBase;

#endregion

namespace Sobees.Controls.Twitter.Cls
{
  public class BTwitterViewModel : BWorkspaceViewModel
  {
    protected readonly TwitterViewModel _twitterViewModel;

    protected BTwitterViewModel(TwitterViewModel model, Messenger messenger, TwitterWorkspaceSettings settings)
      : this(model, messenger)
    {
      WorkspaceSettings = settings;
    }

    protected BTwitterViewModel(TwitterViewModel model, Messenger messenger)
    {
      _twitterViewModel = model;
      MessengerInstance = messenger;
    }

    public TwitterSettings Settings => _twitterViewModel.Settings as TwitterSettings;

    protected UserAccount CurrentAccount => _twitterViewModel.CurrentAccount;

    protected string PasswordHash => _twitterViewModel.PasswordHash;

    public TwitterWorkspaceSettings WorkspaceSettings { get; set; }

    public override void StartWaiting()
    {
      _twitterViewModel.StartWaiting();
      IsWaiting = true;
    }

    public override void StopWaiting()
    {
      _twitterViewModel.EndUpdateAll();
      _twitterViewModel.StopWaiting();
      IsWaiting = false;
    }

    public override void DoActionMessage(BMessage obj)
    {
    }
  }
}