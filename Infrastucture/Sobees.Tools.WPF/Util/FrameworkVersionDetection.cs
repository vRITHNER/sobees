#region

using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using Microsoft.Win32;

#endregion

namespace Sobees.Tools.Util
{

  #region class FrameworkVersionDetection

  /// <summary>
  ///   Provides support for determining if a specific version of the .NET
  ///   Framework runtime is installed and the service pack level for the
  ///   runtime version.
  /// </summary>
  public static class FrameworkVersionDetection
  {
    #region events

    #endregion

    #region class-wide fields

    private const string Netfx10RegKeyName = "Software\\Microsoft\\.NETFramework\\Policy\\v1.0";
    private const string Netfx10RegKeyValue = "3705";

    private const string Netfx10SPxMSIRegKeyName = "Software\\Microsoft\\Active Setup\\Installed Components\\{78705f0d-e8db-4b2d-8193-982bdda15ecd}";

    private const string Netfx10SPxOCMRegKeyName = "Software\\Microsoft\\Active Setup\\Installed Components\\{FDC11A6F-17D1-48f9-9EA3-9051954BAA24}";

    private const string Netfx10SPxRegValueName = "Version";
    private const string Netfx11RegKeyName = "Software\\Microsoft\\NET Framework Setup\\NDP\\v1.1.4322";
    private const string Netfx20RegKeyName = "Software\\Microsoft\\NET Framework Setup\\NDP\\v2.0.50727";
    private const string Netfx30RegKeyName = "Software\\Microsoft\\NET Framework Setup\\NDP\\v3.0\\Setup";
    private const string Netfx35RegKeyName = "Software\\Microsoft\\NET Framework Setup\\NDP\\v3.5";
    private const string Netfx40ClientRegKeyName = "Software\\Microsoft\\NET Framework Setup\\NDP\\v4\\Client";
    private const string Netfx40FullRegKeyName = "Software\\Microsoft\\NET Framework Setup\\NDP\\v4\\Full";
    private const string Netfx11PlusRegValueName = "Install";
    private const string Netfx30PlusRegValueName = "InstallSuccess";
    private const string Netfx11PlusSPxRegValueName = "SP";
    private const string Netfx20PlusBuildRegValueName = "Increment";
    private const string Netfx30PlusVersionRegValueName = "Version";
    private const string Netfx35PlusBuildRegValueName = "Build";
    private const string Netfx30PlusWCFRegKeyName = Netfx30RegKeyName + "\\Windows Communication Foundation";
    private const string Netfx30PlusWPFRegKeyName = Netfx30RegKeyName + "\\Windows Presentation Foundation";
    private const string Netfx30PlusWFRegKeyName = Netfx30RegKeyName + "\\Windows Workflow Foundation";
    private const string Netfx30PlusWFPlusVersionRegValueName = "FileVersion";
    private const string CardSpaceServicesRegKeyName = "System\\CurrentControlSet\\Services\\idsvc";
    private const string CardSpaceServicesPlusImagePathRegName = "ImagePath";

    #endregion

    #region private and internal properties and methods

    #region properties

    #endregion

    #region methods

    #region GetRegistryValue

    private static bool GetRegistryValue<T>(RegistryHive hive,
                                            string key,
                                            string value,
                                            RegistryValueKind kind,
                                            out T data)
    {
      var success = false;
      data = default(T);

      using (var baseKey = RegistryKey.OpenRemoteBaseKey(hive, String.Empty))
      {
        if (baseKey != null)
        {
          using (var registryKey = baseKey.OpenSubKey(key, RegistryKeyPermissionCheck.ReadSubTree))
          {
            if (registryKey != null)
            {
              // If the key was opened, try to retrieve the value.
              var kindFound = registryKey.GetValueKind(value);
              if (kindFound == kind)
              {
                var regValue = registryKey.GetValue(value, null);
                if (regValue != null)
                {
                  data = (T) Convert.ChangeType(regValue, typeof (T), CultureInfo.InvariantCulture);
                  success = true;
                }
              }
            }
          }
        }
      }
      return success;
    }

    #endregion

    #region IsNetfxInstalled functions

    #region IsNetfx10Installed

    private static bool IsNetfx10Installed()
    {
      var regValue = string.Empty;
      return (GetRegistryValue(RegistryHive.LocalMachine, Netfx10RegKeyName, Netfx10RegKeyValue, RegistryValueKind.String, out regValue));
    }

    #endregion

    #region IsNetfx11Installed

    private static bool IsNetfx11Installed()
    {
      var found = false;
      var regValue = 0;

      if (GetRegistryValue(RegistryHive.LocalMachine, Netfx11RegKeyName, Netfx11PlusRegValueName, RegistryValueKind.DWord, out regValue))
      {
        if (regValue == 1)
        {
          found = true;
        }
      }

      return found;
    }

    #endregion

    #region IsNetfx20Installed

    private static bool IsNetfx20Installed()
    {
      var found = false;
      var regValue = 0;

      if (GetRegistryValue(RegistryHive.LocalMachine, Netfx20RegKeyName, Netfx11PlusRegValueName, RegistryValueKind.DWord, out regValue))
      {
        if (regValue == 1)
        {
          found = true;
        }
      }

      return found;
    }

    #endregion

    #region IsNetfx30Installed

    private static bool IsNetfx30Installed()
    {
      var found = false;
      var regValue = 0;

      if (GetRegistryValue(RegistryHive.LocalMachine, Netfx30RegKeyName, Netfx30PlusRegValueName, RegistryValueKind.DWord, out regValue))
      {
        if (regValue == 1)
        {
          found = true;
        }
      }

      return found;
    }

    #endregion

    #region IsNetfx35Installed

    private static bool IsNetfx35Installed()
    {
      var found = false;
      var regValue = 0;

      if (GetRegistryValue(RegistryHive.LocalMachine, Netfx35RegKeyName, Netfx11PlusRegValueName, RegistryValueKind.DWord, out regValue))
      {
        if (regValue == 1)
        {
          found = true;
        }
      }

      return found;
    }

    #endregion

    #region IsNetfx40Installed

    private static bool IsNetfx40Installed()
    {
      var found = false;
      var regValue = 0;

      if (GetRegistryValue(RegistryHive.LocalMachine, Netfx40ClientRegKeyName, Netfx11PlusRegValueName, RegistryValueKind.DWord, out regValue))
      {
        if (regValue == 1)
        {
          found = true;
        }
      }
      if (GetRegistryValue(RegistryHive.LocalMachine, Netfx40FullRegKeyName, Netfx11PlusRegValueName, RegistryValueKind.DWord, out regValue))
      {
        if (regValue == 1)
        {
          found = true;
        }
      }

      return found;
    }

    #endregion

    #endregion

    #region GetNetfxSPLevel functions

