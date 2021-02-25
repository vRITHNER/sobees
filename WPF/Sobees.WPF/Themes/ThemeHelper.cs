#region

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using Sobees.Tools.Logging;
using Sobees.Tools.Theme;
using Sobees.Tools.Util;

#endregion

namespace Sobees.Themes
{
  /// <summary>bThemeHelper</summary>
  [System.Runtime.InteropServices.GuidAttribute("DED1C32E-ECAD-49B2-83C5-08323B187A2A")]
  public class BThemeHelper
  {
    public static string CurrentTheme = "Classical";
    private static BThemeInfoCollection _themes;
    internal static ListCollectionView ThemeView;

    public static BThemeInfo CurrentBThemeInfo { get; private set; }

    public static BThemeInfoCollection BThemesCollection(string themePath)
    {
      return _themes ?? (_themes = new BThemeInfoCollection(themePath));
    }

    /// <summary>GetThemes</summary>
    /// <returns></returns>
    public static ListCollectionView GetThemes()
    {
      var themePath = string.Format(@"{0}\{1}", Path.GetDirectoryName(AssemblyHelper.GetEntryAssemblyLocation()),
                                    "Themes");

      return (ListCollectionView) CollectionViewSource.GetDefaultView(BThemesCollection(themePath));
    }


    /// <summary>ApplyNextTheme</summary>
    /// <returns></returns>
    public static string ApplyNextTheme()
    {
      try
      {
        var offset = CurrentBThemeInfo.Position;
        offset++;
        if (offset > _themes.Count - 1)
          offset = 0;

        var s = string.Empty;
        ThemeView.Filter = (a => ((BThemeInfo)a).Position == offset);
        var bThemeInfo = ThemeView.CurrentItem as BThemeInfo;
        if (bThemeInfo != null)
        {
          s = ApplyTheme(bThemeInfo.SkinName);
        }
        return s;
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("Error::bThemeHelper::ApplyNextTheme:", ex);
      }
      return string.Empty;
    }

    public static string ApplyTheme(string themeName)
    {
      var nRetry = 0;
      try
      {
        //Workaround for old settings
        if (themeName.ToLower().Equals("blue"))
          themeName = "classical";

        BThemeInfo bThemeInfo = null;

        var themePath = string.Format(@"{0}\{1}", Path.GetDirectoryName(AssemblyHelper.GetEntryAssemblyLocation()),
                                      "Themes");

        ThemeView = CollectionViewSource.GetDefaultView(BThemesCollection(themePath)) as ListCollectionView;

        if (ThemeView != null)
        {
          ThemeView.Filter = (a => ((BThemeInfo)a).SkinName.ToLower().Contains(themeName.ToLower()));
          bThemeInfo = ThemeView.CurrentItem as BThemeInfo;
        }

        var skinResources = GetSkinResources(bThemeInfo);

        //ResourceDictionary rdToKeep = null;
      //foreach (var mergedDictionary in Application.Current.Resources.MergedDictionaries)
      //{
      //  if (mergedDictionary.Source != null
      //      && !string.IsNullOrEmpty(mergedDictionary.Source.OriginalString)
      //      && mergedDictionary.Source.OriginalString.ToLower().Contains("template"))
      //    rdToKeep = mergedDictionary;
      //}

        //TODO: Need to verify why it's crashing 1st time and it's good after....
      _retryAfterError:
        try
        {
          foreach (var dictionary in Application.Current.Resources.MergedDictionaries)
          {
            if (dictionary.Source == null) continue;
            if (dictionary.Source.OriginalString.ToLower().Contains("template")) continue;
            Application.Current.Resources.MergedDictionaries.Remove(dictionary);
            goto _retryAfterError;
          }
          foreach (var dictionary in Application.Current.Resources.MergedDictionaries)
          {
            if (dictionary.Source != null) continue;
            var i = Application.Current.Resources.MergedDictionaries.IndexOf(dictionary);
            Application.Current.Resources.MergedDictionaries.Insert(i, skinResources);
            Application.Current.Resources.MergedDictionaries.Remove(dictionary);
            break;
          }
        }
        catch (Exception)
        {
          if (nRetry < 2)
          {
            nRetry++;
            goto _retryAfterError;
          }
          throw;
        }

        CurrentBThemeInfo = bThemeInfo;
        CurrentTheme = themeName;
        if (ThemeView != null) ThemeView.Filter = null;
        if (bThemeInfo != null) return bThemeInfo.SkinName;
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("Error::bThemeHelper::ApplySkin:",
                          ex);
      }
      return themeName;
    }

    /// <summary>GetSkinResources</summary>
    /// <param name="themeInfo"></param>
    /// <returns></returns>
    public static ResourceDictionary GetSkinResources(BThemeInfo themeInfo)
    {
      ResourceDictionary skinDictionary = null;

      try
      {
        if (themeInfo == null)
        {
          TraceHelper.Trace("ThemeHelper::GetSkinResources:", "themeInfo == NULL");
          return null;
        }

        // Load all of the DLLs used by the skin into our AppDomain.
        var path = Path.GetDirectoryName(themeInfo.AssemblyName);
        var assemblies = Directory.GetFiles(path,
                                            "*.dll");
        foreach (var assembly in assemblies)
          Assembly.LoadFrom(assembly);

        // Load the master ResourceDictionary for the skin.
        var assemblyName =
          Path.GetFileNameWithoutExtension(themeInfo.AssemblyName);
        var packUri = string.Format("{0};component/Theme{1}.xaml", assemblyName, themeInfo.SkinName);
        var resourceLocator = new Uri(packUri,
                                      UriKind.Relative);

        skinDictionary = Application.LoadComponent(resourceLocator) as ResourceDictionary;
      }
      catch (Exception ex)
      {
        Debug.Fail("Could not load theme: " + ex.Message);
      }
      return skinDictionary;
    }

    /// <summary>
    ///   ChangeCustomTheme
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="bThemeInfo"></param>
    /// <param name="customThemeToMerge"></param>
    /// <param name="assembly"></param>
    public static void ChangeCustomTheme(object sender,
                                         BThemeInfo bThemeInfo,
                                         string[] customThemeToMerge,
                                         Assembly assembly)
    {
      try
      {
        if (sender == null || customThemeToMerge.Length == 0)
          return;

        var control = sender as FrameworkElement;

        var assemblyName = assembly.GetName().Name.ToLower().Replace(".dll",
                                                                     "");

        foreach (var customThemeName in customThemeToMerge)
        {
          if (customThemeName != string.Empty)
          {
            var uriName = string.Format("/{0};component/Themes/{1}{2}.xaml",
                                        assemblyName,
                                        bThemeInfo.SkinName,
                                        customThemeName);
            var uri = new Uri(uriName,
                              UriKind.Relative);

            Debug.WriteLine(uri.ToString());

            var skinDictionary = Application.LoadComponent(uri) as ResourceDictionary;
            if (control != null) control.Resources.MergedDictionaries.Add(skinDictionary);
          }
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("Error::ThemeHelper::ChangeTheme:",
                          ex);
      }
    }
  }
}
