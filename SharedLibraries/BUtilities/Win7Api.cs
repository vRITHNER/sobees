#region

using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security;

#endregion

namespace Sobees.Library.BUtilities
{
  public class Win7Api
  {
    #region Win7 declarations

    //#define DWM_SIT_DISPLAYFRAME    0x00000001  // Display a window frame around the provided bitmap

    [DllImport("dwmapi.dll", PreserveSig = false)]
    public static extern void DwmInvalidateIconicBitmaps(IntPtr hwnd);

    [DllImport("dwmapi.dll", PreserveSig = false)]
    public static extern void DwmSetIconicThumbnail(IntPtr hwnd, IntPtr hbmp, DWM_SIT dwSITFlags);

    [DllImport("dwmapi.dll", PreserveSig = false)]
    public static extern void DwmSetIconicLivePreviewBitmap(IntPtr hwnd, IntPtr hbmp, RefPOINT pptClient,
                                                            DWM_SIT dwSITFlags);

    [DllImport("shell32.dll", PreserveSig = false)]
    public static extern void SHGetItemFromDataObject(IDataObject pdtobj, DOGIF dwFlags, [In] ref Guid riid,
                                                      [Out, MarshalAs(UnmanagedType.Interface)] out object ppv);

    [DllImport("shell32.dll", PreserveSig = false, EntryPoint = "SHAddToRecentDocs")]
    private static extern void _SHAddToRecentDocsObj(SHARD uFlags, object pv);

    [DllImport("shell32.dll", EntryPoint = "SHAddToRecentDocs")]
    private static extern void _SHAddToRecentDocs_String(SHARD uFlags, [MarshalAs(UnmanagedType.LPWStr)] string pv);

    // This overload is required.  There's a cast in the Shell code that causes the wrong vtbl to be used
    // if we let the marshaller convert the parameter to an IUnknown.
    [DllImport("shell32.dll", EntryPoint = "SHAddToRecentDocs")]
    [SecurityCritical, SuppressUnmanagedCodeSecurity]
    private static extern void _SHAddToRecentDocs_ShellLink(SHARD uFlags, IShellLinkW pv);

    public static void SHAddToRecentDocs(string path)
    {
      _SHAddToRecentDocs_String(SHARD.PATHW, path);
    }

    // Win7 only.
    public static void SHAddToRecentDocs(IShellLinkW shellLink)
    {
      _SHAddToRecentDocs_ShellLink(SHARD.LINK, shellLink);
    }

    public static void SHAddToRecentDocs(SHARDAPPIDINFO info)
    {
      _SHAddToRecentDocsObj(SHARD.APPIDINFO, info);
    }

    public static void SHAddToRecentDocs(SHARDAPPIDINFOIDLIST infodIdList)
    {
      _SHAddToRecentDocsObj(SHARD.APPIDINFOIDLIST, infodIdList);
    }


    [DllImport("shell32.dll", PreserveSig = false)]
    public static extern HRESULT SHCreateItemFromParsingName([MarshalAs(UnmanagedType.LPWStr)] string pszPath,
                                                             IBindCtx pbc, [In] ref Guid riid,
                                                             [Out, MarshalAs(UnmanagedType.Interface)] out object ppv);

    [DllImport("shell32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool Shell_NotifyIcon(NIM dwMessage, [In] NOTIFYICONDATA lpdata);

    /// <summary>
    ///   Sets the User Model AppID for the current process, enabling Windows to retrieve this ID
    /// </summary>
    /// <param name="AppID"></param>
    [DllImport("shell32.dll", PreserveSig = false)]
    public static extern void SetCurrentProcessExplicitAppUserModelID([MarshalAs(UnmanagedType.LPWStr)] string AppID);

    /// <summary>
    ///   Retrieves the User Model AppID that has been explicitly set for the current process via SetCurrentProcessExplicitAppUserModelID
    /// </summary>
    /// <param name="AppID"></param>
    [DllImport("shell32.dll")]
    public static extern HRESULT GetCurrentProcessExplicitAppUserModelID(
      [Out, MarshalAs(UnmanagedType.LPWStr)] out string AppID);

    #endregion
  }
}