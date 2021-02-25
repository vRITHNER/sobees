#region

using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Markup;
using System.Xml;
using GalaSoft.MvvmLight.Messaging;
using Sobees.Controls.LinkedIn.Cls;
using Sobees.Infrastructure.Controls;
using Sobees.Library.BLinkedInLib;
using Sobees.Tools.Logging;

#endregion

namespace Sobees.Controls.LinkedIn.ViewModel
{
  public class FriendsViewModel : BLinkedInViewModel
  {
    #region Fields

    private string _stringSearch;

    #endregion

    #region Properties

    public ObservableCollection<LinkedInUser> FriendsDisplayTemp { get; set; }
    public DispatcherNotifiedObservableCollection<LinkedInUser> FriendsDisplay { get; set; }

    #endregion

    public string StringSearch
    {
      get { return _stringSearch; }
      set
      {
        _stringSearch = value;
        RaisePropertyChanged();
      }
    }

    public override DataTemplate DataTemplateView
    {
      get
      {
        const string dt =
          "<DataTemplate x:Name='dtLinkedIn' xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' xmlns:LinkedIn='clr-namespace:Sobees.Controls.LinkedIn.Views;assembly=Sobees.Controls.LinkedIn'><Grid HorizontalAlignment='Stretch' VerticalAlignment='Stretch'><LinkedIn:Friends /></Grid></DataTemplate>";

        var stringReader = new StringReader(dt);
        var xmlReader = XmlReader.Create(stringReader);
        return XamlReader.Load(xmlReader) as DataTemplate;
      }
      set { base.DataTemplateView = value; }
    }

    #region Constructors

    public FriendsViewModel(LinkedInViewModel model, Messenger messenger) : base(model, messenger)
    {
      FriendsDisplay = new DispatcherNotifiedObservableCollection<LinkedInUser>();
      FriendsDisplayTemp = new ObservableCollection<LinkedInUser>();

      Refresh();
    }

    #endregion

    #region Commands

    #endregion

    #region Methods

    public override void UpdateAll()
    {
      FriendsDisplay.Clear();
      foreach (var e in Friends)
        FriendsDisplay.Add(e);

      UpdateView();
    }

    public override void UpdateView()
    {
      try
      {
        FriendsDisplayTemp.Clear();
        foreach (var e in
          FriendsDisplay.Where(
            e =>
            (!string.IsNullOrEmpty(StringSearch) && e.NickName.ToUpper().Contains(StringSearch.ToUpper()) || string.IsNullOrEmpty(StringSearch)) &&
            (!string.IsNullOrEmpty(SobeesSettings.Filter) && e.NickName.ToUpper().Contains(SobeesSettings.Filter.ToUpper()) || string.IsNullOrEmpty(SobeesSettings.Filter))))
        {
          FriendsDisplayTemp.Add(e);
        }

        FriendsDisplay.Clear();
        foreach (var e in FriendsDisplayTemp)
          FriendsDisplay.Add(e);

        IsAnyDataVisibility = FriendsDisplay.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
        EndUpdateAll();
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
        EndUpdateAll();
      }
    }

    #endregion
  }
}