    #region GetNetfx10SPLevel

    private static int GetNetfx10SPLevel()
    {
      var foundKey = false;
      var servicePackLevel = -1;
      string regValue;

      if (IsTabletOrMediaCenter())
      {
        foundKey = GetRegistryValue(RegistryHive.LocalMachine, Netfx10SPxOCMRegKeyName, Netfx10SPxRegValueName, RegistryValueKind.String, out regValue);
      }
      else
      {
        foundKey = GetRegistryValue(RegistryHive.LocalMachine, Netfx10SPxMSIRegKeyName, Netfx10SPxRegValueName, RegistryValueKind.String, out regValue);
      }

      if (foundKey)
      {
        // This registry value should be of the format
        // #,#,#####,# where the last # is the SP level
        // Try to parse off the last # here
        var index = regValue.LastIndexOf(',');
        if (index > 0)
        {
          Int32.TryParse(regValue.Substring(index + 1), out servicePackLevel);
        }
      }

      return servicePackLevel;
    }

    #endregion

    #region GetNetfx11SPLevel

    private static int GetNetfx11SPLevel()
    {
      var regValue = 0;

      // We can only get -1 if the .NET Framework is not
      // installed or there was some kind of error retrieving
      // the data from the registry
      var servicePackLevel = -1;

      if (GetRegistryValue(RegistryHive.LocalMachine, Netfx11RegKeyName, Netfx11PlusSPxRegValueName, RegistryValueKind.DWord, out regValue))
      {
        servicePackLevel = regValue;
      }

      return servicePackLevel;
    }

    #endregion

    #region GetNetfx20SPLevel

    private static int GetNetfx20SPLevel()
    {
      var regValue = 0;

      // We can only get -1 if the .NET Framework is not
      // installed or there was some kind of error retrieving
      // the data from the registry
      var servicePackLevel = -1;

      if (GetRegistryValue(RegistryHive.LocalMachine, Netfx20RegKeyName, Netfx11PlusSPxRegValueName, RegistryValueKind.DWord, out regValue))
      {
        servicePackLevel = regValue;
      }

      return servicePackLevel;
    }

    #endregion

    #region GetNetfx30SPLevel

    // This code is MOST LIKELY correct but will need to be verified.
    //
    // Currently, there are no service packs available for version 3.0 of 
    // the framework, so we always return -1. When a service pack does
    // become available, this method will need to be revised to correctly
    // determine the service pack level.
    private static int GetNetfx30SPLevel()
    {
      //int regValue = 0;

      // We can only get -1 if the .NET Framework is not
      // installed or there was some kind of error retrieving
      // the data from the registry
      var servicePackLevel = -1;

      //if (GetRegistryValue(RegistryHive.LocalMachine, Netfx30RegKeyName, Netfx11PlusSPxRegValueName, RegistryValueKind.DWord, out regValue))
      //{
      //    servicePackLevel = regValue;
      //}

      return servicePackLevel;
    }

    #endregion

    #region GetNetfx35SPLevel

    private static int GetNetfx35SPLevel()
    {
      var regValue = 0;

      // We can only get -1 if the .NET Framework is not
      // installed or there was some kind of error retrieving
      // the data from the registry
      var servicePackLevel = -1;

      if (GetRegistryValue(RegistryHive.LocalMachine, Netfx35RegKeyName, Netfx11PlusSPxRegValueName, RegistryValueKind.DWord, out regValue))
      {
        servicePackLevel = regValue;
      }

      return servicePackLevel;
    }

    #endregion

    #region GetNetfx40SPLevel

    private static int GetNetfx40SPLevel()
    {
      var regValue = 0;

      // We can only get -1 if the .NET Framework is not
      // installed or there was some kind of error retrieving
      // the data from the registry
      var servicePackLevel = -1;

      if (GetRegistryValue(RegistryHive.LocalMachine, Netfx40ClientRegKeyName, Netfx11PlusSPxRegValueName, RegistryValueKind.DWord, out regValue))
      {
        servicePackLevel = regValue;
      }

      return servicePackLevel;
    }

    #endregion

    #endregion

    #region GetNetfxExactVersion functions

    #region GetNetfx10ExactVersion

    private static Version GetNetfx10ExactVersion()
    {
      var foundKey = false;
      var fxVersion = new Version();
      string regValue;

      if (IsTabletOrMediaCenter())
      {
        foundKey = GetRegistryValue(RegistryHive.LocalMachine, Netfx10SPxOCMRegKeyName, Netfx10SPxRegValueName, RegistryValueKind.String, out regValue);
      }
      else
      {
        foundKey = GetRegistryValue(RegistryHive.LocalMachine, Netfx10SPxMSIRegKeyName, Netfx10SPxRegValueName, RegistryValueKind.String, out regValue);
      }

      if (foundKey)
      {
        // This registry value should be of the format
        // #,#,#####,# where the last # is the SP level
        // Try to parse off the last # here
        var index = regValue.LastIndexOf(',');
        if (index > 0)
        {
          var tokens = regValue.Substring(0, index).Split(',');
          if (tokens.Length == 3)
          {
            fxVersion = new Version(
              Convert.ToInt32(tokens[0], NumberFormatInfo.InvariantInfo),
              Convert.ToInt32(tokens[1], NumberFormatInfo.InvariantInfo),
              Convert.ToInt32(tokens[2], NumberFormatInfo.InvariantInfo));
          }
        }
      }

      return fxVersion;
    }

    #endregion

    #region GetNetfx11ExactVersion

    private static Version GetNetfx11ExactVersion()
    {
      var regValue = 0;

      // We can only get -1 if the .NET Framework is not
      // installed or there was some kind of error retrieving
      // the data from the registry
      var fxVersion = new Version();

      if (GetRegistryValue(RegistryHive.LocalMachine, Netfx11RegKeyName, Netfx11PlusRegValueName, RegistryValueKind.DWord, out regValue))
      {
        if (regValue == 1)
        {
          // In the strict sense, we are cheating here, but the registry key name itself
          // contains the version number.
          var tokens = Netfx11RegKeyName.Split(new[] {"NDP\\v"}, StringSplitOptions.None);
          if (tokens.Length == 2)
          {
            fxVersion = new Version(tokens[1]);
          }
        }
      }

      return fxVersion;
    }

    #endregion

    #region GetNetfx20ExactVersion

