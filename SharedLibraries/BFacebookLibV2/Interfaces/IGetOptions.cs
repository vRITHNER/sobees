namespace Sobees.Library.BFacebookLibV2.Interfaces
{
  using Sobees.Library.BFacebookLibV2.Http;

  public interface IGetOptions
  {

    #region Methods

    /// <summary>
    /// Gets an instance of <code>SocialQueryString</code> representing the GET parameters.
    /// </summary>
    SocialQueryString GetQueryString();

    #endregion

  }

}