using System;
using System.ComponentModel;
using Sobees.Library.BGenericLib;

namespace Sobees.Controls.TwitterSearch.Cls
{
  public class TwitterSearchWorkspaceSettings : INotifyPropertyChanged
  {
    #region Fields

    private string _searchQuery;

    #endregion

    #region Properties

    public string SearchQuery
    {
      get { return _searchQuery; }
      set
      {
        _searchQuery = value;
        OnPropertyChanged("SearchQuery");
      }
    }

    public DateTime DateLastUpdate { get; set; }

    public int ColumnInGrid { get; set; }
    public double ColumnInGridWidth { get; set; }
    public EnumLanguages Language { get; set; }

    public double RefreshTime { get; set; }

    public int Rpp { get; set; }

    public string GeoCode { get; set; }

    #endregion
    public TwitterSearchWorkspaceSettings()
    {
      
    }
    public TwitterSearchWorkspaceSettings(string searchQuery, int columnInGrid, double columnInGridWidth,
                                          EnumLanguages language, double refreshTime, int rpp, string geoCode)
    {
      SearchQuery = searchQuery;
      ColumnInGrid = columnInGrid;
      ColumnInGridWidth = columnInGridWidth;
      Language = language;
      RefreshTime = refreshTime;
      Rpp = rpp;
      GeoCode = geoCode;
    }

    #region INotifyPropertyChanged Members

    /// <summary>
    /// Raised when a property on this object has a new value.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    #endregion

    /// <summary>
    /// Raises this object's PropertyChanged event.
    /// </summary>
    /// <param name="propertyName">The property that has a new value.</param>
    protected virtual void OnPropertyChanged(string propertyName)
    {
      //this.VerifyPropertyName(propertyName);

      PropertyChangedEventHandler handler = PropertyChanged;
      if (handler != null)
      {
        var e = new PropertyChangedEventArgs(propertyName);
        handler(this, e);
      }
    }
  }
}