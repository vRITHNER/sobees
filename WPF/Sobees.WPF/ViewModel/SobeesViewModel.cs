#region

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight.Messaging;
using Sobees.Configuration.BGlobals;
using Sobees.Infrastructure.Cache;
using Sobees.Infrastructure.Commands;
using Sobees.Infrastructure.Controls;
using Sobees.Infrastructure.Model;
using Sobees.Infrastructure.ViewModelBase;
using Sobees.Library.BLocalizeLib;
using Sobees.Settings;
using Sobees.Themes;
using Sobees.Tools.Helpers;
using Sobees.Tools.Logging;
using Sobees.Tools.Threading;
using Sobees.Tools.Threading.Extensions;

#endregion

namespace Sobees.ViewModel
{
  public class SobeesViewModel : BWorkspaceViewModel
  {
    private ObservableCollection<BServiceWorkspaceViewModel> _bserviceWorkspaces1;
    private ObservableCollection<BServiceWorkspaceViewModel> _bserviceWorkspaces2;

    public SobeesViewModel()
    {
      ChangeLanguage(string.Empty);

      if (IsInDesignMode)
      {
      }
      else
      {
        Application.Current.Exit += CurrentExit;

        Messenger.Default.Register<string>(this, DoAction);
        Messenger.Default.Register<BMessage>(this, DoActionMessage);
        InitCommands();

        Refresh();
      }
    }

    #region Service Workspaces

    /// <summary>
    ///   Returns the collection of available workspaces to display.
    ///   A 'workspace' is a ViewModel that can request to be closed.
    /// </summary>
    public ObservableCollection<BServiceWorkspaceViewModel> BServiceWorkspaces1 => _bserviceWorkspaces1 ?? (_bserviceWorkspaces1 = new ObservableCollection<BServiceWorkspaceViewModel>());

    /// <summary>
    ///   Returns the collection of available workspaces to display.
    ///   A 'workspace' is a ViewModel that can request to be closed.
    /// </summary>
    public ObservableCollection<BServiceWorkspaceViewModel> BServiceWorkspaces2 => _bserviceWorkspaces2 ?? (_bserviceWorkspaces2 = new ObservableCollection<BServiceWorkspaceViewModel>());

    #endregion

    #region Private Helpers

    #endregion

    #region Fields

    private IconViewModel _iconViewModel;

    #endregion

    #region Properties

    public IconViewModel IconViewModel
    {
      get => _iconViewModel ?? (_iconViewModel = new IconViewModel());
        set => _iconViewModel = value;
    }

    #endregion

    #region Methods

    private void CurrentExit(object sender, EventArgs e)
    {
      SaveSettings();
    }

    private void SaveSettings()
    {
      //TraceHelper.Trace(this, "SobeesViewModel::SaveSettings");
      BCommandManager.ClearCommands();
      var task = Task.Factory.StartNew(Settings.SaveSettings);
      task.Wait();
    }

    /// <summary>
    ///   This method is called when a service is created.
    /// </summary>
    /// <param name="model">The new service created</param>
    /// <param name="slvm">The sourceloader qho created the service. This service must be closed and remove from the serviceWorkspace</param>
    public void ServiceLoaded(BServiceWorkspaceViewModel model, SourceLoaderViewModel slvm)
    {
      BServiceWorkspaces1.Add(model);
      if (slvm != null)
      {
        BServiceWorkspaces1.Remove(slvm);
      }
    }

    private async void RestoreSettings()
    {
      if (Settings == null)
        Settings = new BSettings();

      var task = Task.Factory.StartNew(Settings.RestoreSettings);
      task.Wait();
    }

