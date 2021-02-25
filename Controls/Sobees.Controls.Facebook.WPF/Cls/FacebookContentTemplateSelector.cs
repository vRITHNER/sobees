#region

using System;
using System.Windows;
using System.Windows.Controls;
using Sobees.Library.BFacebookLibV2.Objects.Feed;
using Sobees.Tools.Logging;

#endregion

namespace Sobees.Controls.Facebook.Cls
{
  public class FacebookContentTemplateSelector : DataTemplateSelector
  {
    public DataTemplate CtclAppTemplate { get; set; }
    public DataTemplate CtclDefaultTemplate { get; set; }
    public DataTemplate CtclVideoTemplate { get; set; }
    public DataTemplate CtclLinkTemplate { get; set; }
    public DataTemplate CtclAlbumTemplate { get; set; }
    public DataTemplate CtclEventTemplate { get; set; }
    public DataTemplate CtclGroupTemplate { get; set; }

    public override DataTemplate SelectTemplate(object item,
      DependencyObject container)
    {
      if (item == null) return CtclDefaultTemplate;
      var entry = item as FacebookFeedEntry;
      if (entry != null)
        switch ( Convert.ToInt32(entry.Type))
        {
          case 237:
            return CtclAppTemplate;
          case 46:
            return CtclDefaultTemplate;
          case 164:
            return CtclDefaultTemplate;
          case 137:
            return CtclVideoTemplate;
          case 80:
            return CtclVideoTemplate;
          case 236: //Note
            return CtclLinkTemplate;
          case 56:
            return CtclDefaultTemplate;
          case 0:
            return CtclAlbumTemplate;
          case 247:
            return CtclAlbumTemplate;
          case 11: //group post by creator
            return CtclGroupTemplate;
          case 12: // event by author
            return CtclEventTemplate;
          case 94: // event by member
            return CtclEventTemplate;
          case 92: //group by member
            return CtclGroupTemplate;
          case 66: //Note
            return CtclGroupTemplate;
          case 81: //Note
            return CtclGroupTemplate;
          case 79: //Album Share
            return CtclAlbumTemplate;
          case 128: //Video
            return CtclVideoTemplate;
          case 82: //Album
            return CtclAlbumTemplate;
          case 259: //New Picture profil
            return CtclAlbumTemplate;
          case 83: //Link to an app
            return CtclVideoTemplate;
          default:
            TraceHelper.Trace(this, "Facebook-> Nouveau format pour les posts... TODO" + entry.Type);
            break;
        }

      return CtclDefaultTemplate;
    }
  }
}