#region Includes

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Sobees.Tools.Logging;
using Sobees.Tools.Serialization;

#endregion

namespace Sobees.Infrastructure.Model
{
  [XmlInclude(typeof (BPosition))]
  public class BTemplate : INotifyPropertyChanged, IXmlSerializable
  {
    private bool _dependencyPropertyDescriptorHack;
    private bool _isUserSelectedbTemplate = true;

    public BTemplate()
    {
    }

    public bool DependencyPropertyDescriptorHack
    {
      get { return _dependencyPropertyDescriptorHack; }
      set
      {
        _dependencyPropertyDescriptorHack = value;
        OnPropertyChanged("DependencyPropertyDescriptorHack");
      }
    }

    public int Columns { get; set; }
    public int Rows { get; set; }
    public string ImgUrl { get; set; }
    public List<BPosition> BPositions { get; set; }
    public List<BPosition> GridSplitterPositions { get; set; }

    public bool IsUserSelectedbTemplate
    {
      get { return _isUserSelectedbTemplate; }
      set { _isUserSelectedbTemplate = value; }
    }

    #region INotifyPropertyChanged Members

    /// <summary>
    /// Raised when a property on this object has a new value.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    #endregion

    #region IXmlSerializable Members

    public XmlSchema GetSchema()
    {
      return null;
    }

    public void ReadXml(XmlReader reader)
    {
      try
      {
        reader.MoveToContent();
        reader.Read();
        //reader.ReadStartElement("BTemplate");

        reader.ReadStartElement("IsUserSelectedbTemplate");
        IsUserSelectedbTemplate = bool.Parse(reader.ReadContentAsString());
        reader.ReadEndElement();

        reader.ReadStartElement("Columns");
        Columns = int.Parse(reader.ReadContentAsString());
        reader.ReadEndElement();

        reader.ReadStartElement("Rows");
        Rows = int.Parse(reader.ReadContentAsString());
        reader.ReadEndElement();

        reader.ReadStartElement("BPositions");
        BPositions = GenericCollectionSerializer.DeserializeObject<List<BPosition>>(reader.ReadContentAsString());
        reader.ReadEndElement();

        reader.ReadStartElement("GridSplitterPositions");
        GridSplitterPositions =
          GenericCollectionSerializer.DeserializeObject<List<BPosition>>(reader.ReadContentAsString());
        reader.ReadEndElement();

        reader.ReadEndElement();
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }

    public void WriteXml(XmlWriter writer)
    {
      try
      {
        //writer.WriteStartElement("BTemplate");
        writer.WriteElementString("IsUserSelectedbTemplate", IsUserSelectedbTemplate.ToString());
        writer.WriteElementString("Columns", Columns.ToString());
        writer.WriteElementString("Rows", Rows.ToString());
        string positions = GenericCollectionSerializer.SerializeObject(BPositions);
        writer.WriteElementString("BPositions", positions);
        string gsPositions = GenericCollectionSerializer.SerializeObject(GridSplitterPositions);
        writer.WriteElementString("GridSplitterPositions", gsPositions);
        //writer.WriteEndElement();
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }

    #endregion

    public override string ToString()
    {
      return $"Columns:{Columns} | Rows:{Rows} ({ImgUrl}) | IsUserSelectedGridTemplate: {IsUserSelectedbTemplate}";
    }

      /// <summary>
      /// Raises this object's PropertyChanged event.
      /// </summary>
      /// <param name="propertyName">The property that has a new value.</param>
      protected virtual void OnPropertyChanged(string propertyName)
      {
          PropertyChangedEventHandler handler = PropertyChanged;
          if (handler != null)
          {
              var e = new PropertyChangedEventArgs(propertyName);
              handler(this, e);
          }
      }
  }
}