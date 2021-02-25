using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Markup;
using System.Xml;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Sobees.Controls.LinkedIn.Cls;
using Sobees.Infrastructure.ViewModelBase;
using Sobees.Library.BLinkedInLib;


namespace Sobees.Controls.LinkedIn.ViewModel
{
  public class ProfileViewModel : BLinkedInViewModel, IBProfileViewModel
  {
    #region Fields

#if !SILVERLIGHT
    private LinkedInUser _CurrentUser;
#else
    private User _CurrentUser;
#endif

    #endregion

    #region Properties

#if !SILVERLIGHT
    public LinkedInUser CurrentUser
    {
      get { return _CurrentUser; }
      set
      {
        _CurrentUser = value;
        RaisePropertyChanged();
      }
    }
#else
    public User CurrentUser
    {
      get { return _CurrentUser; }
      set
      {
        _CurrentUser = value;
        RaisePropertyChanged("CurrentUser");
      }
    }
#endif
    protected string Id { get; set; }

    public DataTemplate Profilcontrol
    {
      get
      {
        var dt =
          "<DataTemplate x:Name='dtGroupsView' " +
          "xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
          "xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' " +
          "xmlns:Views='clr-namespace:Sobees.Controls.LinkedIn.Views;assembly=Sobees.Controls.LinkedIn'>" +
          "<Grid HorizontalAlignment='Stretch' VerticalAlignment='Stretch'>" +
          "<Views:Profile HorizontalAlignment='Stretch' VerticalAlignment='Stretch'/>" +
          "</Grid>" +
          "</DataTemplate>";

#if SILVERLIGHT
        return XamlReader.Load(dt) as DataTemplate;
#else
        var stringReader = new StringReader(dt);
        var xmlReader = XmlReader.Create(stringReader);
        return XamlReader.Load(xmlReader) as DataTemplate;
#endif
      }
    }

    public BWorkspaceViewModel CurrentViewModel => this;

    public override DataTemplate DataTemplateView
    {
      get
      {
        var dt =
          "<DataTemplate x:Name='dtGroupsView' " +
          "xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
          "xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' " +
          "xmlns:Views='clr-namespace:Sobees.Infrastructure.Controls;assembly=Sobees.Infrastructure'>" +
          "<Grid HorizontalAlignment='Stretch' VerticalAlignment='Stretch'>" +
          "<Views:UcProfile HorizontalAlignment='Stretch' VerticalAlignment='Stretch'/>" +
          "</Grid>" +
          "</DataTemplate>";

#if SILVERLIGHT
        return XamlReader.Load(dt) as DataTemplate;
#else
        var stringReader = new StringReader(dt);
        var xmlReader = XmlReader.Create(stringReader);
        return XamlReader.Load(xmlReader) as DataTemplate;
#endif
      }
    }

    #endregion

    public ProfileViewModel(LinkedInViewModel model, Messenger messenger)
      : base(model, messenger)
    {
      InitCommands();
    }

    #region Commands

    public RelayCommand CloseProfileCommand { get; private set; }

    #endregion

    #region Methods

    #region Override

    protected override void InitCommands()
    {
      CloseProfileCommand = new RelayCommand(CloseProfile);
      base.InitCommands();
    }

    public override void UpdateAll()
    {
#if !SILVERLIGHT
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
                             CurrentUser = LinkedInLibV2.GetInfoUser(Id);
                             EndUpdateAll();
                           };

        worker.RunWorkerAsync();
      }
#else
      ProxyLinkedIn.GetInfoUserAsyncCompleted += ProxyGetInfoUserAsyncCompleted;
      ProxyLinkedIn.GetInfoUserAsyncAsync(BGlobals.LINKEDIN_SL_KEY,
                                  BGlobals.LINKEDIN_SL_SECRET,
                                  SobeesSettings.Accounts[SobeesSettings.Accounts.IndexOf(new UserAccount(Settings.UserName, EnumAccountType.LinkedIn))].SessionKey,
                                  SobeesSettings.Accounts[SobeesSettings.Accounts.IndexOf(new UserAccount(Settings.UserName, EnumAccountType.LinkedIn))].Secret,
                                   Id);
#endif
    }

    #endregion

    public void ShowUser(string id)
    {
      foreach (var user in Friends)
      {
        if (user.Id == id)
        {
          CurrentUser = user;
        }
      }
      Id = id;
      Refresh();
    }

    private void CloseProfile()
    {
#if SILVERLIGHT
      ProxyLinkedIn.GetInfoUserAsyncCompleted -= ProxyGetInfoUserAsyncCompleted;
#endif
      MessengerInstance.Send("CloseProfile");
    }

    #endregion

    #region Callback SL

#if SILVERLIGHT
    void ProxyGetInfoUserAsyncCompleted(object sender, GetInfoUserAsyncCompletedEventArgs e)
    {
      if (e.Error == null && e.Result != null)
      {
        Deployment.Current.Dispatcher.BeginInvoke(() =>
        {
          CurrentUser = e.Result;
        });
        EndUpdateAll();
      }
      else
      {
        EndUpdateAll();
        if (e.Error != null) TraceHelper.Trace(this, e.Error);
      }
    }
#endif

    #endregion
  }
}