    private void ServiceClosed()
    {
      var current = 0;
      while (current < BServiceWorkspaces1.Count)
      {
        var model = BServiceWorkspaces1[current];
        if (!model.IsClosed)
        {
          current++;
          continue;
        }
        var oldPosition = model.PositionInGrid;
        if (model.IsMaxiMize)
        {
          model.IsMaxiMize = false;
          Messenger.Default.Send("Minimize");
        }
        BServiceWorkspaces1.Remove(model);
        if (model.IsServiceWorkspace && !BViewModelLocator.ViewsManagerViewModelStatic.IsChangeTemplate)
        {
          if (BServiceWorkspaces1.Count == 0)
          {
            //If the last service is removed, we add a serviceloader
            AddSourceLoader1();
          }
          else
          {
            // If we have a template with more than 1 row & column, we replace the ServiceWorkspaceViewModel with a SourceLoaderViewModel
            if (BViewModelLocator.ViewsManagerViewModelStatic.BServiceWorkspaceGridTemplate1.Rows > 1 &&
                BViewModelLocator.ViewsManagerViewModelStatic.BServiceWorkspaceGridTemplate1.Columns > 1)
            {
              AddSourceLoader1();
            }
            else
            {
              //We must remove the position in grid and reorder the service.
              //If we have only one row
              if (BViewModelLocator.ViewsManagerViewModelStatic.BServiceWorkspaceGridTemplate1.Rows == 1)
              {
                //remove the last columns
                BViewModelLocator.ViewsManagerViewModelStatic.BServiceWorkspaceGridTemplate1.Columns--;

                //remove the BPosition that was in the last column
                foreach (
                  var position in
                    BViewModelLocator.ViewsManagerViewModelStatic.BServiceWorkspaceGridTemplate1.BPositions)
                {
                  if (position.Col !=
                      BViewModelLocator.ViewsManagerViewModelStatic.BServiceWorkspaceGridTemplate1.Columns)
                    continue;
                  BViewModelLocator.ViewsManagerViewModelStatic.BServiceWorkspaceGridTemplate1.BPositions.Remove(
                    position);
                  break;
                }

                //remove the gridspliter
                foreach (
                  var position in
                    BViewModelLocator.ViewsManagerViewModelStatic.BServiceWorkspaceGridTemplate1.GridSplitterPositions)
                {
                  if (position.Col !=
                      BViewModelLocator.ViewsManagerViewModelStatic.BServiceWorkspaceGridTemplate1.Columns - 1)
                    continue;
                  BViewModelLocator.ViewsManagerViewModelStatic.BServiceWorkspaceGridTemplate1.GridSplitterPositions.
                                    Remove(
                                      position);
                  break;
                }

                //Reorganise the services
                foreach (var viewModel in BServiceWorkspaces1)
                {
                  if (oldPosition.Col < viewModel.PositionInGrid.Col)
                  {
                    foreach (
                      var position in
                        BViewModelLocator.ViewsManagerViewModelStatic.BServiceWorkspaceGridTemplate1.BPositions)
                    {
                      if (position.Col == viewModel.PositionInGrid.Col - 1)
                      {
                        viewModel.PositionInGrid = position;
                      }
                    }
                  }
                }
              }

                //We must remove the position in grid and reorder the service.
                //If we have only one column
              else if (BViewModelLocator.ViewsManagerViewModelStatic.BServiceWorkspaceGridTemplate1.Columns == 1)
              {
                //remove the last rows
                BViewModelLocator.ViewsManagerViewModelStatic.BServiceWorkspaceGridTemplate1.Rows--;

                //remove the BPosition that was in the last row
                foreach (
                  var position in
                    BViewModelLocator.ViewsManagerViewModelStatic.BServiceWorkspaceGridTemplate1.BPositions)
                {
                  if (position.Row != BViewModelLocator.ViewsManagerViewModelStatic.BServiceWorkspaceGridTemplate1.Rows)
                    continue;
                  BViewModelLocator.ViewsManagerViewModelStatic.BServiceWorkspaceGridTemplate1.BPositions.Remove(
                    position);
                  break;
                }

                //remove the gridspliter
                foreach (
                  var position in
                    BViewModelLocator.ViewsManagerViewModelStatic.BServiceWorkspaceGridTemplate1.GridSplitterPositions)
                {
                  if (position.Row !=
                      BViewModelLocator.ViewsManagerViewModelStatic.BServiceWorkspaceGridTemplate1.Rows - 1)
                    continue;
                  BViewModelLocator.ViewsManagerViewModelStatic.BServiceWorkspaceGridTemplate1.GridSplitterPositions.
                                    Remove(
                                      position);
                  break;
                }

                //Reorganise the services
                foreach (var viewModel in BServiceWorkspaces1)
                {
                  if (oldPosition.Row < viewModel.PositionInGrid.Row)
                  {
                    foreach (
                      var position in
                        BViewModelLocator.ViewsManagerViewModelStatic.BServiceWorkspaceGridTemplate1.BPositions)
                    {
                      if (position.Row == viewModel.PositionInGrid.Row - 1)
                      {
                        viewModel.PositionInGrid = position;
                      }
                    }
                  }
                }
              }
              else if (BViewModelLocator.ViewsManagerViewModelStatic.BServiceWorkspaceGridTemplate1.Columns ==
                       oldPosition.ColSpan)
              {
                //remove a row
                BViewModelLocator.ViewsManagerViewModelStatic.BServiceWorkspaceGridTemplate1.Rows--;

                //remove the BPosition
                BViewModelLocator.ViewsManagerViewModelStatic.BServiceWorkspaceGridTemplate1.BPositions.Remove(
                  oldPosition);

                //Reajust BPosition
                foreach (
                  var position in
                    BViewModelLocator.ViewsManagerViewModelStatic.BServiceWorkspaceGridTemplate1.BPositions)
                {
                  if (position.Row > oldPosition.Row)
                  {
                    position.Row--;
                  }
                }

                //remove the gridspliters
                var i = 0;
                while (i <
                       BViewModelLocator.ViewsManagerViewModelStatic.BServiceWorkspaceGridTemplate1.
                                         GridSplitterPositions.Count)
                {
                  if (
                    BViewModelLocator.ViewsManagerViewModelStatic.BServiceWorkspaceGridTemplate1.GridSplitterPositions
                      [i].Row != BViewModelLocator.ViewsManagerViewModelStatic.BServiceWorkspaceGridTemplate1.Rows - 1)
                  {
                    i++;
                    continue;
                  }
                  BViewModelLocator.ViewsManagerViewModelStatic.BServiceWorkspaceGridTemplate1.GridSplitterPositions.
                                    Remove(
                                      BViewModelLocator.ViewsManagerViewModelStatic.BServiceWorkspaceGridTemplate1.
                                                        GridSplitterPositions[i]);
                }
              }
              else if (BViewModelLocator.ViewsManagerViewModelStatic.BServiceWorkspaceGridTemplate1.Rows ==
                       oldPosition.RowSpan)
              {
                //remove a row
                BViewModelLocator.ViewsManagerViewModelStatic.BServiceWorkspaceGridTemplate1.Columns--;

                //remove the BPosition
                BViewModelLocator.ViewsManagerViewModelStatic.BServiceWorkspaceGridTemplate1.BPositions.Remove(
                  oldPosition);

                //Reajust BPosition
                foreach (
                  var position in
                    BViewModelLocator.ViewsManagerViewModelStatic.BServiceWorkspaceGridTemplate1.BPositions)
                {
                  if (position.Col > oldPosition.Col)
                  {
                    position.Col--;
                  }
                }

                //remove the gridspliters
                var i = 0;
                while (i <
                       BViewModelLocator.ViewsManagerViewModelStatic.BServiceWorkspaceGridTemplate1.
                                         GridSplitterPositions.Count)
                {
                  if (
                    BViewModelLocator.ViewsManagerViewModelStatic.BServiceWorkspaceGridTemplate1.
                                      GridSplitterPositions[i].Col !=
                    BViewModelLocator.ViewsManagerViewModelStatic.BServiceWorkspaceGridTemplate1.Columns - 1)
                  {
                    i++;
                    continue;
                  }
                  BViewModelLocator.ViewsManagerViewModelStatic.BServiceWorkspaceGridTemplate1.GridSplitterPositions
                                   .Remove(
                                     BViewModelLocator.ViewsManagerViewModelStatic.BServiceWorkspaceGridTemplate1.
                                                       GridSplitterPositions[i]);
                }
              }
              else
              {
                //remove the BPosition
                BViewModelLocator.ViewsManagerViewModelStatic.BServiceWorkspaceGridTemplate1.BPositions.Remove(
                  oldPosition);

                //Reajust BPosition
                foreach (
                  var position in
                    BViewModelLocator.ViewsManagerViewModelStatic.BServiceWorkspaceGridTemplate1.BPositions)
                {
                  if ((position.ColSpan == oldPosition.ColSpan && position.RowSpan == oldPosition.RowSpan
                       &&
                       (position.Row == oldPosition.Row + 1 || position.Row == oldPosition.Row - 1 ||
                        position.Col == oldPosition.Col - 1 || position.Col == oldPosition.Col + 1))
                      ||
                      (position.Row == oldPosition.Row && position.RowSpan == oldPosition.RowSpan &&
                       (position.Col == oldPosition.Col + position.ColSpan ||
                        position.Col == oldPosition.Col - position.ColSpan))
                      ||
                      (position.Col == oldPosition.Col && position.ColSpan == oldPosition.ColSpan &&
                       (position.Row == oldPosition.Row + position.RowSpan ||
                        position.Row == oldPosition.Row - position.RowSpan)))
                  {
                    //Reorganise the services
                    BServiceWorkspaceViewModel currentModel = null;
                    foreach (var viewModel in BServiceWorkspaces1)
                    {
                      if (position.Col == viewModel.PositionInGrid.Col
                          && position.ColSpan == viewModel.PositionInGrid.ColSpan
                          && position.Row == viewModel.PositionInGrid.Row
                          && position.RowSpan == viewModel.PositionInGrid.RowSpan)
                      {
                        currentModel = viewModel;
                      }
                    }
                    if (position.Row == oldPosition.Row + position.RowSpan)
                    {
                      position.Row--;
                      position.RowSpan++;
                    }
                    if (position.Row == oldPosition.Row - position.RowSpan)
                    {
                      position.RowSpan++;
                    }
                    if (position.Col == oldPosition.Col + position.ColSpan)
                    {
                      position.Col--;
                      position.ColSpan++;
                    }
                    if (position.Col == oldPosition.Col - position.ColSpan)
                    {
                      position.ColSpan++;
                    }
                    if (currentModel != null) currentModel.PositionInGrid = position;
                    break;
                  }
                }

                //remove the gridspliters
                var i = 0;
                while (i <
                       BViewModelLocator.ViewsManagerViewModelStatic.BServiceWorkspaceGridTemplate1.
                                         GridSplitterPositions.Count)
                {
                  if (
                    BViewModelLocator.ViewsManagerViewModelStatic.BServiceWorkspaceGridTemplate1.
                                      GridSplitterPositions[i].Col == oldPosition.Col
                    &&
                    BViewModelLocator.ViewsManagerViewModelStatic.BServiceWorkspaceGridTemplate1.
                                      GridSplitterPositions[i].Row == oldPosition.Row)
                  {
                    i++;
                    continue;
                  }
                  BViewModelLocator.ViewsManagerViewModelStatic.BServiceWorkspaceGridTemplate1.GridSplitterPositions
                                   .Remove(
                                     BViewModelLocator.ViewsManagerViewModelStatic.BServiceWorkspaceGridTemplate1.
                                                       GridSplitterPositions[i]);
                }

                //AddSourceLoader1();
              }
            }
          }
          BViewModelLocator.ViewsManagerViewModelStatic.UpdateView();
          model.Cleanup();

          if (BServiceWorkspaces1.Count == 1)
            BViewModelLocator.ViewsManagerViewModelStatic.SelectedServiceIndex = 0;

          AddSourceLoader1();
          return;
        }
        current++;
      }
      AddSourceLoader1();
      if (BServiceWorkspaces1.Count == 1)
        BViewModelLocator.ViewsManagerViewModelStatic.SelectedServiceIndex = 0;

      SaveSettings();
    }

