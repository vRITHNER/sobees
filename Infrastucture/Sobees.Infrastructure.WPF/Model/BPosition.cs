#region

using System;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Serialization;

#endregion

namespace Sobees.Infrastructure.Model
{
#if !SILVERLIGHT
  [Serializable]
#endif
  [XmlInclude(typeof(Orientation))]
  [XmlInclude(typeof(VerticalAlignment))]
  [XmlInclude(typeof(HorizontalAlignment))]
  public class BPosition
  {
    public BPosition() { }

    public BPosition(BPosition pos)
    {
      Col = pos.Col;
      Row = pos.Row;
      RowSpan = pos.RowSpan;
      ColSpan = pos.ColSpan;
      Orientation = pos.Orientation;
      VerticalAlignment = pos.VerticalAlignment;
      HorizontalAlignment = pos.HorizontalAlignment;
    }

    public BPosition(int col,
                     int row,
                     int rowspan,
                     int colspan)
    {
      Col = col;
      Row = row;
      RowSpan = rowspan;
      ColSpan = colspan;
    }

    public BPosition(int col,
                     int row,
                     int rowspan,
                     int colspan,
                     Orientation orientation,
                     VerticalAlignment va,
                     HorizontalAlignment ha)
    {
      Col = col;
      Row = row;
      RowSpan = rowspan;
      ColSpan = colspan;
      Orientation = orientation;
      VerticalAlignment = va;
      HorizontalAlignment = ha;
    }

    public int Col { get; set; }
    public int Row { get; set; }
    public int RowSpan { get; set; }
    public int ColSpan { get; set; }

    public double Width { get; set; }
    public double Height { get; set; }
    public GridUnitType UnitType { get; set; }

    public Orientation Orientation { get; set; }
    public VerticalAlignment VerticalAlignment { get; set; }
    public HorizontalAlignment HorizontalAlignment { get; set; }

    public static void SetPosition(FrameworkElement element,
                                   BPosition bp)
    {
      if (element != null
          && bp != null)
      {
        Grid.SetColumn(element,
                       bp.Col);
        Grid.SetRow(element,
                    bp.Row);
        Grid.SetColumnSpan(element,
                           bp.ColSpan);
        Grid.SetRowSpan(element,
                        bp.RowSpan);
      }
    }

    public static BPosition GetGridSpans(Grid gr)
    {
      if (gr == null) return null;
      var position = new BPosition(0,
                                   0,
                                   gr.RowDefinitions.Count,
                                   gr.ColumnDefinitions.Count);

      return position;
    }

    public override bool Equals(object obj)
    {
      var bp = obj as BPosition;
      if (bp == null) return false;

      if (Col == bp.Col && Row == bp.Row && ColSpan == bp.ColSpan
          && RowSpan == bp.RowSpan)
        return true;

      return false;
    }

    public bool Equals(BPosition other)
    {
      if (ReferenceEquals(null,
                          other)) return false;
      if (ReferenceEquals(this,
                          other)) return true;
      return other.Col == Col && other.Row == Row && other.RowSpan == RowSpan && other.ColSpan == ColSpan;
    }

    public override int GetHashCode()
    {
      unchecked
      {
        var result = Col;
        result = (result * 397) ^ Row;
        result = (result * 397) ^ RowSpan;
        result = (result * 397) ^ ColSpan;
        return result;
      }
    }
  }
}