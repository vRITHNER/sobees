#region

using System;
using System.Collections.Generic;
using System.Windows.Media;
using Sobees.Library.BTwitterLib;

#endregion

namespace Sobees.Controls.Twitter.Cls
{
  public class TwitterWorkspaceSettings
  {
    public TwitterWorkspaceSettings()
    {
    }

    public TwitterWorkspaceSettings(EnumTwitterType type, int count, double refreshTime, int columnInGrid, double columnInGridWidth)
      : this(type, count, refreshTime, null, null, columnInGrid, columnInGridWidth)
    {
    }

    public TwitterWorkspaceSettings(EnumTwitterType type, int count, double refreshTime, int columnInGrid, TwitterListShow lst,double columnInGridWidth)
      : this(type, count, refreshTime, columnInGrid, lst as TwitterList, columnInGridWidth)
    {
      Color = lst.ColorIcon;
    }

    
    public TwitterWorkspaceSettings(EnumTwitterType type, int count, double refreshTime, int columnInGrid, TwitterList lst, double columnInGridWidth)
    {
      RefreshTime = refreshTime;
      if (RefreshTime.Equals(double.NaN))
        RefreshTime = DoubleValueAttribute.GetDoubleValue(type);
      
      Type = type;
      Count = count;
      GroupName = lst.FullName;
      UserToGet = lst.Id;
      ColumnInGrid = columnInGrid;
      ColumnInGridWidth = columnInGridWidth;
    }

    public TwitterWorkspaceSettings(EnumTwitterType type, int count, double refreshTime, int columnInGrid,
                                    double columnInGridWidth, int maxTweets)
      : this(type, count, refreshTime, null, null, columnInGrid, columnInGridWidth)
    {
      MaxTweets = maxTweets;
    }

    public TwitterWorkspaceSettings(EnumTwitterType type, int count, double refreshTime, string groupName,
                                    List<string> groupMembers, int columnInGrid, double columnInGridWidth)
      : this(type, count, refreshTime, groupName, groupMembers, null, columnInGrid, columnInGridWidth)
    {
    }

    public TwitterWorkspaceSettings(EnumTwitterType type, int count, double refreshTime, string groupName,
                                    List<string> groupMembers, int columnInGrid, double columnInGridWidth, int maxTweets)
      : this(type, count, refreshTime, groupName, groupMembers, null, columnInGrid, columnInGridWidth)
    {
      MaxTweets = maxTweets;
    }

    public TwitterWorkspaceSettings(EnumTwitterType type, int count, double refreshTime, string groupName,
                                    List<string> groupMembers, string userToGet, int columnInGrid,
                                    double columnInGridWidth)
    {
      RefreshTime = refreshTime;
      if (RefreshTime.Equals(double.NaN))
      {
        RefreshTime = DoubleValueAttribute.GetDoubleValue(type);
      }

      Type = type;
      Count = count;
      GroupName = groupName;
      GroupMembers = groupMembers;
      UserToGet = userToGet;
      ColumnInGrid = columnInGrid;
      ColumnInGridWidth = columnInGridWidth;
    }

    public TwitterWorkspaceSettings(EnumTwitterType type, int count, double refreshTime, string groupName,
                                    List<string> groupMembers, string userToGet, int columnInGrid,
                                    double columnInGridWidth, int maxTweets)
    {
      RefreshTime = refreshTime;
      if (RefreshTime.Equals(double.NaN))
      {
        RefreshTime = DoubleValueAttribute.GetDoubleValue(type);
      }

      Type = type;
      Count = count;
      GroupName = groupName;
      GroupMembers = groupMembers;
      UserToGet = userToGet;
      ColumnInGrid = columnInGrid;
      ColumnInGridWidth = columnInGridWidth;
      MaxTweets = maxTweets;
    }

    public EnumTwitterType Type { get; set; }

    public double RefreshTime { get; set; }

    public string GroupName { get; set; }

    public List<string> GroupMembers { get; set; }

    public int Count { get; set; }

    public int MaxTweets { get; set; }

    public int PageNb { get; set; }

    public string UserToGet { get; set; }

    public Brush Color { get; set; }

    public int ColumnInGrid { get; set; }

    public double ColumnInGridWidth { get; set; }

    public DateTime DateLastUpdate { get; set; }
  }
}