    public void DoActionMessage(BMessage message)
    {
      switch (message.Action)
      {
        case "ChangeTheme":
          ChangeTheme(message.Parameter as string);
          break;

        case "DisplayPopupFb":
          DisplayPopupFb(message.Parameter as string);
          break;

        case "DisplayPopupTw":
          DisplayPopupTw(message.Parameter as string);
          break;
      }
    }

    public override void DoAction(string param)
    {
      switch (param)
      {
        case "WindowLoaded":
          RestoreSettings();
          break;

        case "NoSettingsRestored":
          //RestoreSettings();
          break;

        case "SettingsRestored":
          OnGetSettingsCompleted();
          break;

        case "SaveSettings":
          SaveSettings();
          break;

        case "CloseSaveSettings":
          SaveSobeesSettings();
          break;

        case "ClearCacheAll":
          ClearCacheAll();
          break;

        case "ClearCache":
          ClearCache();
          break;

        case "SendLog":
          SendLog();
          break;

        case "ServiceClosed":
          ServiceClosed();
          break;

        case "PopupClosed":
          PopupCloseCommand.Execute(null);
          break;
      }
      base.DoAction(param);
    }

    private void DisplayPopupFb(string message)
    {
      PopupMessageContentFb = message;
      PopupOpenCommand.Execute(null);
    }

