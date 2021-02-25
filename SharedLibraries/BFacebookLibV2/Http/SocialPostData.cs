namespace Sobees.Library.BFacebookLibV2.Http
{
  using System;
  using System.Collections.Specialized;
  using System.Globalization;
  using System.Linq;
  using Sobees.Library.BFacebookLibV2.Cls;

  /// <summary>
  /// A wrapper class extending the functionality of <code>NameValueCollection</code>.
  /// </summary>
  public class SocialQueryString
  {

    #region Private fields

    private readonly NameValueCollection _nvc = new NameValueCollection();

    #endregion

    #region Properties

    /// <summary>
    /// Gets a reference to the internal <code>NameValueCollection</code>.
    /// </summary>
    public NameValueCollection NameValueCollection => _nvc;

    /// <summary>
    /// Gets the number of key/value pairs contained in the internal <code>NameValueCollection</code> instance.
    /// </summary>
    public int Count => _nvc.Count;

    /// <summary>
    /// Gets whether the internal <code>NameValueCollection</code> is empty.
    /// </summary>
    public bool IsEmpty => _nvc.Count == 0;

    #endregion

    #region Constructors

    public SocialQueryString() { }

    public SocialQueryString(NameValueCollection nvc)
    {
      _nvc = nvc ?? new NameValueCollection();
    }

    #endregion

    #region Methods

    public void Add(string key, object value)
    {
      _nvc.Add(key, String.Format(CultureInfo.InvariantCulture, "{0}", value));
    }

    public void Set(string key, object value)
    {
      _nvc.Add(key, String.Format(CultureInfo.InvariantCulture, "{0}", value));
    }

    public override string ToString()
    {
      return SocialUtils.NameValueCollectionToQueryString(_nvc);
    }

    public bool ContainsKey(string key)
    {
      return _nvc.Get(key) != null || _nvc.AllKeys.Contains(key);
    }

    // TODO: Determine which methods from NameValueCollection that also should be exposed in this class

    #endregion

    #region Operator overloading

    public static implicit operator SocialQueryString(NameValueCollection query)
    {
      return new SocialQueryString(query);
    }

    #endregion

  }

}
