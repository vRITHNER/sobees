using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Sobees.Controls.LinkedIn.Cls;
using Sobees.Infrastructure.Controls;
using Sobees.Library.BLinkedInLib;
using Sobees.Library.BLocalizeLib;
using Sobees.Tools.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Markup;
using System.Xml;

namespace Sobees.Controls.LinkedIn.ViewModel
{
  public class SearchViewModel : BLinkedInViewModel
  {
    public SearchViewModel(LinkedInViewModel model, Messenger messenger)
      : base(model, messenger)
    {
      Entries = new DispatcherNotifiedObservableCollection<LinkedInUser>();
      EntriesDisplay = new DispatcherNotifiedObservableCollection<LinkedInUser>();
      InitCommands();
    }

    #region Fields

    private int _numberResult;
    private bool _onlyInNetwork;
    private bool _onlyOutNetwork;

    private string _searchCompagny;
    private string _searchName;
    private string _searchText;

    private bool _useAdvanced;
    private OAuthLinkedInV2.EnumLinkedInSearchSort _selectedSort;
    private LinkedInIndustryCode _selectedIndustry;

    public List<LinkedInIndustryCode> IndustriesCode => LinkedInIndustryCode.GetAllCode();

    public List<OAuthLinkedInV2.EnumLinkedInSearchSort> SortType => new List<OAuthLinkedInV2.EnumLinkedInSearchSort>
    {
      OAuthLinkedInV2.EnumLinkedInSearchSort.ctx,
      OAuthLinkedInV2.EnumLinkedInSearchSort.distance,
      OAuthLinkedInV2.EnumLinkedInSearchSort.endorsers,
      OAuthLinkedInV2.EnumLinkedInSearchSort.relevance
    };

    #endregion Fields

    #region Properties

#if !SILVERLIGHT

    public DispatcherNotifiedObservableCollection<LinkedInUser> Entries { get; set; }

    public DispatcherNotifiedObservableCollection<LinkedInUser> EntriesDisplay { get; set; }

#else
    public ObservableCollection<User> Entries { get; set; }
    public ObservableCollection<User> EntriesDisplay { get; set; }
#endif

    public String StringSearch
    {
      get { return _searchText; }
      set
      {
        _searchText = value;
        RaisePropertyChanged();
      }
    }

    public String SearchCompagny
    {
      get { return _searchCompagny; }
      set
      {
        _searchCompagny = value;
        RaisePropertyChanged();
      }
    }

    public bool UseAdvanced
    {
      get { return _useAdvanced; }
      set
      {
        _useAdvanced = value;

        if (!_useAdvanced)
          SelectedIndustry = null;

        RaisePropertyChanged();
      }
    }

    public bool OnlyInNetwork
    {
      get { return _onlyInNetwork; }
      set
      {
        _onlyInNetwork = value;
        RaisePropertyChanged();
      }
    }

    public bool OnlyOutNetwork
    {
      get { return _onlyOutNetwork; }
      set
      {
        _onlyOutNetwork = value;
        RaisePropertyChanged();
      }
    }

    public int NumberResult
    {
      get
      {
        if (_numberResult < 10)
          return 10;

        return _numberResult;
      }
      set
      {
        _numberResult = value;
        RaisePropertyChanged();
      }
    }

    public OAuthLinkedInV2.EnumLinkedInSearchSort SelectedSort
    {
      get { return _selectedSort; }
      set
      {
        _selectedSort = value;
        RaisePropertyChanged();
      }
    }

    public String SearchName
    {
      get { return _searchName; }
      set
      {
        _searchName = value;
        RaisePropertyChanged();
      }
    }

#if !SILVERLIGHT

    public LinkedInIndustryCode SelectedIndustry
    {
      get { return _selectedIndustry; }
      set
      {
        _selectedIndustry = value;
        RaisePropertyChanged();
      }
    }

#else
    public IndustryCode SelectedIndustry
    {
      get { return _selectedIndustry; }
      set
      {
        _selectedIndustry = value;
        RaisePropertyChanged("SelectedIndustry");
      }
    }
#endif

    public override DataTemplate DataTemplateView
    {
      get
      {
        const string dt = "<DataTemplate x:Name='dtLinkedIn' " +
                          "xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
                          "xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' " +
                          "xmlns:LinkedIn='clr-namespace:Sobees.Controls.LinkedIn.Views;assembly=Sobees.Controls.LinkedIn'>" +
                          "<Grid HorizontalAlignment='Stretch' VerticalAlignment='Stretch'>" +
                          "<LinkedIn:Search />" +
                          "</Grid>" +
                          "</DataTemplate>";

#if SILVERLIGHT
        return XamlReader.Load(dt) as DataTemplate;
#else
        var stringReader = new StringReader(dt);
        XmlReader xmlReader = XmlReader.Create(stringReader);
        return XamlReader.Load(xmlReader) as DataTemplate;
#endif
      }
      set { base.DataTemplateView = value; }
    }