    private static Version GetNetfx20ExactVersion()
    {
      var regValue = String.Empty;

      // We can only get -1 if the .NET Framework is not
      // installed or there was some kind of error retrieving
      // the data from the registry
      var fxVersion = new Version();

      if (GetRegistryValue(RegistryHive.LocalMachine, Netfx20RegKeyName, Netfx20PlusBuildRegValueName, RegistryValueKind.String, out regValue))
      {
        if (!String.IsNullOrEmpty(regValue))
        {
          // In the strict sense, we are cheating here, but the registry key name itself
          // contains the version number.
          var versionTokens = Netfx20RegKeyName.Split(new[] {"NDP\\v"}, StringSplitOptions.None);
          if (versionTokens.Length == 2)
          {
            var tokens = versionTokens[1].Split('.');
            if (tokens.Length == 3)
            {
              fxVersion = new Version(
                Convert.ToInt32(tokens[0], NumberFormatInfo.InvariantInfo),
                Convert.ToInt32(tokens[1], NumberFormatInfo.InvariantInfo),
                Convert.ToInt32(tokens[2], NumberFormatInfo.InvariantInfo),
                Convert.ToInt32(regValue, NumberFormatInfo.InvariantInfo));
            }
          }
        }
      }

      return fxVersion;
    }

    #endregion

    #region GetNetfx30ExactVersion

    private static Version GetNetfx30ExactVersion()
    {
      var regValue = String.Empty;

      // We can only get the default version if the .NET Framework
      // is not installed or there was some kind of error retrieving
      // the data from the registry
      var fxVersion = new Version();

      if (GetRegistryValue(RegistryHive.LocalMachine, Netfx30RegKeyName, Netfx30PlusVersionRegValueName, RegistryValueKind.String, out regValue))
      {
        if (!String.IsNullOrEmpty(regValue))
        {
          fxVersion = new Version(regValue);
        }
      }

      return fxVersion;
    }

    #endregion

    #region GetNetfx35ExactVersion

    private static Version GetNetfx35ExactVersion()
    {
      var regValue = String.Empty;

      // We can only get the default version if the .NET Framework
      // is not installed or there was some kind of error retrieving
      // the data from the registry
      var fxVersion = new Version();

      if (GetRegistryValue(RegistryHive.LocalMachine, Netfx35RegKeyName, Netfx30PlusVersionRegValueName, RegistryValueKind.String, out regValue))
      {
        if (!String.IsNullOrEmpty(regValue))
        {
          fxVersion = new Version(regValue);
        }
      }

      return fxVersion;
    }

    #endregion

    #region GetNetfx40ExactVersion

    private static Version GetNetfx40ExactVersion()
    {
      var regValue = String.Empty;

      // We can only get the default version if the .NET Framework
      // is not installed or there was some kind of error retrieving
      // the data from the registry
      var fxVersion = new Version();

      if (GetRegistryValue(RegistryHive.LocalMachine, Netfx40ClientRegKeyName, Netfx30PlusVersionRegValueName, RegistryValueKind.String, out regValue))
      {
        if (!String.IsNullOrEmpty(regValue))
        {
          fxVersion = new Version(regValue);
        }
      }

      return fxVersion;
    }

    #endregion

    #endregion

    #region WindowsFounationLibrary functions

    #region CardSpace

    #region IsNetfx30CardSpaceInstalled

    private static bool IsNetfx30CardSpaceInstalled()
    {
      var found = false;
      var regValue = String.Empty;

      if (GetRegistryValue(RegistryHive.LocalMachine, CardSpaceServicesRegKeyName, CardSpaceServicesPlusImagePathRegName, RegistryValueKind.ExpandString, out regValue))
      {
        if (!String.IsNullOrEmpty(regValue))
        {
          found = true;
        }
      }

      return found;
    }

    #endregion

    #region GetNetfx30CardSpaceSPLevel

    // Currently, there are no service packs available for version 3.0 of 
    // the framework, so we always return -1. When a service pack does
    // become available, this method will need to be revised to correctly
    // determine the service pack level. Based on the current method for
    // determining if CardSpace is installed, it may not be possible to
    // correctly determine the Service Pack level for CardSpace.
    private static int GetNetfx30CardSpaceSPLevel()
    {
      var servicePackLevel = -1;
      return servicePackLevel;
    }

    #endregion

    #region GetNetfx30CardSpaceExactVersion

    private static Version GetNetfx30CardSpaceExactVersion()
    {
      var regValue = String.Empty;

      // We can only get the default version if the .NET Framework
      // is not installed or there was some kind of error retrieving
      // the data from the registry
      var fxVersion = new Version();

      if (GetRegistryValue(RegistryHive.LocalMachine, CardSpaceServicesRegKeyName, CardSpaceServicesPlusImagePathRegName, RegistryValueKind.ExpandString, out regValue))
      {
        if (!String.IsNullOrEmpty(regValue))
        {
          var fileVersionInfo = FileVersionInfo.GetVersionInfo(regValue.Trim('"'));
          var index = fileVersionInfo.FileVersion.IndexOf(' ');
          fxVersion = new Version(fileVersionInfo.FileVersion.Substring(0, index));
        }
      }

      return fxVersion;
    }

    #endregion

    #endregion

    #region Windows Communication Foundation

    #region IsNetfx30WCFInstalled

    private static bool IsNetfx30WCFInstalled()
    {
      var found = false;
      var regValue = 0;

      if (GetRegistryValue(RegistryHive.LocalMachine, Netfx30PlusWCFRegKeyName, Netfx30PlusRegValueName, RegistryValueKind.DWord, out regValue))
      {
        if (regValue == 1)
        {
          found = true;
        }
      }

      return found;
    }

    #endregion

    #region GetNetfx30WCFSPLevel

    // This code is MOST LIKELY correct but will need to be verified.
    //
    // Currently, there are no service packs available for version 3.0 of 
    // the framework, so we always return -1. When a service pack does
    // become available, this method will need to be revised to correctly
    // determine the service pack level.
    private static int GetNetfx30WCFSPLevel()
    {
      //int regValue = 0;

      // We can only get -1 if the .NET Framework is not
      // installed or there was some kind of error retrieving
      // the data from the registry
      var servicePackLevel = -1;

      //if (GetRegistryValue(RegistryHive.LocalMachine, Netfx30PlusWCFRegKeyName, Netfx11PlusSPxRegValueName, RegistryValueKind.DWord, out regValue))
      //{
      //    servicePackLevel = regValue;
      //}

      return servicePackLevel;
    }

    #endregion

    #region GetNetfx30WCFExactVersion

    private static Version GetNetfx30WCFExactVersion()
    {
      var regValue = String.Empty;

      // We can only get the default version if the .NET Framework
      // is not installed or there was some kind of error retrieving
      // the data from the registry
      var fxVersion = new Version();

      if (GetRegistryValue(RegistryHive.LocalMachine, Netfx30PlusWCFRegKeyName, Netfx30PlusVersionRegValueName, RegistryValueKind.String, out regValue))
      {
        if (!String.IsNullOrEmpty(regValue))
        {
          fxVersion = new Version(regValue);
        }
      }

      return fxVersion;
    }

    #endregion

    #endregion

    #region Windows Presentation Foundation

    #region IsNetfx30WPFInstalled