    private void DisplayPopupTw(string message)
    {
      PopupMessageContentTw = message;
      PopupOpenCommand.Execute(null);
    }

    /// <summary>
    ///   SaveSobeesSettings
    /// </summary>
    private void SaveSobeesSettings()
    {
      //SobeesSettings.Accounts = BViewModelLocator.SettingsViewModelStatic.Accounts;
      SobeesSettings.UrlShortener = BViewModelLocator.SettingsViewModelStatic.URLShortener;
      SobeesSettings.CloseBoxPublication = BViewModelLocator.SettingsViewModelStatic.CloseBoxPublication;
      SobeesSettings.IsEnabledProxy = BViewModelLocator.SettingsViewModelStatic.UseProxyServer;
      if (BViewModelLocator.SettingsViewModelStatic.ProxyPassword != null)
      {
        SobeesSettings.ProxyPassword = BViewModelLocator.SettingsViewModelStatic.ProxyPassword;
      }

      if (BViewModelLocator.SettingsViewModelStatic.BitLyPassword != null)
      {
        SobeesSettings.BitLyPassword = BViewModelLocator.SettingsViewModelStatic.BitLyPassword;
      }
      SobeesSettings.AlertsEnabled = BViewModelLocator.SettingsViewModelStatic.AlertsEnabled;
      SobeesSettings.BitLyUserName = BViewModelLocator.SettingsViewModelStatic.BitLyUserName;
      SobeesSettings.IsSendByEnter = BViewModelLocator.SettingsViewModelStatic.IsSendByEnter;
      SobeesSettings.ProxyServer = BViewModelLocator.SettingsViewModelStatic.ProxyServerName;
      SobeesSettings.ProxyPort = BViewModelLocator.SettingsViewModelStatic.ProxyPort;
      SobeesSettings.ProxyUserDomain = BViewModelLocator.SettingsViewModelStatic.ProxyUserDomain;
      SobeesSettings.ProxyUserName = BViewModelLocator.SettingsViewModelStatic.ProxyUserName;

      //Account
      if (BViewModelLocator.SettingsViewModelStatic.Accounts != null)
      {
        foreach (var account in BViewModelLocator.SettingsViewModelStatic.Accounts)
        {
          foreach (var userAccount in SobeesSettings.Accounts)
          {
            if (account.Login == userAccount.Login && account.Type == userAccount.Type)
            {
              userAccount.AlertsRemovedWordsList.Clear();
              userAccount.AlertsRemovedWordsList.AddRange(account.AlertsRemovedWordsList);
              userAccount.AlertsUsersList.Clear();
              userAccount.AlertsUsersList.AddRange(account.AlertsUsersList);
              userAccount.AlertsWordsList.Clear();
              userAccount.AlertsWordsList.AddRange(account.AlertsWordsList);
              userAccount.IsSignatureActivated = account.IsSignatureActivated;
              userAccount.IsSpellCheckActivated = account.IsSpellCheckActivated;
              userAccount.IsStatusClosedOk = account.IsStatusClosedOk;
              userAccount.NbMaxPosts = account.NbMaxPosts;
              userAccount.NbPostToGet = account.NbPostToGet;
              userAccount.PasswordHash = account.PasswordHash;
              userAccount.PictureUrl = account.PictureUrl;
              userAccount.Signature = account.Signature;
              userAccount.SpamList.Clear();
              userAccount.SpamList.AddRange(account.SpamList);
              userAccount.IsCheckedUseAlertsDM = account.IsCheckedUseAlertsDM;
              userAccount.IsCheckedUseAlertsFriends = account.IsCheckedUseAlertsFriends;
              userAccount.IsCheckedUseAlertsGroups = account.IsCheckedUseAlertsGroups;
              userAccount.IsCheckedUseAlertsRemovedWords = account.IsCheckedUseAlertsRemovedWords;
              userAccount.IsCheckedUseAlertsReply = account.IsCheckedUseAlertsReply;
              userAccount.IsCheckedUseAlertsUsers = account.IsCheckedUseAlertsUsers;
              userAccount.IsCheckedUseAlertsWords = account.IsCheckedUseAlertsWords;
              userAccount.TypeAlertsFB = account.TypeAlertsFB;
            }
          }
        }
        var i = 0;
        while (i < SobeesSettings.Accounts.Count)
        {
          var find = false;
          foreach (var userAccount in BViewModelLocator.SettingsViewModelStatic.Accounts)
          {
            if (SobeesSettings.Accounts[i].Login == userAccount.Login && SobeesSettings.Accounts[i].Type == userAccount.Type)
              find = true;
          }

          if (!find)
          {
            var account = SobeesSettings.Accounts[i];
            SobeesSettings.Accounts.RemoveAt(i);
            Messenger.Default.Send(new BMessage("DeleteUser", account));
          }
          else
          {
            i++;
          }
        }

        //services
      }
      SobeesSettings.ShowGlobalFilter = BViewModelLocator.SettingsViewModelStatic.ShowGlobalFilter;
      SobeesSettings.DisableAds = BViewModelLocator.SettingsViewModelStatic.DisableAds;
      SobeesSettings.FontSizeValue = BViewModelLocator.SettingsViewModelStatic.FontSizeValue;

      ((SobeesApplication) Application.Current).FontSize = SobeesSettings.FontSizeValue;

      SobeesSettings.MinimizeWindowInTray = BViewModelLocator.SettingsViewModelStatic.MinimizeWindowInTray;
      BViewModelLocator.ViewsManagerViewModelStatic.IsGlobalFilterVisible = SobeesSettings.ShowGlobalFilter ? Visibility.Visible : Visibility.Collapsed;

      SobeesSettings.Theme = BThemeHelper.CurrentTheme;

      if (SobeesSettings.RunAtStartup != BViewModelLocator.SettingsViewModelStatic.RunAtStartup)
      {
        SobeesSettings.RunAtStartup = BViewModelLocator.SettingsViewModelStatic.RunAtStartup;
        BShortcutHelper.SetStartupShortcut(SobeesSettings.RunAtStartup);
      }

      // LANGUAGE //TODO lang
      if (BViewModelLocator.SettingsViewModelStatic.SelectedLanguage.ShortName != SobeesSettings.Language)
      {
        ChangeLanguage(BViewModelLocator.SettingsViewModelStatic.SelectedLanguage.ShortName);
      }
      if (BViewModelLocator.SettingsViewModelStatic.ViewState != !SobeesSettings.ViewState)
      {
        OrderServices();
        SobeesSettings.ViewState = !BViewModelLocator.SettingsViewModelStatic.ViewState;
        Messenger.Default.Send("ViewStateChanged");
      }

      SobeesSettings.LanguageTranslator = BViewModelLocator.SettingsViewModelStatic.CurrentLanguageTraductor;

      BViewModelLocator.SettingsViewModelStatic.CloseCommand.Execute(null);
      Settings.SaveBackupSettings();
      Settings.SaveSettings();
      SobeesSettings.IsFirstLaunch = false;

      Messenger.Default.Send("CloseSettings");
      Messenger.Default.Send("SettingsUpdated");
    }

