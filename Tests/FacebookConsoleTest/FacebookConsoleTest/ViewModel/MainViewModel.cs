namespace FacebookConsoleTest.ViewModel
{
  #region

  using System.Collections.ObjectModel;
  using System.Threading.Tasks;
  using GalaSoft.MvvmLight;
  using Sobees.Library.BFacebookLibV2;
  using Sobees.Library.BFacebookLibV2.Objects.Feed;
  using Sobees.Library.BFacebookLibV2.Objects.Users;

  #endregion

  public class MainViewModel : ViewModelBase
  {
    private FacebookUser _currentFacebookUser;
    private bool _isInProgress;

    public MainViewModel()
    {
      Init();
    }

    public FacebookUser CurrentFacebookUser
    {
      get { return _currentFacebookUser; }
      set
      {
        _currentFacebookUser = value;
        RaisePropertyChanged();
        Task.Run(async ()=> await RefreshData());
      }
    }

    public ObservableCollection<FacebookFeedEntry> FacebookFeedEntries { get; set; }

    public bool IsInProgress
    {
      get { return _isInProgress; }
      set
      {
        _isInProgress = value;
        RaisePropertyChanged();
      }
    }

    private void Init()
    {
    }

    private async Task<bool> RefreshData()
    {
      IsInProgress = true;
      FacebookFeedEntries = FacebookContext.Instance.GetHome(CurrentFacebookUser.Id);
      RaisePropertyChanged(() => FacebookFeedEntries);
      IsInProgress = false;
      return await Task.FromResult(true);
      
    }
  }
}