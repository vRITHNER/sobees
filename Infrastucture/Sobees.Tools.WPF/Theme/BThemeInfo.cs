#region

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Xml.Serialization;
using Sobees.Tools.Logging;
using Sobees.Tools.Util;

#endregion

namespace Sobees.Tools.Theme
{
  public class BThemeInfo : IComparable<BThemeInfo>
  {
    #region Data

    private static BThemeInfo _default;

    public readonly string AssemblyName;
    public readonly bool IsDefault;
    public readonly string SkinName;

    #endregion // Data

    #region Constructors

    public BThemeInfo(string assemblyName,
                      string skinName)
    {
      IsDefault = false;

      AssemblyName = assemblyName;
      SkinName = skinName.ToLower();
    }

    public BThemeInfo()
    {
      IsDefault = true;

      AssemblyName = String.Empty;
      SkinName = "bTheme";
    }

    #endregion // Constructors

    #region Properties

    /// <summary>
    ///   Define the position of the theme inside Themes collection
    /// </summary>
    public int Position { get; set; }

    /// <summary>
    ///   Indicate if we need to take the next Theme in Themes collection
    /// </summary>
    public bool TakeNextTheme { get; set; }

    [XmlIgnore]
    public ResourceDictionary SkinResourceDictionary { get; set; }

    public static BThemeInfo Default => _default ?? (_default = new BThemeInfo());

    public bool Exists => IsDefault || File.Exists(AssemblyName);

    #endregion // Properties

    #region Base Class Overrides

    public override bool Equals(object obj)
    {
      var other = obj as BThemeInfo;

      if (other == null)
        return false;

      return String.Equals(SkinName, other.SkinName, StringComparison.InvariantCultureIgnoreCase);
    }

    public override int GetHashCode()
    {
      return base.GetHashCode() + AssemblyName.GetHashCode();
    }

    public override string ToString()
    {
      return SkinName;
    }

    #endregion // Base Class Overrides

    #region IComparable<BThemeInfo> Members

    public int CompareTo(BThemeInfo other)
    {
      if (other == null)
        return +1;

      if (SkinName == null)
        return other.SkinName == null ? 0 : -1;

      return SkinName.CompareTo(other.SkinName);
    }

    #endregion
  }

  [Serializable]
  public class BThemeInfoForQueue
  {
    #region Properties

    /// <summary>
    ///   Define the position of the theme inside Themes collection
    /// </summary>
    public int Position { get; set; }

    /// <summary>
    ///   Indicate if we need to take the next Theme in Themes collection
    /// </summary>
    public bool TakeNextTheme { get; set; }

    #endregion // Properties
  }

  public class BThemeInfoCollection : ReadOnlyCollection<BThemeInfo>
  {
    private readonly string _mThemesLocation = string.Empty;

    public BThemeInfoCollection()
      : base(new List<BThemeInfo>())
    {
      // Add the default skin, which is baked into Podder.
      //Items.Add(BThemeInfo.Default);

      // Now load the external skins, if any exist.
      LoadExternalSkins(string.Empty);

      ((List<BThemeInfo>) Items).Sort();
    }

    public BThemeInfoCollection(string location)
      : base(new List<BThemeInfo>())
    {
      _mThemesLocation = location;

      // Add the default skin, which is baked into Podder.
      //Items.Add(BThemeInfo.Default);

      // Now load the external skins, if any exist.
      LoadExternalSkins(location);

      ((List<BThemeInfo>) Items).Sort();
    }

    /// <summary>
    ///   LoadExternalSkins
    /// </summary>
    /// <param name = "assemblylocation"></param>
    private void LoadExternalSkins(string assemblylocation)
    {
      var location = assemblylocation == string.Empty ? AssemblyHelper.GetEntryAssemblyLocation() : assemblylocation;

      var rootDir = Path.GetDirectoryName(location);
      var themesDir = Path.Combine(rootDir, _mThemesLocation);

        if (!Directory.Exists(themesDir)) return;
        
        var childDomain = AppDomain.CreateDomain("LoadingZone");

        try
        {
            var assemblyName = AssemblyHelper.GetExecutingAssemblyName();
            var typeName = typeof (SkinAssemblyLoader).FullName;

            //Create an instance of SkinAssemblyLoader in the  
            //child AppDomain. A proxy to the object is returned.
            var assembly = childDomain.Load(assemblyName);

            var loader = assembly.CreateInstance(typeName) as SkinAssemblyLoader;

            var resourceAssemblies = Directory.GetFiles(themesDir, "*Sobees.Themes*.dll");

            if (loader == null) return;
            foreach (var themeInfo in
                resourceAssemblies.Where(assm => true).Select(loader.GetThemeInfoForAssembly).Where(themeInfo => themeInfo != null))
            {
                themeInfo.Position = Items.Count;
                Items.Add(themeInfo);
            }
        }
        catch (Exception ex)
        {
            TraceHelper.Trace(this, ex);
        }
        finally
        {
            AppDomain.Unload(childDomain);
        }
    }

    #region Nested type: SkinAssemblyLoader

    /// <summary>
    ///   This class is instantiated in a separate AppDomain.  
    ///   It loads each resource assembly and extracts some
    ///   information from them.  When the object is finished 
    ///   being used, the AppDomain in which it lives is unloaded.
    /// </summary>
    private class SkinAssemblyLoader : MarshalByRefObject
    {
      /// <summary>
      ///   This method runs in a temporary AppDomain, and 
      ///   extracts information about a resource assembly.
      /// </summary>
      /// <param name = "assemblyFile">
      ///   An absolute file path to a resource assembly.
      /// </param>
      internal BThemeInfo GetThemeInfoForAssembly(string assemblyFile)
      {
        Assembly assm = null;
        try
        {
          assm = Assembly.LoadFrom(assemblyFile);
        }
        catch (Exception ex)
        {
          Debug.Fail("Error while loading resource assembly: " + ex.Message);
        }

        if (assm == null)
          return null;

        return new BThemeInfo(assemblyFile, assm.GetName().Name.ToLower().Replace("sobees.themes.btheme", ""));
      }
    }

    #endregion
  }
}