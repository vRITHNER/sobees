#region

using System;
using System.Reflection;
using Sobees.Tools.Logging;

#endregion

namespace Sobees.Tools.Util
{
  public class AssemblyHelper
  {
    private static readonly string Assemblylocation = string.Empty;
    private static readonly string AssemblyName = string.Empty;
    private static readonly string AssemblyFullName = string.Empty;
    private static readonly string AssemblyVersion = string.Empty;

#if !SILVERLIGHT
    private static readonly Assembly EntryAssembly = Assembly.GetEntryAssembly();
#else
    private static Assembly EntryAssembly = Assembly.GetExecutingAssembly();
#endif

    public static string GetEntryAssemblyLocation()
    {
      try
      {
        var value = Assemblylocation == string.Empty ? EntryAssembly.Location : Assemblylocation;
        return value;
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("AssemblyHelper::GetEntryAssemblyLocation:", (ex));
      }
      return Assemblylocation;
    }

    public static string GetEntryAssemblyName()
    {
      try
      {
        var value = AssemblyName == string.Empty ? EntryAssembly.GetName().Name : AssemblyName;
        return value;
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("AssemblyHelper::GetEntryAssemblyName:", (ex));
      }
      return AssemblyName;
    }


    public static string GetEntryAssemblyFullName()
    {
      try
      {
        var value = AssemblyFullName == string.Empty ? EntryAssembly.GetName().FullName : AssemblyFullName;
        return value;
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("AssemblyHelper::GetEntryAssemblyFullName:", (ex));
      }
      return AssemblyFullName;
    }

    public static string GetEntryAssemblyVersion()
    {
      try
      {
        var value = AssemblyVersion == string.Empty ? EntryAssembly.GetName().Version.ToString() : AssemblyVersion;
        return value;
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("AssemblyHelper::GetEntryAssemblyVersion:", (ex));
      }
      return AssemblyVersion;
    }


    public static string GetExecutingAssemblyName()
    {
      try
      {
        return Assembly.GetExecutingAssembly().GetName().Name;
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("AssemblyHelper::GetExecutingAssemblyName:", (ex));
      }
      return string.Empty;
    }

    public static string GetExecutingAssemblyFullName()
    {
      try
      {
        return Assembly.GetExecutingAssembly().GetName().FullName;
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("AssemblyHelper::GetExecutingAssemblyFullName:", (ex));
      }
      return string.Empty;
    }
  }
}