#region

using System.Windows.Input;
using BUtility;

#endregion

namespace Sobees.Tools.KeysHelper
{
  public class KeysHelper
  {
    public static bool CheckEnterKey(object[] objs)
    {
      var kea = objs[2] as KeyEventArgs;
      return CheckEnterKey(kea);
    }

    public static bool CheckEnterKey(KeyEventArgs kea)
    {
      if (kea == null)
      {
        BLogManager.LogEntry("Helper::CheckEnterKey()", "KeyEventArgs was NULL.");
        return false;
      }
      return kea.Key.Equals(Key.Enter);
    }
  }
}