    private void OrderServices()
    {
      var temp = new ObservableCollection<BServiceWorkspaceViewModel>(BServiceWorkspaces1);

      BServiceWorkspaces1.Clear();

      while (temp.Any())
      {
        var i = 0;
        while (i < BServiceWorkspaces1.Count)
        {
          if (BServiceWorkspaces1[i].PositionInGrid.Col > temp[0].PositionInGrid.Col)
            break;

          i++;
        }
        BServiceWorkspaces1.Insert(i, temp[0]);
        temp.RemoveAt(0);
      }
    }

    private void ClearCache()
    {
      Task.Factory.StartNew(BCacheImage.WebGetter.CleanCache);

      //using (var worker = new BackgroundWorker())
      //{
      //  worker.DoWork += delegate(object s,
      //                            DoWorkEventArgs args)
      //                     {
      //                       if (worker.CancellationPending)
      //                       {
      //                         args.Cancel = true;
      //                         return;
      //                       }
      //                      ;
      //                     };

      //  worker.RunWorkerAsync();
    }

    private void ClearCacheAll()
    {
      Task.Factory.StartNew(BCacheImage.WebGetter.CleanCacheAll);

      //using (var worker = new BackgroundWorker())
      //{
      //  worker.DoWork += delegate(object s,
      //                            DoWorkEventArgs args)
      //                     {
      //                       if (worker.CancellationPending)
      //                       {
      //                         args.Cancel = true;
      //                         return;
      //                       }
      //                       BCacheImage.WebGetter.CleanCacheAll();
      //                     };
      //  worker.RunWorkerAsync();
      //}
    }