    private static bool IsNetfx30WPFInstalled()
    {
      var found = false;
      var regValue = 0;

      if (GetRegistryValue(RegistryHive.LocalMachine, Netfx30PlusWPFRegKeyName, Netfx30PlusRegValueName, RegistryValueKind.DWord, out regValue))
      {
        if (regValue == 1)
        {
          found = true;
        }
      }

      return found;
    }

    #endregion

    #region GetNetfx30WPFSPLevel

    // This code is MOST LIKELY correct but will need to be verified.
    //
    // Currently, there are no service packs available for version 3.0 of 
    // the framework, so we always return -1. When a service pack does
    // become available, this method will need to be revised to correctly
    // determine the service pack level.
    private static int GetNetfx30WPFSPLevel()
    {
      //int regValue = 0;

      // We can only get -1 if the .NET Framework is not
      // installed or there was some kind of error retrieving
      // the data from the registry
      var servicePackLevel = -1;

      //if (GetRegistryValue(RegistryHive.LocalMachine, Netfx30PlusWPFRegKeyName, Netfx11PlusSPxRegValueName, RegistryValueKind.DWord, out regValue))
      //{
      //    servicePackLevel = regValue;
      //}

      return servicePackLevel;
    }

    #endregion

    #region GetNetfx30WPFExactVersion

    private static Version GetNetfx30WPFExactVersion()
    {
      var regValue = String.Empty;

      // We can only get the default version if the .NET Framework
      // is not installed or there was some kind of error retrieving
      // the data from the registry
      var fxVersion = new Version();

      if (GetRegistryValue(RegistryHive.LocalMachine, Netfx30PlusWPFRegKeyName, Netfx30PlusVersionRegValueName, RegistryValueKind.String, out regValue))
      {
        if (!String.IsNullOrEmpty(regValue))
        {
          fxVersion = new Version(regValue);
        }
      }

      return fxVersion;
    }

    #endregion

    #endregion

    #region Windows Workflow Foundation

    #region IsNetfx30WFInstalled

    private static bool IsNetfx30WFInstalled()
    {
      var found = false;
      var regValue = 0;

      if (GetRegistryValue(RegistryHive.LocalMachine, Netfx30PlusWFRegKeyName, Netfx30PlusRegValueName, RegistryValueKind.DWord, out regValue))
      {
        if (regValue == 1)
        {
          found = true;
        }
      }

      return found;
    }

    #endregion

    #region GetNetfx30WFSPLevel

    // This code is MOST LIKELY correct but will need to be verified.
    //
    // Currently, there are no service packs available for version 3.0 of 
    // the framework, so we always return -1. When a service pack does
    // become available, this method will need to be revised to correctly
    // determine the service pack level.
    private static int GetNetfx30WFSPLevel()
    {
      //int regValue = 0;

      // We can only get -1 if the .NET Framework is not
      // installed or there was some kind of error retrieving
      // the data from the registry
      var servicePackLevel = -1;

      //if (GetRegistryValue(RegistryHive.LocalMachine, Netfx30PlusWFRegKeyName, Netfx11PlusSPxRegValueName, RegistryValueKind.DWord, out regValue))
      //{
      //    servicePackLevel = regValue;
      //}

      return servicePackLevel;
    }

    #endregion

    #region GetNetfx30WFExactVersion

    private static Version GetNetfx30WFExactVersion()
    {
      var regValue = String.Empty;

      // We can only get the default version if the .NET Framework
      // is not installed or there was some kind of error retrieving
      // the data from the registry
      var fxVersion = new Version();

      if (GetRegistryValue(RegistryHive.LocalMachine, Netfx30PlusWFRegKeyName, Netfx30PlusWFPlusVersionRegValueName, RegistryValueKind.String, out regValue))
      {
        if (!String.IsNullOrEmpty(regValue))
        {
          fxVersion = new Version(regValue);
        }
      }

      return fxVersion;
    }

    #endregion

    #endregion

    #endregion

    #region IsTabletOrMediaCenter

    private static bool IsTabletOrMediaCenter()
    {
      return ((SafeNativeMethods.GetSystemMetrics(SystemMetric.SM_TABLETPC) != 0) || (SafeNativeMethods.GetSystemMetrics(SystemMetric.SM_MEDIACENTER) != 0));
    }

    #endregion

    #endregion

    #endregion

    #region public properties and methods

    #region properties

    #endregion

    #region methods

    #region IsInstalled

    #region IsInstalled(FrameworkVersion frameworkVersion)

    /// <summary>
    ///   Determines if the specified .NET Framework version is installed
    ///   on the local computer.
    /// </summary>
    /// <param name = "frameworkVersion">One of the
    ///   <see cref = "FrameworkVersion" /> values.</param>
    /// <returns><see langword = "true" /> if the specified .NET Framework
    ///   version is installed; otherwise <see langword = "false" />.</returns>
    public static bool IsInstalled(FrameworkVersion frameworkVersion)
    {
      var ret = false;

      switch (frameworkVersion)
      {
        case FrameworkVersion.Fx10:
          ret = IsNetfx10Installed();
          break;

        case FrameworkVersion.Fx11:
          ret = IsNetfx11Installed();
          break;

        case FrameworkVersion.Fx20:
          ret = IsNetfx20Installed();
          break;

        case FrameworkVersion.Fx30:
          ret = IsNetfx30Installed();
          break;

        case FrameworkVersion.Fx35:
          ret = IsNetfx35Installed();
          break;

        case FrameworkVersion.Fx40:
          ret = IsNetfx40Installed();
          break;

        default:
          break;
      }

      return ret;
    }

    #endregion

    #region IsInstalled(WindowsFoundationLibrary foundationLibrary)

    /// <summary>
    ///   Determines if the specified .NET Framework Foundation Library is
    ///   installed on the local computer.
    /// </summary>
    /// <param name = "foundationLibrary">One of the
    ///   <see cref = "WindowsFoundationLibrary" /> values.</param>
    /// <returns><see langword = "true" /> if the specified .NET Framework
    ///   Foundation Library is installed; otherwise <see langword = "false" />.</returns>
    public static bool IsInstalled(WindowsFoundationLibrary foundationLibrary)
    {
      var ret = false;

      switch (foundationLibrary)
      {
        case WindowsFoundationLibrary.CardSpace:
          ret = IsNetfx30CardSpaceInstalled();
          break;

        case WindowsFoundationLibrary.WCF:
          ret = IsNetfx30WCFInstalled();
          break;

        case WindowsFoundationLibrary.WF:
          ret = IsNetfx30WFInstalled();
          break;

        case WindowsFoundationLibrary.WPF:
          ret = IsNetfx30WPFInstalled();
          break;

        default:
          break;
      }

      return ret;
    }

    #endregion

    #endregion

    #region GetServicePackLevel

