#region

using GalaSoft.MvvmLight.Messaging;
using Sobees.Controls.LinkedIn.Cls;
using Sobees.Infrastructure.Cls;
using Sobees.Infrastructure.Commands;
using Sobees.Infrastructure.Controls;
using Sobees.Library.BGenericLib;
using Sobees.Library.BLinkedInLib;
using Sobees.Library.BLocalizeLib;
using Sobees.Tools.Logging;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Threading;
using System.Xml;

#endregion

namespace Sobees.Controls.LinkedIn.ViewModel
{
  public class HomeViewModel : BLinkedInViewModel
  {
    public HomeViewModel(LinkedInViewModel model, Messenger messenger)
      : base(model, messenger)
    {
      Entries = new DispatcherNotifiedObservableCollection<LinkedInEntry>();
      EntriesDisplay = new DispatcherNotifiedObservableCollection<LinkedInEntry>();
      NewEntries = new ObservableCollection<Entry>();

      InitCommands();

      Refresh();
    }

    #region Fields

    private BRelayCommand _postCommentCommand;

    #endregion Fields

    #region Properties

    public override DataTemplate DataTemplateView
    {
      get
      {
        const string dt =
          "<DataTemplate x:Name='dtLinkedIn' " + "xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " + "xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' " +
          "xmlns:LinkedIn='clr-namespace:Sobees.Controls.LinkedIn.Views;assembly=Sobees.Controls.LinkedIn'>" + "<Grid HorizontalAlignment='Stretch' VerticalAlignment='Stretch'>" +
          "<LinkedIn:Home />" + "</Grid>" + "</DataTemplate>";

        var stringReader = new StringReader(dt);
        var xmlReader = XmlReader.Create(stringReader);
        return XamlReader.Load(xmlReader) as DataTemplate;
      }
      set { base.DataTemplateView = value; }
    }

    public DispatcherNotifiedObservableCollection<LinkedInEntry> Entries { get; set; }

    public ObservableCollection<Entry> NewEntries { get; set; }

    public DispatcherNotifiedObservableCollection<LinkedInEntry> EntriesDisplay { get; set; }

    public bool ShowCONN => _linkedInViewModel.ShowCONN;

    public bool ShowSTAT => _linkedInViewModel.ShowSTAT;

    public bool ShowAPPS => _linkedInViewModel.ShowAPPS;

    public bool ShowJOBS => _linkedInViewModel.ShowJOBS;

    public bool ShowJGRP => _linkedInViewModel.ShowJGRP;

    public bool ShowRECU => _linkedInViewModel.ShowRECU;

    public bool ShowPRFU => _linkedInViewModel.ShowPRFU;

    public bool ShowOTHER => _linkedInViewModel.ShowOTHER;

    #endregion Properties

    #region Commands

    public BRelayCommand PostCommentCommand => _postCommentCommand ?? (_postCommentCommand = new BRelayCommand(PostComment));

    #endregion Commands

    #region Methods

    protected bool IdEntryComment { get; set; }

    private void PostComment(object obj)
    {
      var txt = obj as TextBox;
      if (txt == null) return;
      var entry = txt.DataContext as Entry;
      if (entry == null) return;
      {
        var comment = RemoveAccentFromString(txt.Text);

        if (string.IsNullOrEmpty(comment))
          return;

        IdEntryComment = true;

        using (var worker = new BackgroundWorker())
        {
          worker.DoWork += delegate(object s, DoWorkEventArgs args)
                           {
                             if (worker.CancellationPending)
                             {
                               args.Cancel = true;
                               return;
                             }
                             try
                             {
                               IsWaiting = true;
                               if (LinkedInLibV2.SetComments(comment, entry.Id))
                               {
                                 txt.Dispatcher.BeginInvoke(DispatcherPriority.Background, (DispatcherOperationCallback)delegate
                                                                                                                         {
                                                                                                                           txt.Text = string.Empty;
                                                                                                                           return null;
                                                                                                                         }, null);
                                 Refresh();
                               }
                             }
                             catch (Exception ex)
                             {
                               var txtError = new LocText("Sobees.Configuration.BGlobals:Resources:errorLinkedIn").ResolveLocalizedValue();
                               MessengerInstance.Send(new BMessage("ShowError", txtError));
                               TraceHelper.Trace(this, ex);
                             }
                           };

          worker.RunWorkerAsync();
        }
      }
    }

