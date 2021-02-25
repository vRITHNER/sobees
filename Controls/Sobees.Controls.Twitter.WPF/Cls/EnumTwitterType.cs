#region Includes

using System;
using System.Reflection;

#endregion

namespace Sobees.Controls.Twitter.Cls
{
  public enum EnumTwitterType
  {
    [DoubleValue(10)]
    Replies,
    [DoubleValue(10)]
    Search,
    [DoubleValue(10)]
    DirectMessages,
    [DoubleValue(2)]
    Friends,
    [DoubleValue(5)]
    Groups,
    [DoubleValue(5)]
    List,
    [DoubleValue(5)]
    User,
    [DoubleValue(15)]
    Favorites,
  }

  /// <summary>
  /// This attribute is used to represent a double value
  /// for a value in an enum.
  /// </summary>
  public class DoubleValueAttribute : Attribute
  {
    #region Properties

    /// <summary>
    /// Holds the doublevalue for a value in an enum.
    /// </summary>
    public double DoubleValue { get; protected set; }

    #endregion

    #region Constructor

    /// <summary>
    /// Constructor used to init a StringValue Attribute
    /// </summary>
    /// <param name="value"></param>
    public DoubleValueAttribute(double value)
    {
      DoubleValue = value;
    }

    #endregion

    /// <summary>
    /// Will get the string value for a given enums value, this will
    /// only work if you assign the StringValue attribute to
    /// the items in your enum.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static double GetDoubleValue(EnumTwitterType value)
    {
      // Get the type
      Type type = value.GetType();

      // Get fieldinfo for this type
      FieldInfo fieldInfo = type.GetField(value.ToString());

      // Get the stringvalue attributes
      var attribs = fieldInfo.GetCustomAttributes(
                      typeof(DoubleValueAttribute), false) as DoubleValueAttribute[];

      // Return the first if there was a match.
      return attribs.Length > 0 ? attribs[0].DoubleValue : 10; // 10 is default!!!
    }
  }
}