    #region GetServicePackLevel(FrameworkVersion frameworkVersion)

    /// <summary>
    ///   Retrieves the service pack level for the specified .NET Framework
    ///   version.
    /// </summary>
    /// <param name = "frameworkVersion">One of the
    ///   <see cref = "FrameworkVersion" /> values.</param>
    /// <returns>An <see cref = "Int32">integer</see> value representing
    ///   the service pack level for the specified .NET Framework version. If
    ///   the specified .NET Frameowrk version is not found, -1 is returned.
    /// </returns>
    public static int GetServicePackLevel(FrameworkVersion frameworkVersion)
    {
      var servicePackLevel = -1;

      switch (frameworkVersion)
      {
        case FrameworkVersion.Fx10:
          servicePackLevel = GetNetfx10SPLevel();
          break;

        case FrameworkVersion.Fx11:
          servicePackLevel = GetNetfx11SPLevel();
          break;

        case FrameworkVersion.Fx20:
          servicePackLevel = GetNetfx20SPLevel();
          break;

        case FrameworkVersion.Fx30:
          servicePackLevel = GetNetfx30SPLevel();
          break;

        case FrameworkVersion.Fx35:
          servicePackLevel = GetNetfx35SPLevel();
          break;

        default:
          break;
      }

      return servicePackLevel;
    }

    #endregion

    #region GetServicePackLevel(WindowsFoundationLibrary foundationLibrary)

    /// <summary>
    ///   Retrieves the service pack level for the specified .NET Framework
    ///   Foundation Library.
    /// </summary>
    /// <param name = "foundationLibrary">One of the
    ///   <see cref = "WindowsFoundationLibrary" /> values.</param>
    /// <returns>An <see cref = "Int32">integer</see> value representing
    ///   the service pack level for the specified .NET Framework Foundation
    ///   Library. If the specified .NET Frameowrk Foundation Library is not
    ///   found, -1 is returned.
    /// </returns>
    public static int GetServicePackLevel(WindowsFoundationLibrary foundationLibrary)
    {
      var servicePackLevel = -1;

      switch (foundationLibrary)
      {
        case WindowsFoundationLibrary.CardSpace:
          servicePackLevel = GetNetfx30CardSpaceSPLevel();
          break;

        case WindowsFoundationLibrary.WCF:
          servicePackLevel = GetNetfx30WCFSPLevel();
          break;

        case WindowsFoundationLibrary.WF:
          servicePackLevel = GetNetfx30WFSPLevel();
          break;

        case WindowsFoundationLibrary.WPF:
          servicePackLevel = GetNetfx30WPFSPLevel();
          break;

        default:
          break;
      }

      return servicePackLevel;
    }

    #endregion

    #endregion

    #region GetExactVersion

    #region GetExactVersion(FrameworkVersion frameworkVersion)

    /// <summary>
    ///   Retrieves the exact version number for the specified .NET Framework
    ///   version.
    /// </summary>
    /// <param name = "frameworkVersion">One of the
    ///   <see cref = "FrameworkVersion" /> values.</param>
    /// <returns>A <see cref = "Version">version</see> representing
    ///   the exact version number for the specified .NET Framework version.
    ///   If the specified .NET Frameowrk version is not found, a 
    ///   <see cref = "Version" /> is returned that represents a 0.0.0.0 version
    ///   number.
    /// </returns>
    public static Version GetExactVersion(FrameworkVersion frameworkVersion)
    {
      var fxVersion = new Version();

      switch (frameworkVersion)
      {
        case FrameworkVersion.Fx10:
          fxVersion = GetNetfx10ExactVersion();
          break;

        case FrameworkVersion.Fx11:
          fxVersion = GetNetfx11ExactVersion();
          break;

        case FrameworkVersion.Fx20:
          fxVersion = GetNetfx20ExactVersion();
          break;

        case FrameworkVersion.Fx30:
          fxVersion = GetNetfx30ExactVersion();
          break;

        case FrameworkVersion.Fx35:
          fxVersion = GetNetfx35ExactVersion();
          break;

        default:
          break;
      }

      return fxVersion;
    }

    #endregion

    #region GetExactVersion(WindowsFoundationLibrary foundationLibrary)

    /// <summary>
    ///   Retrieves the exact version number for the specified .NET Framework
    ///   Foundation Library.
    /// </summary>
    /// <param name = "foundationLibrary">One of the
    ///   <see cref = "WindowsFoundationLibrary" /> values.</param>
    /// <returns>A <see cref = "Version">version</see> representing
    ///   the exact version number for the specified .NET Framework Foundation
    ///   Library. If the specified .NET Frameowrk Foundation Library is not
    ///   found, a <see cref = "Version" /> is returned that represents a 
    ///   0.0.0.0 version number.
    /// </returns>
    public static Version GetExactVersion(WindowsFoundationLibrary foundationLibrary)
    {
      var fxVersion = new Version();

      switch (foundationLibrary)
      {
        case WindowsFoundationLibrary.CardSpace:
          fxVersion = GetNetfx30CardSpaceExactVersion();
          break;

        case WindowsFoundationLibrary.WCF:
          fxVersion = GetNetfx30WCFExactVersion();
          break;

        case WindowsFoundationLibrary.WF:
          fxVersion = GetNetfx30WFExactVersion();
          break;

        case WindowsFoundationLibrary.WPF:
          fxVersion = GetNetfx30WPFExactVersion();
          break;

        default:
          break;
      }

      return fxVersion;
    }

    #endregion

    #endregion

    #endregion

    #endregion
  }

  #endregion

  #region enum FrameworkVersion

  /// <summary>
  ///   Specifies the .NET Framework versions
  /// </summary>
  public enum FrameworkVersion
  {
    /// <summary>
    ///   .NET Framework 1.0
    /// </summary>
    Fx10,

    /// <summary>
    ///   .NET Framework 1.1
    /// </summary>
    Fx11,

    /// <summary>
    ///   .NET Framework 2.0
    /// </summary>
    Fx20,

    /// <summary>
    ///   .NET Framework 3.0
    /// </summary>
    Fx30,

    /// <summary>
    ///   .NET Framework 3.5 (Orcas)
    /// </summary>
    Fx35,
    Fx40
  }

  #endregion

  #region enum WindowsFoundationLibrary

  /// <summary>
  ///   Specifies the .NET 3.0 Windows Foundation Library
  /// </summary>
  public enum WindowsFoundationLibrary
  {
    /// <summary>
    ///   Windows Communication Foundation
    /// </summary>
    WCF,

    /// <summary>
    ///   Windows Presentation Foundation
    /// </summary>
    WPF,

    /// <summary>
    ///   Windows Workflow Foundation
    /// </summary>
    WF,

    /// <summary>
    ///   Windows CardSpace
    /// </summary>
    CardSpace,
  }

