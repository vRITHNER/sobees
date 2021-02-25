#region

using System;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Messaging;
using Sobees.Infrastructure.Cls;
using Sobees.Library.BFacebookLibV2.Objects.Feed;
using Sobees.Library.BGenericLib;
using Sobees.Tools.Logging;

#endregion

namespace Sobees.Controls.Facebook.ViewModel
{
  public class MyHomeWorkspaceViewModel : HomeWorkspaceViewModel
  {
    #region Constructors

    public MyHomeWorkspaceViewModel(FacebookViewModel model, Messenger messenger)
      : base(model, messenger)
    {
      Entries = new ObservableCollection<FacebookFeedEntry>();
      NewEntries = new ObservableCollection<FacebookFeedEntry>();
      DeleteSpamEntries = new ObservableCollection<FacebookFeedEntry>();
      EntriesDisplay = new ObservableCollection<FacebookFeedEntry>();
      Refresh();
    }

    #endregion

    #region Fields

    public override double GetRefreshTime()
    {
      return Settings.RefreshTime;
    }

    #endregion

    #region Properties

    #endregion

    #region Commands

    #endregion

    #region Methods

    public override void UpdateAll()
    {
      try
      {
        Service.Api.Fql.QueryAsync(
          $"SELECT post_id,target_id,actor_id, app_id, source_id, updated_time, created_time, type, attribution, message ,app_data, attachment, comments, likes FROM stream WHERE source_id= {SobeesSettings.Accounts[SobeesSettings.Accounts.IndexOf(new UserAccount(Settings.UserName, EnumAccountType.Facebook))].UserId} and is_hidden='0' LIMIT {Settings.NbPostToGet}",
          FqlStreamQueryCompleted,
          null);
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }

    #endregion

    #region Callback

    #endregion
  }
}