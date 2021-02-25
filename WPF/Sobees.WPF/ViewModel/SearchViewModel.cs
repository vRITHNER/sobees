#region

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Sobees.Cls;
using Sobees.Infrastructure.Cls;
using Sobees.Infrastructure.Controls;
using Sobees.Infrastructure.Model;
using Sobees.Infrastructure.ViewModelBase;

#endregion

namespace Sobees.ViewModel
{
  public class SearchViewModel : BServiceWorkspaceViewModel
  {
    private BTemplate _bsearchWorkspaceGridTemplate;
    private ObservableCollection<BServiceWorkspaceViewModel> _bsearchWorkspaces;

    private string _stringSearch = SobeesSettingsLocator.SobeesSettingsStatic.WordSearch;

    public SearchViewModel()
    {
      InitCommands();
      Messenger.Default.Register<BMessage>(this, DoActionMessage);
    }

    public string StringSearch
    {
      get => _stringSearch;
        set
      {
        _stringSearch = value;
        RaisePropertyChanged();
      }
    }

    public BTemplate BSearchWorkspaceGridTemplate
    {
      get
      {
        if (_bsearchWorkspaceGridTemplate == null)
        {
          _bsearchWorkspaceGridTemplate = new BTemplate
                                          {
                                            IsUserSelectedbTemplate = false,
                                            BPositions =
                                              new List<BPosition>
                                              {
                                                new BPosition
                                                {
                                                  Col = 0,
                                                  ColSpan = 1,
                                                  Height = -1,
                                                  HorizontalAlignment = HorizontalAlignment.Stretch,
                                                  Orientation = Orientation.Vertical,
                                                  Row = 0,
                                                  RowSpan = 1,
                                                  UnitType = GridUnitType.Star,
                                                  VerticalAlignment = VerticalAlignment.Stretch,
                                                  Width = -1
                                                },
                                                new BPosition
                                                {
                                                  Col = 1,
                                                  ColSpan = 1,
                                                  Height = -1,
                                                  HorizontalAlignment = HorizontalAlignment.Stretch,
                                                  Orientation = Orientation.Vertical,
                                                  Row = 0,
                                                  RowSpan = 1,
                                                  UnitType = GridUnitType.Star,
                                                  VerticalAlignment = VerticalAlignment.Stretch,
                                                  Width = -1
                                                },
                                                new BPosition
                                                {
                                                  Col = 0,
                                                  ColSpan = 1,
                                                  Height = -1,
                                                  HorizontalAlignment = HorizontalAlignment.Stretch,
                                                  Orientation = Orientation.Vertical,
                                                  Row = 1,
                                                  RowSpan = 1,
                                                  UnitType = GridUnitType.Star,
                                                  VerticalAlignment = VerticalAlignment.Stretch,
                                                  Width = -1
                                                },
                                                new BPosition
                                                {
                                                  Col = 1,
                                                  ColSpan = 1,
                                                  Height = -1,
                                                  HorizontalAlignment = HorizontalAlignment.Stretch,
                                                  Orientation = Orientation.Vertical,
                                                  Row = 1,
                                                  RowSpan = 1,
                                                  UnitType = GridUnitType.Star,
                                                  VerticalAlignment = VerticalAlignment.Stretch,
                                                  Width = -1
                                                }
                                              },
                                            Columns = 2,
                                            Rows = 2,
                                            GridSplitterPositions =
                                              new List<BPosition>
                                              {
                                                new BPosition(0, 0, 1, 1, Orientation.Vertical, VerticalAlignment.Stretch,
                                                              HorizontalAlignment.Right)
                                              }
                                          };
        }
        return _bsearchWorkspaceGridTemplate;
      }
      set
      {
        if (_bsearchWorkspaceGridTemplate == null) _bsearchWorkspaceGridTemplate = new BTemplate();

        _bsearchWorkspaceGridTemplate.BPositions = value.BPositions;
        _bsearchWorkspaceGridTemplate.Columns = value.Columns;
        _bsearchWorkspaceGridTemplate.GridSplitterPositions = value.GridSplitterPositions;
        _bsearchWorkspaceGridTemplate.ImgUrl = value.ImgUrl;
        _bsearchWorkspaceGridTemplate.Rows = value.Rows;
        _bsearchWorkspaceGridTemplate.IsUserSelectedbTemplate = value.IsUserSelectedbTemplate;
        _bsearchWorkspaceGridTemplate.DependencyPropertyDescriptorHack = value.DependencyPropertyDescriptorHack;
        RaisePropertyChanged("BServiceWorkspaceGridTemplate1");
      }
    }

    public ObservableCollection<BServiceWorkspaceViewModel> BSearchWorkspaces
    {
      get
      {
        if (_bsearchWorkspaces == null)
        {
          _bsearchWorkspaces = new ObservableCollection<BServiceWorkspaceViewModel>();
          var result =
            BServiceWorkspaceHelper.LoadServiceWorkspaceBack(
              new BServiceWorkspace {Namespace = "Sobees.Controls.SearchNews.ViewModel", ClassName = "SearchNewsViewModel", Dll = "Sobees.Controls.SearchNews.dll"},
              new BPosition(0, 0, 1, 1), null);
          if (result != null)
          {
            _bsearchWorkspaces.Add(result);
          }

          var result2 =
            BServiceWorkspaceHelper.LoadServiceWorkspaceBack(
              new BServiceWorkspace {Namespace = "Sobees.Controls.SearchWeb.ViewModel", ClassName = "SearchWebViewModel", Dll = "Sobees.Controls.SearchWeb.dll"},
              new BPosition(1, 0, 1, 1), null);
          if (result2 != null)
          {
            _bsearchWorkspaces.Add(result2);
          }

          var result3 =
            BServiceWorkspaceHelper.LoadServiceWorkspaceBack(
              new BServiceWorkspace {Namespace = "Sobees.Controls.SearchPictures.ViewModel", ClassName = "SearchPicturesViewModel", Dll = "Sobees.Controls.SearchPictures.dll"},
              new BPosition(1, 1, 1, 1), null);
          if (result3 != null)
          {
            _bsearchWorkspaces.Add(result3);
          }

          var result4 =
            BServiceWorkspaceHelper.LoadServiceWorkspaceBack(
              new BServiceWorkspace {Namespace = "Sobees.Controls.SearchMovies.ViewModel", ClassName = "SearchMoviesViewModel", Dll = "Sobees.Controls.SearchMovies.dll"},
              new BPosition(0, 1, 1, 1), null);
          if (result4 != null)
          {
            _bsearchWorkspaces.Add(result4);
          }
        }
        return _bsearchWorkspaces;
      }
    }

    public RelayCommand SaveKeywordsCommand { get; set; }

    protected override void InitCommands()
    {
      SaveKeywordsCommand = new RelayCommand(SaveKeywords);
      base.InitCommands();
    }

    public void SaveKeywords()
    {
      if ((StringSearch.ToUpper()).Equals(SobeesSettings.WordSearch.ToUpper()))
        return;

      SobeesSettings.WordSearch = StringSearch;
      Messenger.Default.Send("NewSearchKeyword");
    }

    public override void DoAction(string param)
    {
      base.DoAction(param);
    }

    public override void DoActionMessage(BMessage obj)
    {
      switch (obj.Action)
      {
        case "UpdateSearchPicture":
          SetBackground(obj.Parameter as string);
          break;
      }
      base.DoActionMessage(obj);
    }

    private void SetBackground(string s)
    {
    }
  }
}