  #endregion

  #region enum SystemMetric

  /// <summary>
  ///   Flags used with the Windows API GetSystemMetrics(SystemMetric smIndex)
  /// </summary>
  internal enum SystemMetric
  {
    /// <summary>
    ///   Width of the screen of the primary display monitor, in pixels. 
    ///   This is the same values obtained by calling GetDeviceCaps as 
    ///   follows: GetDeviceCaps( hdcPrimaryMonitor, HORZRES).
    /// </summary>
    SM_CXSCREEN = 0,

    /// <summary>
    ///   Height of the screen of the primary display monitor, in pixels. 
    ///   This is the same values obtained by calling GetDeviceCaps as 
    ///   follows: GetDeviceCaps( hdcPrimaryMonitor, VERTRES).
    /// </summary>
    SM_CYSCREEN = 1,

    /// <summary>
    ///   Width of a horizontal scroll bar, in pixels.
    /// </summary>
    SM_CYVSCROLL = 2,

    /// <summary>
    ///   Height of a horizontal scroll bar, in pixels.
    /// </summary>
    SM_CXVSCROLL = 3,

    /// <summary>
    ///   Height of a caption area, in pixels.
    /// </summary>
    SM_CYCAPTION = 4,

    /// <summary>
    ///   Width of a window border, in pixels. This is equivalent to the
    ///   SM_CXEDGE value for windows with the 3-D look.
    /// </summary>
    SM_CXBORDER = 5,

    /// <summary>
    ///   Height of a window border, in pixels. This is equivalent to the 
    ///   SM_CYEDGE value for windows with the 3-D look.
    /// </summary>
    SM_CYBORDER = 6,

    /// <summary>
    ///   Thickness of the frame around the perimeter of a window that has
    ///   a caption but is not sizable, in pixels. SM_CXFIXEDFRAME is the
    ///   height of the horizontal border and SM_CYFIXEDFRAME is the width
    ///   of the vertical border.
    /// </summary>
    SM_CXDLGFRAME = 7,

    /// <summary>
    ///   Thickness of the frame around the perimeter of a window that has
    ///   a caption but is not sizable, in pixels. SM_CXFIXEDFRAME is the
    ///   height of the horizontal border and SM_CYFIXEDFRAME is the width
    ///   of the vertical border.
    /// </summary>
    SM_CYDLGFRAME = 8,

    /// <summary>
    ///   Height of the thumb box in a vertical scroll bar, in pixels
    /// </summary>
    SM_CYVTHUMB = 9,

    /// <summary>
    ///   Width of the thumb box in a horizontal scroll bar, in pixels.
    /// </summary>
    SM_CXHTHUMB = 10,

    /// <summary>
    ///   Default width of an icon, in pixels. The LoadIcon function can 
    ///   load only icons with the dimensions specified by SM_CXICON and
    ///   SM_CYICON
    /// </summary>
    SM_CXICON = 11,

    /// <summary>
    ///   Default height of an icon, in pixels. The LoadIcon function can 
    ///   load only icons with the dimensions SM_CXICON and SM_CYICON.
    /// </summary>
    SM_CYICON = 12,

    /// <summary>
    ///   Width of a cursor, in pixels. The system cannot create 
    ///   cursors of other sizes.
    /// </summary>
    SM_CXCURSOR = 13,

    /// <summary>
    ///   Height of a cursor, in pixels. The system cannot create cursors
    ///   of other sizes.
    /// </summary>
    SM_CYCURSOR = 14,

    /// <summary>
    ///   Height of a single-line menu bar, in pixels.
    /// </summary>
    SM_CYMENU = 15,

    /// <summary>
    ///   Width of the client area for a full-screen window on the primary
    ///   display monitor, in pixels. To get the coordinates of the portion 
    ///   of the screen not obscured by the system taskbar or by application 
    ///   desktop toolbars, call the SystemParametersInfo function with the
    ///   SPI_GETWORKAREA value.
    /// </summary>
    SM_CXFULLSCREEN = 16,

    /// <summary>
    ///   Height of the client area for a full-screen window on the primary
    ///   display monitor, in pixels. To get the coordinates of the portion
    ///   of the screen not obscured by the system taskbar or by application
    ///   desktop toolbars, call the SystemParametersInfo function with the
    ///   SPI_GETWORKAREA value.
    /// </summary>
    SM_CYFULLSCREEN = 17,

    /// <summary>
    ///   For double byte character set versions of the system, this is the
    ///   height of the Kanji window at the bottom of the screen, in pixels.
    /// </summary>
    SM_CYKANJIWINDOW = 18,

    /// <summary>
    ///   Nonzero if a mouse with a wheel is installed; zero otherwise
    /// </summary>
    SM_MOUSEWHEELPRESENT = 75,

    /// <summary>
    ///   Height of the arrow bitmap on a vertical scroll bar, in pixels.
    /// </summary>
    SM_CYHSCROLL = 20,

    /// <summary>
    ///   Width of the arrow bitmap on a horizontal scroll bar, in pixels.
    /// </summary>
    SM_CXHSCROLL = 21,

    /// <summary>
    ///   Nonzero if the debug version of User.exe is installed; zero
    ///   otherwise.
    /// </summary>
    SM_DEBUG = 22,

    /// <summary>
    ///   Nonzero if the left and right mouse buttons are reversed; zero
    ///   otherwise.
    /// </summary>
    SM_SWAPBUTTON = 23,

    /// <summary>
    ///   Reserved for future use
    /// </summary>
    SM_RESERVED1 = 24,

    /// <summary>
    ///   Reserved for future use
    /// </summary>
    SM_RESERVED2 = 25,

    /// <summary>
    ///   Reserved for future use
    /// </summary>
    SM_RESERVED3 = 26,

    /// <summary>
    ///   Reserved for future use
    /// </summary>
    SM_RESERVED4 = 27,

    /// <summary>
    ///   Minimum width of a window, in pixels.
    /// </summary>
    SM_CXMIN = 28,

    /// <summary>
    ///   Minimum height of a window, in pixels.
    /// </summary>
    SM_CYMIN = 29,

    /// <summary>
    ///   Width of a button in a window's caption or title bar, in pixels.
    /// </summary>
    SM_CXSIZE = 30,

    /// <summary>
    ///   Height of a button in a window's caption or title bar, in pixels.
    /// </summary>
    SM_CYSIZE = 31,

    /// <summary>
    ///   Thickness of the sizing border around the perimeter of a window 
    ///   that can be resized, in pixels. SM_CXSIZEFRAME is the width of the
    ///   horizontal border, and SM_CYSIZEFRAME is the height of the
    ///   vertical border.
    /// </summary>
    SM_CXFRAME = 32,

