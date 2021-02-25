#region Includes

using System.Collections.Generic;

#endregion

namespace Sobees.Library.BFacebookLibV1.Cls
{
  ///<summary>
  ///</summary>
  public class FacebookAttachement
  {
    ///<summary>
    ///</summary>
    public string Name { get; set; }

    ///<summary>
    ///</summary>
    public string Href { get; set; }

    ///<summary>
    ///</summary>
    public string Caption { get; set; }

    ///<summary>
    ///</summary>
    public string Description { get; set; }

    ///<summary>
    ///</summary>
    public List<FacebookStreamMedia> Medias { get; set; }
    ///<summary>
    ///</summary>
    public List<FacebookMediaProperty> Properties { get; set; }
  }
}