    public static string RemoveAccentFromString(string chaine)
    {
      chaine = chaine.Replace("“", "''");
      chaine = chaine.Replace("”", "''");
      var txt = chaine;
      try
      {
        // Déclaration de variables
        var accent = "ÀÁÂÃÄÅàáâãäåÒÓÔÕÖØòóôõöøÈÉÊËèéêëÌÍÎÏìíîïÙÚÛÜùúûüÿÑñÇç";
        var sansAccent = "AAAAAAaaaaaaOOOOOOooooooEEEEeeeeIIIIiiiiUUUUuuuuyNnCc";

        // Conversion des chaines en tableaux de caractères
        var tableauSansAccent = sansAccent.ToCharArray();
        var tableauAccent = accent.ToCharArray();

        // Pour chaque accent
        for (var i = 0; i < accent.Length; i++)
        {
          // Remplacement de l'accent par son équivalent sans accent dans la chaîne de caractères
          chaine = chaine.Replace(tableauAccent[i].ToString(), tableauSansAccent[i].ToString());
        }

        chaine = chaine.Replace("&", " ");
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("", ex);
        return txt;
      }

      // Retour du résultat
      return chaine;
    }

    /// <summary>
    ///   UpdateAll
    /// </summary>
    public override void UpdateAll()
    {
      try
      {
        using (var worker = new BackgroundWorker())
        {
          worker.DoWork += delegate(object s, DoWorkEventArgs args)
                           {
                             if (worker.CancellationPending)
                             {
                               args.Cancel = true;
                               return;
                             }
                             try
                             {
                               NewEntries.Clear();
                               if (!Friends.Any())
                               {
                                 foreach (var friend in LinkedInLibV2.GetConnections().Where(friend => !Friends.Contains(friend)))
                                 {
                                   Friends.Add(friend);
                                 }
                               }
                               foreach (var user in LinkedInLibV2.GetNetworkUpdate(null, Settings.NbPostToGet, 0, 0, 0))
                               {
                                 if (!Entries.Contains(user))
                                 {
                                   var i = 0;
                                   while (Entries.Count > i)
                                   {
                                     if (Entries[i].PubDate < user.PubDate)
                                       break;
                                     i++;
                                   }
                                   if (user.PubDate > Settings.DateLastUpdate)
                                   {
                                     if (Settings.DateLastUpdate != DateTime.MinValue)
                                     {
                                       user.HasBeenViewed = false;
                                       NewEntries.Add(user);
                                     }
                                   }
                                   Entries.Insert(i, user);
                                 }
                                 else
                                 {
                                   var pos = Entries.IndexOf(user);
                                   if (user.Comments != null)
                                   {
                                     if (Entries[pos].Comments == null)
                                     {
                                       Entries.RemoveAt(pos);
                                       Entries.Insert(pos, user);
                                     }
                                     else if (user.Comments.Count != Entries[pos].Comments.Count)
                                     {
                                       foreach (
                                         var comment in
                                           from uComm in user.Comments let findCom = Entries[pos].Comments.Any(eComm => eComm.Body.Equals(uComm.Body)) where !findCom select uComm)
                                       {
                                         Application.Current.Dispatcher.BeginInvoke(new Action(() => Entries[pos].Comments.Add(comment)));
                                       }
                                     }
                                   }
                                   Entries[pos].HasBeenViewed = true;
                                 }
                               }
                               if (Entries.Count > 0)
                               {
                                 Settings.DateLastUpdate = Entries[0].PubDate;
                               }
                               ShowAlerts(NewEntries, Settings.UserName, EnumAccountType.LinkedIn);
                             }
                             catch (Exception ex)
                             {
                               var txtError = new LocText("Sobees.Configuration.BGlobals:Resources:errorLinkedIn").ResolveLocalizedValue();
                               MessengerInstance.Send(new BMessage("ShowError", txtError));
                               TraceHelper.Trace(this, ex);
                             }
                             EndUpdateAll();
                             UpdateView();
                           };

          worker.RunWorkerAsync();
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
        EndUpdateAll();
      }
    }

    public override void UpdateView()
    {
      using (var worker = new BackgroundWorker())
      {
        worker.DoWork += delegate(object s, DoWorkEventArgs args)
                         {
                           if (worker.CancellationPending)
                           {
                             args.Cancel = true;
                             return;
                           }

                           try
                           {
                             if (!IdEntryComment)
                             {
                               var i = 0;

                               //Remove Entries that don't need anymore be show
                               while (i < EntriesDisplay.Count)
                               {
                                 var entry = EntriesDisplay[i];
                                 if (!Entries.Contains(entry) || entry.User != null && entry.User.NickName.ToLower().Contains("private"))
                                   EntriesDisplay.RemoveAt(i);

                                 if ((ShowAPPS && entry.UpdateType == "APPS" ||
                                      ShowCONN && entry.UpdateType == "CONN" ||
                                      ShowJGRP && entry.UpdateType == "JGRP" ||
                                      ShowJOBS && entry.UpdateType == "JOBP" ||
                                      ShowSTAT && entry.UpdateType == "SHAR" ||
                                      ShowOTHER && (entry.UpdateType == "PICU" || entry.UpdateType == "PRFU") ||
                                      ShowPRFU && entry.UpdateType == "PROF" ||
                                      ShowRECU && (entry.UpdateType == "PREC" || entry.UpdateType == "SVPR"))
                                      &&
                                     (string.IsNullOrEmpty(Filter) ||
                                      ((entry.Title != null && entry.Title.ToLower().Contains(Filter.ToLower()) ||
                                      (entry.User != null && entry.User.NickName.ToLower().Contains(Filter.ToLower()))))
                                      
                                      )
                                   )
                                 {
                                   i++;
                                 }
                                 else
                                 {
                                   EntriesDisplay.RemoveAt(i);
                                 }
                               }

                               //EntriesDisplay.Clear();

                               //Add other entries
                               foreach (var entry in Entries)
                               {
                                 if ((ShowAPPS && entry.UpdateType == "APPS" ||
                                      ShowCONN && entry.UpdateType == "CONN" ||
                                      ShowJGRP && entry.UpdateType == "JGRP" ||
                                      ShowJOBS && entry.UpdateType == "JOBP" ||
                                      ShowSTAT && entry.UpdateType == "SHAR" ||
                                      ShowOTHER && (entry.UpdateType == "PICU" || entry.UpdateType == "PRFU") ||
                                      ShowPRFU && entry.UpdateType == "PROF" ||
                                      ShowRECU && (entry.UpdateType == "PREC" || entry.UpdateType == "SVPR"))
                                      &&
                                     (string.IsNullOrEmpty(Filter) ||
                                      ((entry.Title != null && entry.Title.ToLower().Contains(Filter.ToLower()) ||
                                      (entry.User != null && entry.User.NickName.ToLower().Contains(Filter.ToLower()))))
                                      )
                                   )
                                 {
                                   if (EntriesDisplay.Contains(entry)) continue;
                                   if(entry.User != null && entry.User.NickName.ToLower().Contains("private")) continue;
                                   var j = 0;
                                   while (EntriesDisplay.Count > j)
                                   {
                                     if (EntriesDisplay[j].PubDate < entry.PubDate)
                                       break;
                                     j++;
                                   }
                                   EntriesDisplay.Insert(j, entry);
                                 }
                               }
                             }
                             else
                             {
                               //When the update occured after Post a comment.
                               var entryTemp = new Entry();
                               var findEn = false;
                               foreach (var entry in Entries.Where(entry => entry.Id.Equals(IdEntryComment)))
                               {
                                 entryTemp = entry;
                                 findEn = true;
                                 break;
                               }

                               if (findEn)
                               {
                                 var pos = 0;
                                 var find = false;
                                 foreach (var entryDisplay in EntriesDisplay)
                                 {
                                   if (entryDisplay.Id.Equals(IdEntryComment))
                                   {
                                     find = true;
                                     break;
                                   }
                                   pos++;
                                 }

                                 if (find)
                                 {
                                   if (EntriesDisplay[pos].Comments == null)
                                   {
                                     Application.Current.Dispatcher.BeginInvoke(new Action(() => { EntriesDisplay[pos].Comments = entryTemp.Comments; }));
                                   }
                                   else
                                   {
                                     if (EntriesDisplay[pos].Comments.Count > 0 && EntriesDisplay[pos].Comments.Count != entryTemp.Comments.Count)
                                       Application.Current.Dispatcher.BeginInvoke(
                                         new Action(() => EntriesDisplay[pos].Comments.Add(entryTemp.Comments[entryTemp.Comments.Count - 1])));
                                     else
                                     {
                                       if (EntriesDisplay[pos].Comments.Count != entryTemp.Comments.Count)
                                         Application.Current.Dispatcher.BeginInvoke(new Action(() => EntriesDisplay[pos].Comments.Add(entryTemp.Comments[0])));
                                     }
                                   }
                                 }
                               }
                               IdEntryComment = false;
                             }
                             IdEntryComment = false;
                           }
                           catch (Exception ex)
                           {
                             IdEntryComment = false;
                             TraceHelper.Trace(this, ex);
                           }
                           IsAnyDataVisibility = EntriesDisplay.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
                           ////#endif

                           //UpdateNumberPost(EntriesDisplay);
                           UpdateNumberPost(Entries);

                           EndUpdateAll();
                         };
        worker.RunWorkerAsync();
      }
    }

    #endregion Methods

    #region UPDATE NUMBER POST

    private int MaxPost
    {
      get
      {
        if (Settings == null) return 200;
        return Settings.NbMaxPosts > 5 ? Settings.NbMaxPosts : 200;
      }
    }

    private void UpdateNumberPost(ObservableCollection<LinkedInEntry> listEntry)
    {
      try
      {
        if (listEntry == null || listEntry.Count < 1)
          return;

        var max = MaxPost;
        var i = 0;

        while (i < listEntry.Count)
        {
          if (i >= max)
            listEntry.Remove(listEntry[i]);
          else
            i++;
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }

    #endregion UPDATE NUMBER POST
  }
}