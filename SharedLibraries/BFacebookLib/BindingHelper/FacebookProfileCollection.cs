using System.Collections.Generic;

namespace Sobees.Library.BFacebookLibV1.BindingHelper
{
    /// <summary>
    /// Represents collection of profile object
    /// </summary>
    public sealed class FacebookProfileCollection : FacebookDataCollection<FacebookProfile>
    {
                /// <summary>
        /// Initializes FacebookContactCollection object
        /// </summary>
        internal FacebookProfileCollection()
            : base()
        {
        }

        /// <summary>
        /// Initializes FacebookContactCollection from contacts list
        /// </summary>
        /// <param name="profiles">list of profiles</param>
        internal FacebookProfileCollection(IEnumerable<FacebookProfile> profiles)
            : base(profiles)
        {
        }
    }
}
