#region

using System;
using System.Reflection;
using System.Security;

#endregion

namespace Sobees.Tools.Util
{
  public class AssemblyInfoDetails
  {
    private static string productVersion = string.Empty;

    private static string productName = string.Empty;

    ///Get System version from the assembly ///
    public static string ProductVersion
    {
      get
      {
        var assembly = Assembly.GetEntryAssembly();
        if (assembly != null)
          productVersion = assembly.GetName().Version.ToString();

        return productVersion;
      }
    }

    ///Get the name of the System from the assembly ///
    public static string ProductName
    {
      get
      {
        var assembly = Assembly.GetEntryAssembly();
        if (assembly != null)
          productName = assembly.GetName().ToString();

        return productName;
      }
    }

    public static string ProductNameOnly
    {
      get
      {
        var assembly = Assembly.GetEntryAssembly();
        if (assembly != null)
          productName = assembly.GetName().Name;

        return productName;
      }
    }

    /// <summary>
    ///   return the version of the CLR .NET version installed
    /// </summary>
    public static string DotNetFrameworkVersion
    {
      get
      {
        try
        {
          //bool fx35Installed = FrameworkVersionDetection.IsInstalled(FrameworkVersion.Fx35);
          var fx40Installed = FrameworkVersionDetection.IsInstalled(FrameworkVersion.Fx40);
          var version = ".NET version:{0}|{1}";
          version = string.Format(version, Environment.Version, fx40Installed ? "4.0" : "3.5");
          //
          return version;
        }
        catch (SecurityException)
        {
          return "Unknown";
        }
      }
    }
  }
}