using Sobees.Library.BGenericLib;

namespace Sobees.Library.BFacebookLibV1.Cls
{
  public class FacebookEntry : Entry
  {
    private Like _likeFacebook;
    private int _nbComments;
  	private int _nbLikes;

    public string AppId { get; set; }

    public int NbComments
    {
      get { return _nbComments; }
      set
      {
        if (value == _nbComments)
          return;

        _nbComments = value;
        OnPropertyChanged("NbComments");
      }
    }

		public int NbLikes
		{
			get { return _nbLikes; }
			set
			{
				if (value == _nbLikes)
					return;

				_nbLikes = value;
				OnPropertyChanged("NbLikes");
			}
		}

    public int CanPost { get; set; }

    public Like LikeFacebook
    {
      get { return _likeFacebook; }
      set
      {
        //if (value == _likeFacebook)
        //  return;

        _likeFacebook = value;
        OnPropertyChanged("LikeFaceBook");
      }
    }
#if SILVERLIGHT
		public EnumPostType FbPostType
		{
			get; set;
		}
#endif
    public FacebookAttachement Attachement { get; set; }
  }
}