#region

using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Sobees.Controls.Facebook.Cls;
using Sobees.Infrastructure.Cls;
using Sobees.Infrastructure.Controls;
using Sobees.Library.BGenericLib;
using Sobees.Tools.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using Sobees.Library.BFacebookLibV2.Exceptions;
using Sobees.Library.BFacebookLibV2.Extensions;
using Sobees.Library.BFacebookLibV2.Objects.Attachments;
using Sobees.Library.BFacebookLibV2.Objects.Feed;

#endregion

namespace Sobees.Controls.Facebook.ViewModel
{
  
  public class HomeWorkspaceViewModel : BFacebookViewModel
  {
    #region Constructors

    public HomeWorkspaceViewModel(FacebookViewModel viewModel, Messenger messenger)
      : base(viewModel, messenger)
    {
      InitializeWorkspace();
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

    #region Other

    public DateTime DStart = new DateTime(2005, 01, 01);

    public ObservableCollection<FacebookFeedEntry> Entries { get; set; }

    public ObservableCollection<FacebookFeedEntry> DeleteSpamEntries { get; set; }

    public ObservableCollection<FacebookFeedEntry> EntriesDisplay { get; set; }

    public ObservableCollection<FacebookFeedEntry> NewEntries { get; set; }

    private int NbToGet
    {
      get
      {
        if (SobeesSettings.Accounts == null) return 20;
        if (Settings == null) return 20;
        var i = SobeesSettings.Accounts.IndexOf(new UserAccount(Settings.UserName, EnumAccountType.Facebook));
        if (i > -1 && i < SobeesSettings.Accounts.Count)
          return SobeesSettings.Accounts[i].NbPostToGet;

        return 20;
      }
    }

    private int MaxPost
    {
      get
      {
        if (SobeesSettings.Accounts == null) return 100;
        if (Settings != null)
          return Settings.NbMaxPosts > 5 ? Settings.NbMaxPosts : 100;

        return 100;
      }
    }

    #endregion

    #region Filters

    //public bool ShowPhotos
    //{
    //  get { return _facebookViewModel.ShowPhotosHome; }
    //}

    public bool ShowLinks => FacebookViewModel.ShowLinksHome;

    public bool ShowApps => FacebookViewModel.ShowAppsHome;

    //public bool ShowVideos
    //{
    //  get { return _facebookViewModel.ShowVideosHome; }
    //}

    public bool ShowStatus => FacebookViewModel.ShowStatusHome;

    #endregion

    #endregion

    #region

    private void InitializeWorkspace()
    {
      InitCommands();
      Entries = new ObservableCollection<FacebookFeedEntry>();
      NewEntries = new ObservableCollection<Entry>();
      DeleteSpamEntries = new ObservableCollection<FacebookFeedEntry>();
      EntriesDisplay = new ObservableCollection<FacebookFeedEntry>();
    }

    #endregion

    #region Commands

    public RelayCommand<string> ShowAllCommentsCommand { get; set; }

    public RelayCommand<TextBox> AddCommentCommand { get; set; }

    public RelayCommand<Comment> DeleteCommentCommand { get; set; }

    public RelayCommand<FacebookFeedEntry> LikeCommand { get; set; }

    #endregion

    #region Methods

    public override void UpdateAll()
    {
      try
      {
        foreach (var entry in Entries)
          entry.HasBeenViewed = true;

        Service.Api.Fql.QueryStreamAsync("", false, FqlStreamQueryCompleted, null, "nf", CurrentSession.UserId.ToString(CultureInfo.InvariantCulture), NbToGet);
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }

    public void UpdateAllNew()
    {
      try
      {
        foreach (var entry in Entries)
          entry.HasBeenViewed = true;

        Service.Api.Fql.QueryAsync(
          $"SELECT post_id,target_id,actor_id, app_id, source_id, updated_time, created_time, type, attribution, message ,app_data, attachment, comments, likes FROM stream WHERE filter_key IN (SELECT filter_key FROM stream_filter WHERE uid= {CurrentSession.UserId}  ) and is_hidden='0' LIMIT {NbToGet}",
          FqlStreamQueryCompleted,
          null);
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }

    private void UpdateNumberPost()
    {
      try
      {
        //var max = 200;
        var max = MaxPost;
        var i = 0;

        while (i < Entries.Count)
        {
          if (i >= max)
            Entries.Remove(Entries[i]);
          else
            i++;
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }

    public override void UpdateView()
    {
      try
      {
        //If the List in cache is empty, the entries displayed must be empty too
        if (Entries == null || !Entries.Any())
        {
          EntriesDisplay?.Clear();
          return;
        }
        var i = 0;
        //We have to remove all entry in EntriesDisplay that aren't in Entries. We remove entries that don't match with filter.
        while (i < EntriesDisplay.Count)
        {
          if (!Entries.Contains(EntriesDisplay[i]) || (IsEntryContainSpam(EntriesDisplay[i])) ||
              !ShowEntry(EntriesDisplay[i]))
          {
            EntriesDisplay.RemoveAt(i);
          }
          else
          {
            EntriesDisplay[i].UpdatedTime = EntriesDisplay[i].UpdatedTime;
            i++;
          }
        }
        i = 0;
        //Add entries from cache that must be shown
        while (i < Entries.Count)
        {
          if (!IsEntryContainSpam(Entries[i]))
          {
            if (!EntriesDisplay.Contains(Entries[i]))
            {
              if (ShowEntry(Entries[i]))
              {
                var pos = 0;
                while (pos < EntriesDisplay.Count && Entries[i].UpdatedTime < EntriesDisplay[pos].UpdatedTime)
                  pos++;
                EntriesDisplay.Insert(pos, Entries[i]);
              }
            }
            else
            {
              EntriesDisplay[EntriesDisplay.IndexOf(Entries[i])].UpdatedTime = Entries[i].UpdatedTime;
              EntriesDisplay[EntriesDisplay.IndexOf(Entries[i])].Comments.Count = Entries[i].Comments.Count;
              EntriesDisplay[EntriesDisplay.IndexOf(Entries[i])].Likes.Count = Entries[i].Likes.Count;
              EntriesDisplay[EntriesDisplay.IndexOf(Entries[i])].Likes = Entries[i].Likes;
            }
          }
          i++;
        }
        IsAnyDataVisibility = EntriesDisplay.Any() ? Visibility.Collapsed : Visibility.Visible;
        UpdateNumberPostDisplay();
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }

    private void UpdateEntry(FacebookFeedEntry entry)
    {
      try
      {
        var pos = Entries.TakeWhile(en => !en.Id.Equals(entry.Id)).Count();
        var back = Entries.ElementAt(pos);

        if (back.UpdatedTime < entry.UpdatedTime || back.Likes.Count != entry.Likes.Count ||
            back.Comments.Count != entry.Comments.Count ||
            (back.Attachements != null && back.Attachements.DataAsObservableCollection.Any(a=> a.Medias != null) &&
             back.Attachements.DataAsObservableCollection.Any(a1 => a1.Medias.Medias.Count() != entry.Attachements.DataAsObservableCollection.Count(a2 => a2.Medias.Medias.Count)))
        {
          if (Application.Current == null || Application.Current.Dispatcher == null)
            return;
          Application.Current.Dispatcher.BeginInvoke(
            new Action(
              () =>
              {
                if (back.Attachement != null)
                  if (back.Attachement.Medias != null)
                  {
                    if (back.Attachement.Medias.Count != entry.Attachements.Medias.Count)
                      back.Attachement.Medias = entry.Attachements.Medias;
                  }

                back.UpdatedTime = entry.UpdatedTime;
                back.Likes = entry.Likes;

                if (back.Comments.Count == back.Comments.Count & back.Comments.Count > 3)
                  Service.Api.Stream.GetCommentsAsync(back.Id, GetCommentsCompleted, back.Id);
                else
                  back.Comments = entry.Comments;
                back.Comments.Count = entry.Comments.Count;
              }));
        }
        //Update User and Page
        try
        {
          if (back.User == null || back.User.Name == null || back.User.Name.Contains("Facebook User"))
            back.User = entry.User;
          if (back.ToUser == null || back.ToUser.Name == null || back.ToUser.Name.Contains("Facebook User"))
            back.ToUser = entry.ToUser;
          foreach (var comments in back.Comments)
          {
            if (comments.User == null || comments.User.NickName == null ||comments.User.NickName.Contains("Facebook User"))
            {
              var comments1 = comments;
              foreach (var comment in entry.Comments.Where(comment => comment.Id == comments1.Id))
                comments.User = comment.User;
            }
          }
          if (back.Likes == null) return;
          if (back.Likes.SampleUsersLike != null)
            foreach (var list in back.Likes.SampleUsersLike)
            {
              if (list != null && list.Name != null && !list.Name.Contains("Facebook User")) continue;

              foreach (var comment in entry.Likes.SampleUsersLike)
              {
                if (list == null) continue;
                if (list.Id != comment.Id) continue;
                list.Name = comment.Name;
                list.NickName = comment.NickName;
                list.ProfileImgUrl = comment.ProfileImgUrl;
              }
            }
          if (entry.Likes == null) return;
          if (entry.Likes.FriendsLike == null) return;
          foreach (var list in entry.Likes.FriendsLike)
          {
            if (list != null && list.Name != null && !list.Name.Contains("Facebook User")) continue;
            foreach (var comment in entry.Likes.FriendsLike)
            {
              if (list == null) continue;
              if (list.Id != comment.Id) continue;

              list.Name = comment.Name;
              list.NickName = comment.NickName;
              list.ProfileImgUrl = comment.ProfileImgUrl;
            }
          }
        }
        catch (Exception ex)
        {
          TraceHelper.Trace(this, ex);
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }

    //private bool IsEntryContainSpam(FacebookFeedEntry entry)
    //{
    //  var containSpam = false;
    //  try
    //  {
    //    if (SobeesSettings.Accounts == null || SobeesSettings.Accounts.Count < 1)
    //      return false;

    //    if (
    //      SobeesSettings.Accounts[
    //        SobeesSettings.Accounts.IndexOf(new UserAccount(Settings.UserName, EnumAccountType.Facebook))].SpamList !=
    //      null)
    //    {
    //      foreach (var spam in
    //        SobeesSettings.Accounts[
    //          SobeesSettings.Accounts.IndexOf(new UserAccount(Settings.UserName, EnumAccountType.Facebook))].SpamList)
    //      {
    //        if (entry.Content.ToLower().Contains(spam.ToLower()))
    //        {
    //          containSpam = true;
    //          break;
    //        }
    //        if (entry.User != null)
    //        {
    //          if (entry.User.NickName.ToLower().Contains(spam.ToLower()))
    //          {
    //            containSpam = true;
    //            break;
    //          }
    //        }
    //        if (entry.Attachement == null) continue;
    //        if (entry.Attachements.Description != null)
    //        {
    //          if (entry.Attachements.Description.ToLower().Contains(spam.ToLower()))
    //          {
    //            containSpam = true;
    //            break;
    //          }
    //        }
    //        if (entry.Attachements.Name != null)
    //        {
    //          if (entry.Attachements.Name.ToLower().Contains(spam.ToLower()))
    //          {
    //            containSpam = true;
    //            break;
    //          }
    //        }
    //        if (entry.Attachements.Caption == null) continue;
    //        if (!entry.Attachements.Caption.ToLower().Contains(spam.ToLower())) continue;
    //        containSpam = true;
    //        break;
    //      }
    //    }
    //  }
    //  catch (Exception ex)
    //  {
    //    TraceHelper.Trace(this, ex);
    //  }

    //  return containSpam;
    //}

    /// <summary>
    /// </summary>
    /// <param name="timestamp"></param>
    /// <returns></returns>
    public static DateTime ConvertUniversalTimeToDate(double timestamp)
    {
      // First make a System.DateTime equivalent to the UNIX Epoch.
      var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);

      // Add the number of seconds in UNIX timestamp to be converted.
      return dateTime.AddSeconds(timestamp);
    }

    private void AddEntry(FacebookFeedEntry entry)
    {
      try
      {
        if (Entries == null)
          return;
        var i = Entries.TakeWhile(e => e.UpdatedTime >= entry.UpdatedTime).Count();
        Entries.Insert(i, entry);
        if (entry.UpdatedTime <= Settings.DateLastUpdate)
          return;
        if (Settings.DateLastUpdate == DateTime.MinValue)
          return;
        //entry.HasBeenViewed = false;

        switch (
          SobeesSettings.Accounts[
            SobeesSettings.Accounts.IndexOf(new UserAccount(Settings.UserName, EnumAccountType.Facebook))].TypeAlertsFB)
        {
        case EnumAlertsFacebookType.All:
          NewEntries.Add(entry);
          break;

        case EnumAlertsFacebookType.Advanced:
          //Don't show messagewith removed word
          if (
            SobeesSettings.Accounts[
              SobeesSettings.Accounts.IndexOf(new UserAccount(Settings.UserName, EnumAccountType.Facebook))].
              IsCheckedUseAlertsRemovedWords)
          {
            if (
              SobeesSettings.Accounts[
                SobeesSettings.Accounts.IndexOf(new UserAccount(Settings.UserName, EnumAccountType.Facebook))]
                .AlertsRemovedWordsList.Where(word => entry.Attachements.DataAsObservableCollection.Any()).Where(word => entry.Attachements.DataAsObservableCollection.Any(a=> a.Description != null)).Any(
                  word =>
                    entry.Caption.ToLower().Contains(word.ToLower()) ||
                    entry.Attachements.DataAsObservableCollection.Any(a=> a.Description.ToLower().Contains(word.ToLower()))))
            {
              return;
            }
          }
          if (
            SobeesSettings.Accounts[
              SobeesSettings.Accounts.IndexOf(new UserAccount(Settings.UserName, EnumAccountType.Facebook))]
              .IsCheckedUseAlertsWords)
          {
            if (
              SobeesSettings.Accounts[
                SobeesSettings.Accounts.IndexOf(new UserAccount(Settings.UserName, EnumAccountType.Facebook))]
                .AlertsWordsList.Where(
                  word => entry.Attachements.DataAsObservableCollection != null).Where(word => entry.Attachements.DataAsObservableCollection.Any(a=> a.Description != null)).Any(
                    word =>
                      entry.Caption.ToLower().Contains(word.ToLower()) ||
                      entry.Attachements.DataAsObservableCollection.Any(a=> a.Description.ToLower().Contains(word.ToLower()))))
            {
              NewEntries.Add(entry);
              return;
            }
          }
          if (
            SobeesSettings.Accounts[
              SobeesSettings.Accounts.IndexOf(new UserAccount(Settings.UserName, EnumAccountType.Facebook))]
              .IsCheckedUseAlertsUsers)
          {
            if (
              SobeesSettings.Accounts[
                SobeesSettings.Accounts.IndexOf(new UserAccount(Settings.UserName, EnumAccountType.Facebook))]
                .AlertsUsersList.Where(
                  user => entry.From != null).Any(user => entry.From.Name.ToLower().Contains(user.ToLower())))
            {
              NewEntries.Add(entry);
              return;
            }
          }
          if (
            !SobeesSettings.Accounts[
              SobeesSettings.Accounts.IndexOf(new UserAccount(Settings.UserName, EnumAccountType.Facebook))]
              .IsCheckedUseAlertsUsers &&
            !SobeesSettings.Accounts[
              SobeesSettings.Accounts.IndexOf(new UserAccount(Settings.UserName, EnumAccountType.Facebook))]
              .IsCheckedUseAlertsWords &&
            !SobeesSettings.Accounts[
              SobeesSettings.Accounts.IndexOf(new UserAccount(Settings.UserName, EnumAccountType.Facebook))].
              IsCheckedUseAlertsRemovedWords)
          {
            NewEntries.Add(entry);
          }
          break;

        case EnumAlertsFacebookType.No:
          break;
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }

    private void ChangeLike(FacebookFeedEntry entry)
    {
      if (entry == null)
        return;
      if (entry.Likes.LikeIt == 1)
        UnlikePost(entry);
      else
        LikePost(entry);
    }

    private void UnlikePost(Entry entry)
    {
      try
      {
        Service.Api.Stream.RemoveLikeAsync(entry.Id, StreamModifyLikesCompleted, entry);
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }

    private void LikePost(Entry entry)
    {
      try
      {
        Service.Api.Stream.AddLikeAsync(entry.Id, StreamModifyLikesCompleted, entry);
      }
      catch (Exception ex)
      {
        //ShowErrorMsg(ex.Message);
        TraceHelper.Trace(this, ex);
      }
    }

    /// <summary>
    ///   ShowEntry
    /// </summary>
    /// <param name="entry"></param>
    /// <returns></returns>
    private bool ShowEntry(FacebookFeedEntry entry)
    {
      try
      {
        
        return (((ShowLinks || !entry.Type.Equals(80)) && (ShowApps || !entry.Type.Equals(237)) &&
                 (!entry.Type.Equals(128)) &&
                 ((!entry.Type.Equals(236) && !entry.Type.Equals(66))) &&
                 ((!entry.Type.Equals(247) && !entry.Type.Equals(259) && !entry.Type.Equals(82) &&
                   !entry.Type.Equals(79)))) &&
                (ShowStatus || (!entry.Type.Equals(46) && !entry.Type.Equals(56) && !entry.Type.Equals(273))) &&
                (string.IsNullOrEmpty(SobeesSettings.Filter) ||
                 entry.From.Name != null && entry.From.Name.ToUpper().Contains(SobeesSettings.Filter.ToUpper()) ||
                 entry.Caption.ToUpper().Contains(SobeesSettings.Filter.ToUpper()) ||
                 entry.Attachements != null && entry.Attachements.Data.Any(a1=> a1.Description != null &&
                 entry.Attachements.Data.Any(a2 => a2.Description.ToUpper().Contains(SobeesSettings.Filter.ToUpper()) ||
                 entry.Attachements.Data.Any(a3 => a3.Description != null && entry.Attachements.Data.Any(a4 => a4.Name != null &&
                 entry.Attachements.Data.Any(a5 => a5.Name.ToUpper().Contains(SobeesSettings.Filter.ToUpper()) ||
                 entry.Attachements != null && entry.Attachements.Data.Any(a6 => a6.Title != null &&
                 entry.Attachements.Data.Any(a7 => a7.Title.ToUpper().Contains(SobeesSettings.Filter.ToUpper())));
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
      return true;
    }

    protected override void InitCommands()
    {
      ShowAllCommentsCommand = new RelayCommand<string>(UpdateComment);
      LikeCommand = new RelayCommand<FacebookFeedEntry>(ChangeLike);
      AddCommentCommand = new RelayCommand<TextBox>(AddComment);
      DeleteCommentCommand = new RelayCommand<Comment>(DeleteComment);
      base.InitCommands();
    }

    private void DeleteComment(Comment comment)
    {
      if (comment != null)
        RemoveComment(comment.Id);
    }

    private void RemoveComment(string id)
    {
      var regexXml = new Regex("\\d*?_\\d*");
      try
      {
        Service.Api.Stream.RemoveCommentAsync(id, StreamRemoveCommentCompleted, regexXml.Match(id).Value);
      }
      catch (Exception ex)
      {
        //ShowErrorMsg(ex.Message);
        TraceHelper.Trace(this, ex);
      }
    }

    private void AddComment(TextBox txBx)
    {
      if (txBx != null)
      {
        var entry = txBx.DataContext as FacebookFeedEntry;
        if (entry == null) return;
        var postId = entry.Id;
        var textComment = txBx.Text;
        if (string.IsNullOrEmpty(textComment)) return;
        txBx.Text = "";
        //IsWaiting = false;
        AddComment(postId, textComment);
      }
    }

    private void AddComment(string id, string text)
    {
      try
      {
        Service.Api.Stream.AddCommentAsync(id, text, StreamAddCommentCompleted, id);
      }
      catch (Exception ex)
      {
        //ShowErrorMsg(ex.Message);
        TraceHelper.Trace(this, ex);
      }
    }

    private void UpdateComment(string postUid)
    {
      if (postUid == null) return;
      try
      {
        StartWaiting();
        Service.Api.Stream.GetCommentsAsync(postUid, GetCommentsCompleted, postUid);
      }
      catch (Exception ex)
      {
        //ShowErrorMsg(ex.Message);
        TraceHelper.Trace(this, ex);
      }
    }

    #region UPDATE NUMBER POST

    private void UpdateNumberPostDisplay()
    {
      try
      {
        if (EntriesDisplay == null || EntriesDisplay.Count < 1)
          return;

        var max = MaxPost;
        var i = 0;

        while (i < EntriesDisplay.Count)
        {
          if (i >= max)
            EntriesDisplay.Remove(EntriesDisplay[i]);
          else
            i++;
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }

    #endregion

    #endregion

    #region Callback

    private void StreamRemoveCommentCompleted(bool result, object state, FacebookException e)
    {
      try
      {
        var postId = state as String;

        UpdateComment(postId);
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }

    protected void FqlStreamQueryCompleted(string result, object state, FacebookException e)
    {
      if (e != null)
      {
        MessengerInstance.Send(new BMessage("ShowError", e.Message));
        EndUpdateAll();
        return;
      }
      try
      {
        var userId = CurrentSession.UserId;

        List<long> lst;
        var lstEntry = FacebookHelper.ConvertFQLToStream(result, userId.ToString(CultureInfo.InvariantCulture));
        if (result.Contains("</profile>"))
        {
          var lstUser = FacebookHelper.ConvertSTREAMToUser(result);
          foreach (var user in lstUser)
          {
            Service.AddUser(new user
            {
              uid = Convert.ToInt64(user.Id),
              name = user.Name,
              profile_url = user.Url,
              pic_square = user.ProfileImgUrl
            });
          }
        }
        lstEntry = FbHelper.AddInfoUser(lstEntry, Service.Friends, out lst);
        if (lst.Count > 0)
        {
          Service.Api.Users.GetInfoAsync(lst, GetUserInfoStreamCompleted, lstEntry);
          //var lstFields = new List<string> {"page_id", "name", "pic_square", "page_url", "type", "fan_count"};
          //lst.Clear(); //doesn't work anymore
          //Service.Api.Pages.GetInfoAsync(lstFields, lst, userId, GetPageInfoStreamCompleted, lstEntry);
        }
        else
        {
          GetStreamCompleted(lstEntry);
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
      EndUpdateAll();
    }

    private void GetUserInfoStreamCompleted(IList<User> users, object state, FacebookException e)
    {
      if (e != null && e.ErrorCode != 0)
      {
        TraceHelper.Trace(this, string.Format("GetUserInfoStreamCompleted:{0}", e.Message));
        MessengerInstance.Send(new BMessage("ShowError", e.Message));
        EndUpdateAll();
        return;
      }
      if (users == null)
        return;

      foreach (var user in users)
        Service.AddUser(user);
      var lst = (List<FacebookFeedEntry>)state;
      GetStreamCompleted(FbHelper.AddInfoUser(lst, Service.Friends));
    }

    private void GetCommentsCompleted(IList<comment> comments, object state, FacebookException e)
    {
      try
      {
        if (e != null)
        {
          MessengerInstance.Send(new BMessage("ShowError", e.Message));
          EndUpdateAll();
          return;
        }
        if (comments != null)
        {
          var lstObjSmall = new Object[2];
          var lstObjBig = new Object[2];
          lstObjSmall[0] = state;
          lstObjBig[0] = state;
          var i = 0;
          var lstCommentSmall = new List<comment>();
          var lstSmall = new List<long>();
          var lstBig = new List<long>();
          foreach (var comment in comments)
          {
            if (i < 20)
            {
              lstCommentSmall.Insert(0, comment);
              lstSmall.Add(comment.fromid);
              lstBig.Add(comment.fromid);
            }
            else
            {
              lstBig.Add(comment.fromid);
            }
            i++;
          }
          lstObjSmall[1] = lstCommentSmall;
          lstObjBig[1] = comments;
          Service.Api.Users.GetInfoAsync(lstSmall, GetUserInfoCompleted, lstObjSmall);
        }
        else
        {
          foreach (var entry in Entries)
          {
            if (!entry.Id.Equals(state)) continue;
            if (Application.Current == null || Application.Current.Dispatcher == null)
              return;
            var currentEntry = entry;
            Application.Current.Dispatcher.BeginInvoke(
              new Action(
                () =>
                {
                  currentEntry.Comments.Clear();
                  currentEntry.Comments.Count = 0;
                }));
            break;
          }
        }
        StopWaiting();
      }
      catch (Exception ex)
      {
        StopWaiting();
        //ShowErrorMsg(ex.Message);
        TraceHelper.Trace(this, ex);
      }
    }

    private void GetUserInfoCompleted(IList<User> users, object state, FacebookException e)
    {
      try
      {
        var lstState = state as Object[];
        if (lstState == null)
          return;
        var idPost = lstState[0] as String;
        var lstComm = lstState[1] as IList<Comment>;
        if (lstComm == null)
          return;
        var lstNewComm = new ObservableCollection<Comment>();
        foreach (var comm in lstComm.OrderBy(c => c.time))
        {
          var c = new Comment { Id = comm.id, Date = FbHelper.ConvertUniversalTimeToDate(comm.time), Body = comm.text };
          var t = comm.id.Substring(0, comm.id.IndexOf("_", StringComparison.Ordinal));

          if (
            comm.fromid.ToString(CultureInfo.InvariantCulture)
              .Equals(CurrentSession.UserId.ToString(CultureInfo.InvariantCulture)) ||
            t.Equals(CurrentSession.UserId.ToString(CultureInfo.InvariantCulture)))
            c.CanRemoveComment = 1;

          foreach (var u in users)
          {
            if (!u.uid.Equals(comm.fromid)) continue;
            var nUser = new User { Id = u.uid.ToString(), NickName = u.name, ProfileImgUrl = u.pic_square };
            c.User = nUser;
            lstNewComm.Add(c);
            break;
          }
          if (string.IsNullOrEmpty(c.User?.NickName))
          {
            c.User = new User
            {
              NickName = "Facebook User",
              ProfileImgUrl = "http://static.ak.fbcdn.net/pics/q_silhouette.gif"
            };
          }
        }

        if (Application.Current == null || Application.Current.Dispatcher == null)
          return;
        Application.Current.Dispatcher.BeginInvoke(
          new Action(
            () =>
            {
              foreach (var entry in Entries)
              {
                if (!entry.Id.Equals(idPost)) continue;
                if (entry.Comments == null || !entry.Comments.Data.Any())
                {
                  entry.Comments.Data = lstNewComm;
                  break;
                }
                foreach (var com in lstNewComm)
                {
                  if (entry.Comments.Data.Contains(com)) continue;
                  var i = 0;
                  foreach (var oldCom in entry.Comments)
                  {
                    if (oldCom.Date < com.Date)
                    {
                      entry.Comments.Insert(i, com);
                      break;
                    }
                    i++;
                  }
                }
                break;
              }
              foreach (var entry in EntriesDisplay)
              {
                if (!entry.Id.Equals(idPost)) continue;
                if (entry.Comments == null || entry.Comments.Count == 0)
                {
                  entry.Comments = lstNewComm;
                  break;
                }
                foreach (var com in lstNewComm)
                {
                  if (!entry.Comments.Contains(com))
                  {
                    var i = 0;
                    foreach (var oldCom in entry.Comments)
                    {
                      if (oldCom.Date > com.Date)
                      {
                        entry.Comments.Insert(i, com);
                        break;
                      }
                      i++;
                    }
                  }
                }
                break;
              }
            }));
      }
      catch (Exception ex)
      {
        //ShowErrorMsg(ex.Message);
        TraceHelper.Trace(this, ex);
      }
    }

    private void StreamModifyLikesCompleted(bool result, object state, FacebookException e)
    {
      try
      {
        var entry = state as FacebookFeedEntry;
        if (entry == null) return;
        var like = entry.Likes;

        if (Application.Current == null || Application.Current.Dispatcher == null)
          return;
        Application.Current.Dispatcher.BeginInvoke(
          new Action(
            () =>
            {
              if (like.LikeIt == 1)
              {
                like.LikeIt = 0;
                like.Count--;
                if (like.Count < 0)
                  like.Count = 0;
              }
              else
              {
                like.LikeIt = 1;
                like.Count++;
              }
              entry.Likes = like;

              var pos = 0;
              foreach (var en in EntriesDisplay)
              {
                if (en.Id.Equals(entry.Id))
                {
                  //en.Likes = like;
                  EntriesDisplay.RemoveAt(pos);
                  EntriesDisplay.Insert(pos, entry);
                  break;
                }
                pos++;
              }
            }));
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }

    private void StreamAddCommentCompleted(string commentid, object state, FacebookException e)
    {
      try
      {
        var postId = state as String;
        UpdateComment(postId);
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }

    private void GetStreamCompleted(IEnumerable<FacebookFeedEntry> entries)
    {
      var facebookEntries = entries as IList<FacebookFeedEntry> ?? entries.ToList();
      if (entries == null || !facebookEntries.Any())
      {
        EndUpdateAll();
        return;
      }
      NewEntries.Clear();
      foreach (var entry in facebookEntries)
      {
        if (Entries.Contains(entry))
          UpdateEntry(entry);
        else
          AddEntry(entry);
      }
      ShowAlerts(NewEntries, Settings.UserName, EnumAccountType.Facebook);
      Settings.DateLastUpdate = Entries[0].UpdatedTime;

      if (Application.Current == null || Application.Current.Dispatcher == null)
        return;
      Application.Current.Dispatcher.BeginInvoke(
        new Action(
          () =>
          {
            UpdateNumberPost();
            UpdateView();
          }));
      EndUpdateAll();
    }

    #endregion
  }
}