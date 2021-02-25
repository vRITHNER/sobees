#region Includes

using System.Windows;
using System.Windows.Controls;

#endregion

namespace Sobees.Infrastructure.Controls
{
  public class BTextBox : TextBox
  {
    public static DependencyProperty CursorPositionProperty = DependencyProperty.Register("CursorPosition", typeof(int),
                                                                                          typeof(BTextBox), null);

    public static DependencyProperty MaxCharactersProperty = DependencyProperty.Register("MaxCharacters", typeof(int),
                                                                                         typeof(BTextBox), null);

    public static DependencyProperty MoveCursorProperty = DependencyProperty.Register("MoveCursor", typeof(bool),
                                                                                      typeof(BTextBox), null);

    public BTextBox()
    {
      TextChanged += BTextBoxTextChanged;
    }


    public int MaxCharacters
    {
      get
      {
        return GetValue(MaxCharactersProperty) == null ? 0 : int.Parse(GetValue(MaxCharactersProperty).ToString());
      }
      set { SetValue(MaxCharactersProperty, value); }
    }

    public int CursorPosition
    {
      get
      {
        if (GetValue(CursorPositionProperty) == null)
          return 0;

        return int.Parse(GetValue(CursorPositionProperty).ToString());
      }
      set { SetValue(CursorPositionProperty, value); }
    }

    public bool MoveCursor
    {
      get
      {
        if (GetValue(MoveCursorProperty) == null)
          return false;

        return bool.Parse(GetValue(MoveCursorProperty).ToString());
      }
      set { SetValue(MoveCursorProperty, value); }
    }


    private void BTextBoxTextChanged(object sender, TextChangedEventArgs e)
    {
      #if(SILVERLIGHT) //TODO:redo TextChanged to be handled in behaviour
            Focus(); //Cause a stackoverflow error in WPF dure to watermark behaviour
      #endif
      if (MoveCursor)
      {
          if (CursorPosition == -1) 
          {
              CursorPosition = 0;
          }
#if !SILVERLIGHT
          CaretIndex = CursorPosition;
#endif
        Select(CursorPosition, 0);
        MoveCursor = false;
      }
      if (MaxCharacters > 0)
      {
        Tag = Text.Length > MaxCharacters ? null : "OK";
      }
    }
  }
}