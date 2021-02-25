using System.Collections.Generic;

namespace Sobees.Library.BFacebookLibV1.BindingHelper
{
  /// <summary>
    /// Represents a photo collection
    /// </summary>
    public sealed class FacebookPhotoCollection : FacebookDataCollection<FacebookPhoto>
    {
        /// <summary>
        /// Initializes photo collection object
        /// </summary>
        internal FacebookPhotoCollection()
            : base()
        {
        }

        /// <summary>
        /// Initializes photo collection object from list of photos
        /// </summary>
        /// <param name="photos"></param>
        internal FacebookPhotoCollection(IEnumerable<FacebookPhoto> photos)
            : base(photos)
        {
        }

    }
}