    #endregion Properties

    #region Commands

    public RelayCommand SaveKeywordsCommand { get; private set; }

    #endregion Commands

    #region Methods

    protected override void InitCommands()
    {
      SaveKeywordsCommand = new RelayCommand(Search);
      base.InitCommands();
    }

    private void Search()
    {
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
                             try
                             {
                               Entries.Clear();
                               List<LinkedInUser> results;
                               if (UseAdvanced)
                               {
                                 if (OnlyInNetwork && !OnlyOutNetwork)
                                 {
                                   results = LinkedInLibV2.Search(StringSearch, SearchName, SearchCompagny, false,
                                                                  string.Empty, false, SelectedIndustry,
                                                                  OAuthLinkedInV2.EnumLinkedInSearchNetwork.IN, 0,
                                                                  NumberResult, SelectedSort);
                                 }
                                 else if (OnlyOutNetwork && !OnlyInNetwork)
                                 {
                                   results = LinkedInLibV2.Search(StringSearch, SearchName, SearchCompagny, false,
                                                                  string.Empty, false, SelectedIndustry,
                                                                  OAuthLinkedInV2.EnumLinkedInSearchNetwork.OUT, 0,
                                                                  NumberResult, SelectedSort);
                                 }
                                 else
                                 {
                                   results = LinkedInLibV2.Search(StringSearch, SearchName, SearchCompagny, false,
                                                                  string.Empty, false, SelectedIndustry,
                                                                  OAuthLinkedInV2.EnumLinkedInSearchNetwork.IN, 0,
                                                                  NumberResult, SelectedSort);
                                   if (results == null)
                                   {
                                     results = LinkedInLibV2.Search(StringSearch, SearchName, SearchCompagny, false,
                                                                    string.Empty, false, SelectedIndustry,
                                                                    OAuthLinkedInV2.EnumLinkedInSearchNetwork.OUT, 0,
                                                                    NumberResult, SelectedSort);
                                   }
                                   else
                                   {
                                     foreach (var user in LinkedInLibV2.Search(StringSearch, SearchName, SearchCompagny, false,
                                                                               string.Empty, false, SelectedIndustry,
                                                                               OAuthLinkedInV2.EnumLinkedInSearchNetwork.OUT, 0,
                                                                               NumberResult, SelectedSort).Where(user => !results.Contains(user)))
                                     {
                                       results.Add(user);
                                     }
                                   }
                                 }
                               }
                               else
                               {
                                 results = LinkedInLibV2.Search(StringSearch, null, null, false, null, false, null,
                                                                OAuthLinkedInV2.EnumLinkedInSearchNetwork.IN, 0, 10,
                                                                OAuthLinkedInV2.EnumLinkedInSearchSort.ctx);
                                 if (results == null)
                                 {
                                   results = LinkedInLibV2.Search(StringSearch, null, null, false, null, false, null,
                                                                  OAuthLinkedInV2.EnumLinkedInSearchNetwork.OUT, 0, 10,
                                                                  OAuthLinkedInV2.EnumLinkedInSearchSort.ctx);
                                 }
                                 else
                                 {
                                   foreach (var user in LinkedInLibV2.Search(StringSearch, null, null, false, null, false, null,
                                                                             OAuthLinkedInV2.EnumLinkedInSearchNetwork.OUT, 0,
                                                                             10,
                                                                             OAuthLinkedInV2.EnumLinkedInSearchSort.ctx).Where(user => !results.Contains(user)))
                                   {
                                     results.Add(user);
                                   }
                                 }
                               }

                               foreach (var result in from result in results let find = Entries.Any(en => result.Id.Equals(en.Id)) where !find select result)
                               {
                                 Entries.Add(result);
                               }
                               EndUpdateAll();
                             }
                             catch (Exception ex)
                             {
                               MessengerInstance.Send(new BMessage("ShowError",
                                                                   new LocText("Sobees.Configuration.BGlobals:Resources:errorLinkedIn").ResolveLocalizedValue()));
                               EndUpdateAll();
                               TraceHelper.Trace(this, ex);
                             }
                           };
        worker.RunWorkerAsync();
      }
    }

    #endregion Methods
  }
}