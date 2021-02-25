#region

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;

#endregion

namespace Sobees.Tools.Helpers
{
  public class BDesignerHelper
  {
    private static bool? _isInDesignMode;

    /// <summary>
    ///   Gets a value indicating whether the control is in design mode (running in Blend
    ///   or Visual Studio).
    /// </summary>
    [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands", Justification = "The security risk here is neglectible.")]
    public static bool IsInDesignModeStatic
    {
      get
      {
        if (!_isInDesignMode.HasValue)
        {
#if SILVERLIGHT
                    _isInDesignMode = DesignerProperties.IsInDesignTool;
#else
          var prop = DesignerProperties.IsInDesignModeProperty;
          _isInDesignMode = (bool) DependencyPropertyDescriptor.FromProperty(prop, typeof (FrameworkElement)).Metadata.DefaultValue;

          // Just to be sure
          if (!_isInDesignMode.Value && Process.GetCurrentProcess().ProcessName.StartsWith("devenv", StringComparison.Ordinal))
          {
            _isInDesignMode = true;
          }
#endif
        }

        return _isInDesignMode.Value;
      }
    }
  }
}