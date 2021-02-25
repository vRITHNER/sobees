#region

using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Markup;
using System.Xml;
using GalaSoft.MvvmLight.Messaging;
using Sobees.Library.BFacebookLibV2.Objects.Feed;
using Sobees.Library.BGenericLib;
using Sobees.Tools.Logging;

#endregion

namespace Sobees.Controls.Facebook.ViewModel
{
  

  public class ProfileHomeViewModel : HomeWorkspaceViewModel
  {
    #region Fields

    private User _userDisplay;

    public override double GetRefreshTime()
    {
      return Settings.RefreshTime;
    }

    #endregion

    #region Properties

    public override DataTemplate DataTemplateView
    {
      get
      {
        const string DT = "<DataTemplate x:Name='dtMainView' " +
                          "xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
                          "xmlns:Views='clr-namespace:Sobees.Controls.Facebook.Views;assembly=Sobees.Controls.Facebook' " +
                          "xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' >" +
                          "<Views:Home/> " +
                          "</DataTemplate>";

        var stringReader = new StringReader(DT);
        var xmlReader = XmlReader.Create(stringReader);
        return XamlReader.Load(xmlReader) as DataTemplate;
      }

      set { base.DataTemplateView = value; }
    }

    public User CurrentUser
    {
      get { return _userDisplay; }
      set
      {
        _userDisplay = value;
        RaisePropertyChanged();
      }
    }

    #endregion

    public ProfileHomeViewModel(FacebookViewModel viewModel, Messenger messenger)
      : base(viewModel, messenger)
    {
      Entries = new ObservableCollection<FacebookFeedEntry>();
      NewEntries = new ObservableCollection<FacebookFeedEntry>();
      DeleteSpamEntries = new ObservableCollection<FacebookFeedEntry>();
      EntriesDisplay = new ObservableCollection<FacebookFeedEntry>();
    }

    public void UpdateUser(User user)
    {
      Entries.Clear();
      EntriesDisplay.Clear();
      CurrentUser = user;
      Refresh();
    }

    public override void UpdateAll()
    {
      try
      {
        if (CurrentUser != null)
        {
          Service.Api.Fql.QueryAsync(
            $"SELECT post_id,target_id,actor_id, app_id, source_id, updated_time, created_time, type, attribution, message ,app_data, attachment, comments, likes FROM stream WHERE source_id = {CurrentUser.Id} LIMIT {Settings.NbPostToGet}", FqlStreamQueryCompleted, null);
        }
        else
        {
          EndUpdateAll();
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this,
          ex);
      }
    }
  }
}