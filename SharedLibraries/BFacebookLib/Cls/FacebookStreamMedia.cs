using System.Collections.Generic;

namespace Sobees.Library.BFacebookLibV1.Cls
{
  ///<summary>
  ///</summary>
  public class FacebookStreamMedia
  {
    ///<summary>
    ///</summary>
    public string Href { get; set; }

    ///<summary>
    ///</summary>
    public string Alt { get; set; }

    ///<summary>
    ///</summary>
    public string Type { get; set; }

    ///<summary>
    ///</summary>
    public string Src { get; set; }

    ///<summary>
    ///</summary>
    public List<FacebookStreamPhoto> Photos { get; set; }

    ///<summary>
    ///</summary>
    public List<FacebookStreamVideo> Videos { get; set; }
  }
}