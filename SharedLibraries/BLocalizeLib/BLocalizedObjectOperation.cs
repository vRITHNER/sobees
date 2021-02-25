using System;
using System.Reflection;

namespace Sobees.Library.BLocalizeLib
{
  public static class BLocalizedObjectOperation
  {
    /// <exception cref="ArgumentNullException"><c>assembly</c> is null.</exception>
    /// <exception cref="ArgumentException">assembly is empty</exception>
    public static string GetString(string assembly, string dictionary, string key)
    {

      if (assembly == null) throw new ArgumentNullException("assembly");
      if (assembly == string.Empty) throw new ArgumentException("assembly is empty", "assembly");

      if (dictionary == null) throw new ArgumentNullException("dictionary");
      if (dictionary == string.Empty) throw new ArgumentException("dictionary is empty", "dictionary");

      if (key == null) throw new ArgumentNullException("key");
      if (key == string.Empty) throw new ArgumentException("key is empty", "key");

      try
      {
        return (string)BLocalizeDictionary.Instance.GetLocalizedObject<object>(
                         assembly, dictionary, key, BLocalizeDictionary.Instance.Culture);
      }
      catch
      {
        return string.Format("No resource key with name '{0}' in dictionary '{1}' in assembly '{2}' founded! ({2}.{1}.{0})",
                             key, dictionary, assembly);
      }
    }

    /// <exception cref="ArgumentNullException"><c>dictionary</c> is null.</exception>
    /// <exception cref="ArgumentException">key is empty</exception>
    public static string GetString(string dictionary, string key)
    {

      if (dictionary == null) throw new ArgumentNullException("dictionary");
      if (dictionary == string.Empty) throw new ArgumentException("dictionary is empty", "dictionary");

      if (key == null) throw new ArgumentNullException("key");
      if (key == string.Empty) throw new ArgumentException("key is empty", "key");

      string assembly = BLocalizeDictionary.Instance.GetAssemblyName(Assembly.GetExecutingAssembly());

      try
      {
        return (string)BLocalizeDictionary.Instance.GetLocalizedObject<object>(
                         assembly, dictionary, key, BLocalizeDictionary.Instance.Culture);
      }
      catch
      {
        return string.Format("No resource key with name '{0}' in dictionary '{1}' in assembly '{2}' founded! ({2}.{1}.{0})",
                             key, dictionary, assembly);
      }
    }
  }
}