    private void SendLog()
    {
      //Send Log by mail to us

      //using (var worker = new BackgroundWorker())
      //{
      //  worker.DoWork += delegate(object s,
      //                            DoWorkEventArgs args)
      //                     {
      //                       if (worker.CancellationPending)
      //                       {
      //                         args.Cancel = true;
      //                         return;
      //                       }

      //                       //Preparae mail
      //                     };
      //  worker.RunWorkerAsync();
      //}
    }

    private void OnGetSettingsCompleted()
    {
      //Send Log by mail to us
      using (var worker = new BackgroundWorker())
      {
        worker.DoWork += delegate(object s,
                                  DoWorkEventArgs args)
                           {
                             if (worker.CancellationPending)
                             {
                               args.Cancel = true;
                               return;
                             }

                             //Preparae mail
                             MainWindow.GetInstance.Dispatcher.
                                        BeginInvokeIfRequired(() =>
                                                                {
                                                                  // Apply saved theme
                                                                  ChangeTheme(SobeesSettings.Theme);

                                                                  //Apply savedLanguage
                                                                  ChangeLanguage(SobeesSettings.Language);

                                                                  // If there's a saved template, restore it
                                                                  if (BViewModelLocator.ViewsManagerViewModelStatic.BServiceWorkspaceGridTemplate1 != null)
                                                                  {
                                                                    //TraceHelper.Trace(this,"ServiceWorkspaceGridTemplate was restored, we get now GetEmptyPositions()!");
                                                                    AddSourceLoader1();
                                                                  }
                                                                  if (BViewModelLocator.ViewsManagerViewModelStatic.BServiceWorkspaceGridTemplate2 != null)
                                                                  {
                                                                    //TraceHelper.Trace(this, "ServiceWorkspaceGridTemplate was restored, we get now GetEmptyPositions()!");
                                                                    AddSourceLoader2();
                                                                  }
                                                                  Messenger.Default.Send("ViewStateChanged");
                                                                  ((SobeesApplication) (Application.Current)).FontSize = SobeesSettings.FontSizeValue;
                                                                });
                           };
        worker.RunWorkerAsync();
      }
    }

