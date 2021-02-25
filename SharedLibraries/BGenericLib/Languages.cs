using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using Sobees.Tools.Logging;

namespace Sobees.Library.BGenericLib
{
#if !SILVERLIGHT
  [Serializable]
#endif
  public enum EnumLanguages
  {
    // Check here languages code :
    // http://www.loc.gov/standards/iso639-2/php/code_list.php

    [Description("English")] en,
    [Description("Français")] fr,
    [Description("Deutsch")] de,
    [Description("Español")] es,
    [Description("Italiano")] it,
    [Description("All")] all,
  }
  #if !SILVERLIGHT
  public class Languages
  {
    public static string GetStateDescription(EnumLanguages state)
    {
      Type type = typeof (EnumLanguages);
      FieldInfo fieldInfo = type.GetField(state.ToString());
      var descAttributes = fieldInfo.GetCustomAttributes(typeof (DescriptionAttribute),
                                                         false) as DescriptionAttribute[];
      return (descAttributes.Length > 0) ? descAttributes[0].Description : null;
    }

    public static EnumLanguages GetStateFromDescription(string state)
    {
      try
      {
        if (string.IsNullOrEmpty(state))
        {
          return EnumLanguages.en;
        }
        var type = typeof (EnumLanguages);
        var fields = new List<FieldInfo>(type.GetFields());
        var finder = new DescAttrFinder(state);
        var fi = fields.Find(finder.FindPredicate);
        return (EnumLanguages) fi.GetRawConstantValue();
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("Languages::GetStateFromDescription:",
                          (ex));
      }
      return EnumLanguages.en;
    }

    #region Nested type: DescAttrFinder

    private class DescAttrFinder
    {
      private readonly string descAttributeValue;

      public DescAttrFinder(string descAttributeValue)
      {
        this.descAttributeValue = descAttributeValue;
      }

      public bool FindPredicate(FieldInfo fi)
      {
        var descAttributes = fi.GetCustomAttributes(typeof (DescriptionAttribute),
                                                    false) as DescriptionAttribute[];
        string desc = (descAttributes.Length > 0) ? descAttributes[0].Description : null;
        return descAttributeValue.CompareTo(desc) == 0;
      }
    }

    #endregion

  }
#endif
}