#region

using System;
using System.Deployment.Application;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.Win32;
using Sobees.Tools.Logging;

#endregion

namespace Sobees.Tools.Helpers
{
  public class BShortcutHelper
  {
    public static bool IsApplicationNetworkDeployed
    {
      get
      {
        if (ApplicationDeployment.IsNetworkDeployed)
        {
          return true;
        }
        return false;
      }
    }

    public static void SetStartupShortcut(bool enable)
    {
      if (!IsApplicationNetworkDeployed)
      {
        var runKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

        if (enable)
        {
          if (runKey != null)
            runKey.SetValue(Application.ProductName, Application.ExecutablePath);
        }
        else
        {
          if (runKey != null)
            runKey.DeleteValue(Application.ProductName, false);
        }
      }
      else
      {
        //TODO BSG: What happens on clickonce updates?  Does this change the required .appref-ms reference?
        var assembly = Assembly.GetEntryAssembly();

        try
        {
          // Get the startup shortcut and delete file to disable.
          var productName = GetProductName(assembly);
          var startupShortcut = GetStartupShortcut(productName);

          if (!enable)
          {
            if (File.Exists(startupShortcut))
              File.Delete(startupShortcut);
            return;
          }

          // Copy program files shortcut into the startup folder to enable.
          var publisherName = GetPublisherName(assembly);
          var programShortcut = GetProgramShortcut(productName, publisherName);

          if (File.Exists(programShortcut))
          {
            if (!File.Exists(startupShortcut))
              File.Copy(programShortcut, startupShortcut);
          }
        }
        catch (Exception ex)
        {
          TraceHelper.Trace("ShortCutHelper::SetStartupShortcut:", ex);
        }
      }
    }

    private static string GetProgramShortcut(string product,
                                             string publisher)
    {
      var allProgramsPath = Environment.GetFolderPath(Environment.SpecialFolder.Programs);
      var shortcutPath = Path.Combine(allProgramsPath, publisher);
      shortcutPath = Path.Combine(shortcutPath, product) + ".appref-ms";
      return shortcutPath;
    }

    private static string GetStartupShortcut(string product)
    {
      var startupPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
      startupPath = Path.Combine(startupPath, product) + ".appref-ms";
      return startupPath;
    }

    private static string GetProductName(Assembly assembly)
    {
      if (!Attribute.IsDefined(assembly, typeof (AssemblyProductAttribute)))
        return string.Empty;

      var product = (AssemblyProductAttribute) Attribute.GetCustomAttribute(assembly, typeof (AssemblyProductAttribute));

      return product.Product;
    }

    private static string GetPublisherName(Assembly assembly)
    {
      if (!Attribute.IsDefined(assembly, typeof (AssemblyCompanyAttribute)))
        return string.Empty;

      var company = (AssemblyCompanyAttribute) Attribute.GetCustomAttribute(assembly, typeof (AssemblyCompanyAttribute));

      return company.Company;
    }
  }
}