    private void AddSourceLoader2()
    {
      var emptyPositions = GetEmptyPositions(BViewModelLocator.ViewsManagerViewModelStatic.BServiceWorkspaceGridTemplate2);
      if (emptyPositions == null) return;

      // Get empty spaces in the loaded template and add SourceLoaders in it
      //TraceHelper.Trace(this, "Foreach empty positions, we add SourceLoaders");

      foreach (var emptyPosition in emptyPositions)
      {
        //TraceHelper.Trace(this, emptyPosition.ToString());
        BServiceWorkspaces2.Add(new SourceLoaderViewModel(emptyPosition, null));
      }
    }

    private void AddSourceLoader1()
    {
      if (BViewModelLocator.ViewsManagerViewModelStatic.IsChangeTemplate) return;

      var emptyPositions = GetEmptyPositions(BViewModelLocator.ViewsManagerViewModelStatic.BServiceWorkspaceGridTemplate1);
      if (emptyPositions == null) return;

      // Get empty spaces in the loaded template and add SourceLoaders in it
      //TraceHelper.Trace(this, "Foreach empty positions, we add SourceLoaders");

      foreach (var emptyPosition in emptyPositions)
      {
        //TraceHelper.Trace(this, emptyPosition.ToString());
        BServiceWorkspaces1.Add(new SourceLoaderViewModel(emptyPosition, null));
      }
    }

