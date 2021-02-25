using System.Collections.Generic;

namespace Sobees.Library.BFacebookLibV1.BindingHelper
{
  /// <summary>
    /// Represents a album collection
    /// </summary>
    public sealed class FacebookPhotoAlbumCollection : FacebookDataCollection<FacebookPhotoAlbum>
    {
        /// <summary>
        /// Initializes FacebookPhotoAlbumCollection object
        /// </summary>
        public FacebookPhotoAlbumCollection()
            : base()
        {
        }

        /// <summary>
        /// Initializes FacebookPhotoAlbumCollection object from list of albums
        /// </summary>
        /// <param name="albums">album list</param>
        public FacebookPhotoAlbumCollection(IEnumerable<FacebookPhotoAlbum> albums)
            : base(albums)
        {
        }

    }
}
