#region

using System.Windows;

#endregion

namespace Sobees.Infrastructure.Windows.Extensions
{
  public static class BWindowExtension
  {
    public static void SizeToFit(this Window window)
    {
      if (window.Height > SystemParameters.VirtualScreenHeight)
        window.Height = SystemParameters.VirtualScreenHeight;

      if (window.Width > SystemParameters.VirtualScreenWidth)
        window.Width = SystemParameters.VirtualScreenWidth;
    }

    public static void MoveIntoView(this Window window)
    {
      if (window.Top + window.Height / 2 > SystemParameters.VirtualScreenHeight)
        window.Top = SystemParameters.VirtualScreenHeight - window.Height;

      if (window.Left + window.Width / 2 > SystemParameters.VirtualScreenWidth)
        window.Left = SystemParameters.VirtualScreenWidth - window.Width;

      if (window.Top < 0)
        window.Top = 0;

      if (window.Left < 0)
        window.Left = 0;

    }


    
  }

 
}