    public List<BPosition> GetEmptyPositions(BTemplate template1)
    {
      try
      {
        var positions = new List<BPosition>();
        foreach (var position in template1.BPositions)
        {
          var emptySpace = true;
          var position1 = position;
          foreach (var serviceWorkspace in BServiceWorkspaces1.Where(serviceWorkspace => serviceWorkspace.PositionInGrid.Equals(position1)))
          {
            emptySpace = false;
          }
          if (emptySpace)
            positions.Add(position);
        }
        return positions;
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this,
                          ex);
      }
      return null;
    }

    public void ChangeTheme(string themeName)
    {
      BThemeHelper.ApplyTheme(themeName);
    }

    public void ChangeLanguage(string language)
    {
      try
      {
        if (string.IsNullOrEmpty(language))
        {
          Thread.CurrentThread.CurrentUICulture = new CultureInfo(CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
          BLocalizeDictionary.Instance.Culture = Thread.CurrentThread.CurrentUICulture;
        }
        else
        {
          if (!SobeesSettings.Language.Equals(language))
          {
            SobeesSettings.Language = language;
          }
          Thread.CurrentThread.CurrentUICulture = new CultureInfo(SobeesSettings.Language);
          BLocalizeDictionary.Instance.Culture = new CultureInfo(SobeesSettings.Language);
        }
      }
      catch (Exception e)
      {
        TraceHelper.Trace(this, e);
      }
    }

    #region Timer

    public override double GetRefreshTime()
    {
      return BGlobals.DEFAULT_TIMER_CLEANCOMMAND;
    }

    public override void UpdateAll()
    {
      try
      {
        ProcessHelper.PerformAggressiveCleanup();
        BCommandManager.ClearCommands();
        //SynchronizationHelper.GetLastUpdate();
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
      }
    }

    #endregion

    #endregion

    #region Commands

    #endregion

    public BSettings Settings { get; set; }
  }
}