    /// <summary>
    ///   Thickness of the sizing border around the perimeter of a window 
    ///   that can be resized, in pixels. SM_CXSIZEFRAME is the width of the
    ///   horizontal border, and SM_CYSIZEFRAME is the height of the 
    ///   vertical border.
    /// </summary>
    SM_CYFRAME = 33,

    /// <summary>
    ///   Minimum tracking width of a window, in pixels. The user cannot drag
    ///   the window frame to a size smaller than these dimensions. A window
    ///   can override this value by processing the WM_GETMINMAXINFO message.
    /// </summary>
    SM_CXMINTRACK = 34,

    /// <summary>
    ///   Minimum tracking height of a window, in pixels. The user cannot 
    ///   drag the window frame to a size smaller than these dimensions. A 
    ///   window can override this value by processing the WM_GETMINMAXINFO
    ///   message
    /// </summary>
    SM_CYMINTRACK = 35,

    /// <summary>
    ///   Width of the rectangle around the location of a first click in a 
    ///   double-click sequence, in pixels. The second click must occur 
    ///   within the rectangle defined by SM_CXDOUBLECLK and SM_CYDOUBLECLK 
    ///   for the system to consider the two clicks a double-click
    /// </summary>
    SM_CXDOUBLECLK = 36,

    /// <summary>
    ///   Height of the rectangle around the location of a first click in a
    ///   double-click sequence, in pixels. The second click must occur 
    ///   within the rectangle defined by SM_CXDOUBLECLK and SM_CYDOUBLECLK
    ///   for the system to consider the two clicks a double-click. (The two
    ///   clicks must also occur within a specified time.)
    /// </summary>
    SM_CYDOUBLECLK = 37,

    /// <summary>
    ///   Width of a grid cell for items in large icon view, in pixels. Each
    ///   item fits into a rectangle of size SM_CXICONSPACING by 
    ///   SM_CYICONSPACING when arranged. This value is always greater 
    ///   than or equal to SM_CXICON
    /// </summary>
    SM_CXICONSPACING = 38,

    /// <summary>
    ///   Height of a grid cell for items in large icon view, in pixels. 
    ///   Each item fits into a rectangle of size SM_CXICONSPACING by 
    ///   SM_CYICONSPACING when arranged. This value is always greater than
    ///   or equal to SM_CYICON.
    /// </summary>
    SM_CYICONSPACING = 39,

    /// <summary>
    ///   Nonzero if drop-down menus are right-aligned with the corresponding
    ///   menu-bar item; zero if the menus are left-aligned.
    /// </summary>
    SM_MENUDROPALIGNMENT = 40,

    /// <summary>
    ///   Nonzero if the Microsoft Windows for Pen computing extensions are
    ///   installed; zero otherwise.
    /// </summary>
    SM_PENWINDOWS = 41,

    /// <summary>
    ///   Nonzero if User32.dll supports DBCS; zero otherwise. 
    ///   (WinMe/95/98): Unicode
    /// </summary>
    SM_DBCSENABLED = 42,

    /// <summary>
    ///   Number of buttons on mouse, or zero if no mouse is installed.
    /// </summary>
    SM_CMOUSEBUTTONS = 43,

    /// <summary>
    ///   Identical Values Changed After Windows NT 4.0
    /// </summary>
    SM_CXFIXEDFRAME = SM_CXDLGFRAME,

    /// <summary>
    ///   Identical Values Changed After Windows NT 4.0
    /// </summary>
    SM_CYFIXEDFRAME = SM_CYDLGFRAME,

    /// <summary>
    ///   Identical Values Changed After Windows NT 4.0
    /// </summary>
    SM_CXSIZEFRAME = SM_CXFRAME,

    /// <summary>
    ///   Identical Values Changed After Windows NT 4.0
    /// </summary>
    SM_CYSIZEFRAME = SM_CYFRAME,

    /// <summary>
    ///   Nonzero if security is present; zero otherwise.
    /// </summary>
    SM_SECURE = 44,

    /// <summary>
    ///   Width of a 3-D border, in pixels. This is the 3-D counterpart 
    ///   of SM_CXBORDER.
    /// </summary>
    SM_CXEDGE = 45,

    /// <summary>
    ///   Height of a 3-D border, in pixels. This is the 3-D counterpart 
    ///   of SM_CYBORDER.
    /// </summary>
    SM_CYEDGE = 46,

    /// <summary>
    ///   Width of a grid cell for a minimized window, in pixels. Each 
    ///   minimized window fits into a rectangle this size when arranged. 
    ///   This value is always greater than or equal to SM_CXMINIMIZED.
    /// </summary>
    SM_CXMINSPACING = 47,

    /// <summary>
    ///   Height of a grid cell for a minimized window, in pixels. Each 
    ///   minimized window fits into a rectangle this size when arranged. 
    ///   This value is always greater than or equal to SM_CYMINIMIZED.
    /// </summary>
    SM_CYMINSPACING = 48,

    /// <summary>
    ///   Recommended width of a small icon, in pixels. Small icons typically
    ///   appear in window captions and in small icon view
    /// </summary>
    SM_CXSMICON = 49,

    /// <summary>
    ///   Recommended height of a small icon, in pixels. Small icons 
    ///   typically appear in window captions and in small icon view.
    /// </summary>
    SM_CYSMICON = 50,

    /// <summary>
    ///   Height of a small caption, in pixels
    /// </summary>
    SM_CYSMCAPTION = 51,

    /// <summary>
    ///   Width of small caption buttons, in pixels.
    /// </summary>
    SM_CXSMSIZE = 52,

    /// <summary>
    ///   Height of small caption buttons, in pixels.
    /// </summary>
    SM_CYSMSIZE = 53,

    /// <summary>
    ///   Width of menu bar buttons, such as the child window close button
    ///   used in the multiple document interface, in pixels.
    /// </summary>
    SM_CXMENUSIZE = 54,

    /// <summary>
    ///   Height of menu bar buttons, such as the child window close button
    ///   used in the multiple document interface, in pixels.
    /// </summary>
    SM_CYMENUSIZE = 55,

    /// <summary>
    ///   Flags specifying how the system arranged minimized windows
    /// </summary>
    SM_ARRANGE = 56,

    /// <summary>
    ///   Width of a minimized window, in pixels.
    /// </summary>
    SM_CXMINIMIZED = 57,

    /// <summary>
    ///   Height of a minimized window, in pixels.
    /// </summary>
    SM_CYMINIMIZED = 58,

    /// <summary>
    ///   Default maximum width of a window that has a caption and sizing 
    ///   borders, in pixels. This metric refers to the entire desktop. The
    ///   user cannot drag the window frame to a size larger than these 
    ///   dimensions. A window can override this value by processing the 
    ///   WM_GETMINMAXINFO message.
    /// </summary>
    SM_CXMAXTRACK = 59,

