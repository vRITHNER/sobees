using System.Windows;
using System.Windows.Controls;

namespace Sobees.Infrastructure.Cls
{
  public class PasswordHelper : DependencyObject
  {
    public static readonly DependencyProperty IsBoundProperty =
      DependencyProperty.RegisterAttached("IsBound", typeof (bool),
                                          typeof (PasswordHelper),
                                          new PropertyMetadata(false));

    public static readonly DependencyProperty TextProperty =
      DependencyProperty.RegisterAttached("Text", typeof (string),
                                          typeof (PasswordHelper),
                                          new PropertyMetadata(string.Empty,
                                                               (sender, args) =>
                                                                 {
                                                                   var pwdBox = sender as PasswordBox;
                                                                   if (pwdBox == null)
                                                                     return;
                                                                   string nval = (args.NewValue as string) ??
                                                                                 string.Empty;
                                                                   if (pwdBox.Password != nval)
                                                                     pwdBox.Password = nval;
                                                                   if (!GetIsBound(pwdBox))
                                                                   {
                                                                     pwdBox.PasswordChanged +=
                                                                       delegate { SetText(pwdBox, pwdBox.Password); };
                                                                     SetIsBound(pwdBox, true);
                                                                   }
                                                                 }));

    /// <summary>
    /// Indicates if a PasswordBox is DataBound
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static bool GetIsBound(PasswordBox obj)
    {
      return (bool) obj.GetValue(IsBoundProperty);
    }

    private static void SetIsBound(PasswordBox obj, bool value)
    {
      obj.SetValue(IsBoundProperty, value);
    }


    /// <summary>
    /// Text of the PasswordBox (bindable vision of Password property)
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static string GetText(PasswordBox obj)
    {
      return (string) obj.GetValue(TextProperty);
    }

    public static void SetText(PasswordBox obj, string value)
    {
      obj.SetValue(TextProperty, value);
    }
  }
}