    /// <summary>
    ///   Default maximum height of a window that has a caption and sizing 
    ///   borders, in pixels. This metric refers to the entire desktop. The
    ///   user cannot drag the window frame to a size larger than these 
    ///   dimensions. A window can override this value by processing the 
    ///   WM_GETMINMAXINFO message.
    /// </summary>
    SM_CYMAXTRACK = 60,

    /// <summary>
    ///   Default width, in pixels, of a maximized top-level window on the
    ///   primary display monitor.
    /// </summary>
    SM_CXMAXIMIZED = 61,

    /// <summary>
    ///   Default height, in pixels, of a maximized top-level window on the 
    ///   primary display monitor.
    /// </summary>
    SM_CYMAXIMIZED = 62,

    /// <summary>
    ///   Least significant bit is set if a network is present; otherwise, 
    ///   it is cleared. The other bits are reserved for future use
    /// </summary>
    SM_NETWORK = 63,

    /// <summary>
    ///   Value that specifies how the system was started: 0-normal, 
    ///   1-failsafe, 2-failsafe /w net
    /// </summary>
    SM_CLEANBOOT = 67,

    /// <summary>
    ///   Width of a rectangle centered on a drag point to allow for limited
    ///   movement of the mouse pointer before a drag operation begins, 
    ///   in pixels.
    /// </summary>
    SM_CXDRAG = 68,

    /// <summary>
    ///   Height of a rectangle centered on a drag point to allow for limited
    ///   movement of the mouse pointer before a drag operation begins. This 
    ///   value is in pixels. It allows the user to click and release the 
    ///   mouse button easily without unintentionally starting a drag 
    ///   operation.
    /// </summary>
    SM_CYDRAG = 69,

    /// <summary>
    ///   Nonzero if the user requires an application to present information
    ///   visually in situations where it would otherwise present the 
    ///   information only in audible form; zero otherwise.
    /// </summary>
    SM_SHOWSOUNDS = 70,

    /// <summary>
    ///   Width of the default menu check-mark bitmap, in pixels.
    /// </summary>
    SM_CXMENUCHECK = 71,

    /// <summary>
    ///   Height of the default menu check-mark bitmap, in pixels.
    /// </summary>
    SM_CYMENUCHECK = 72,

    /// <summary>
    ///   Nonzero if the computer has a low-end (slow) processor; 
    ///   zero otherwise.
    /// </summary>
    SM_SLOWMACHINE = 73,

    /// <summary>
    ///   Nonzero if the system is enabled for Hebrew and Arabic languages,
    ///   zero if not.
    /// </summary>
    SM_MIDEASTENABLED = 74,

    /// <summary>
    ///   Nonzero if a mouse is installed; zero otherwise. This value is 
    ///   rarely zero, because of support for virtual mice and because some 
    ///   systems detect the presence of the port instead of the presence of
    ///   a mouse.
    /// </summary>
    SM_MOUSEPRESENT = 19,

    /// <summary>
    ///   Windows 2000 (v5.0+) Coordinate of the top of the virtual screen.
    /// </summary>
    SM_XVIRTUALSCREEN = 76,

    /// <summary>
    ///   Windows 2000 (v5.0+) Coordinate of the left of the virtual screen.
    /// </summary>
    SM_YVIRTUALSCREEN = 77,

    /// <summary>
    ///   Windows 2000 (v5.0+) Width of the virtual screen.
    /// </summary>
    SM_CXVIRTUALSCREEN = 78,

    /// <summary>
    ///   Windows 2000 (v5.0+) Height of the virtual screen.
    /// </summary>
    SM_CYVIRTUALSCREEN = 79,

    /// <summary>
    ///   Number of display monitors on the desktop.
    /// </summary>
    SM_CMONITORS = 80,

    /// <summary>
    ///   Windows XP (v5.1+) Nonzero if all the display monitors have the 
    ///   same color format, zero otherwise. Note that two displays can have
    ///   the same bit depth, but different color formats. For example, the 
    ///   red, green, and blue pixels can be encoded with different numbers
    ///   of bits, or those bits can be located in different places in a 
    ///   pixel's color value.
    /// </summary>
    SM_SAMEDISPLAYFORMAT = 81,

    /// <summary>
    ///   Windows XP (v5.1+) Nonzero if Input Method Manager/Input Method 
    ///   Editor features are enabled; zero otherwise.
    /// </summary>
    SM_IMMENABLED = 82,

    /// <summary>
    ///   Windows XP (v5.1+) Width of the left and right edges of the focus 
    ///   rectangle drawn by DrawFocusRect. This value is in pixels.
    /// </summary>
    SM_CXFOCUSBORDER = 83,

    /// <summary>
    ///   Windows XP (v5.1+) Height of the top and bottom edges of the focus 
    ///   rectangle drawn by DrawFocusRect. This value is in pixels.
    /// </summary>
    SM_CYFOCUSBORDER = 84,

    /// <summary>
    ///   Nonzero if the current operating system is the Windows XP Tablet PC 
    ///   edition, zero if not.
    /// </summary>
    SM_TABLETPC = 86,

    /// <summary>
    ///   Nonzero if the current operating system is the Windows XP, Media 
    ///   Center Edition, zero if not.
    /// </summary>
    SM_MEDIACENTER = 87,

    /// <summary>
    ///   Metrics Other
    /// </summary>
    SM_CMETRICS_OTHER = 76,

    /// <summary>
    ///   Metrics Windows 2000
    /// </summary>
    SM_CMETRICS_2000 = 83,

    /// <summary>
    ///   Metrics Windows NT
    /// </summary>
    SM_CMETRICS_NT = 88,

    /// <summary>
    ///   Windows XP (v5.1+) This system metric is used in a Terminal 
    ///   Services environment. If the calling process is associated with a 
    ///   Terminal Services client session, the return value is nonzero. If 
    ///   the calling process is associated with the Terminal Server console 
    ///   session, the return value is zero. The console session is not 
    ///   necessarily the physical console - see WTSGetActiveConsoleSessionId 
    ///   for more information.
    /// </summary>
    SM_REMOTESESSION = 0x1000,

    /// <summary>
    ///   Windows XP (v5.1+) Nonzero if the current session is shutting down; 
    ///   zero otherwise.
    /// </summary>
    SM_SHUTTINGDOWN = 0x2000,

    /// <summary>
    ///   Windows XP (v5.1+) This system metric is used in a Terminal 
    ///   Services environment. Its value is nonzero if the current session 
    ///   is remotely controlled; zero otherwise.
    /// </summary>
    SM_REMOTECONTROL = 0x2001,
  }

  #endregion

  #region class SafeNativeMethods

  internal static class SafeNativeMethods
  {
    [DllImport("user32.dll", SetLastError = true)]
    internal static extern int GetSystemMetrics(SystemMetric smIndex);
  }

  #endregion
}