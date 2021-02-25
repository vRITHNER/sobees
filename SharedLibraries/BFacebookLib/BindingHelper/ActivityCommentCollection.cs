using System.Collections.Generic;
using System.Linq;
using Sobees.Library.BFacebookLibV1.Schema;
using Sobees.Library.BFacebookLibV1.Utility;

namespace Sobees.Library.BFacebookLibV1.BindingHelper
{
    /// <summary>
    /// Defines a collection of ActivityComment object
    /// </summary>
    public sealed class ActivityCommentCollection : FacebookDataCollection<ActivityComment>
    {
        /// <summary>
        /// Initializes ActivityCommentCollection object
        /// </summary>
        internal ActivityCommentCollection()
            : base()
        {
        }

        /// <summary>
        /// Initializes ActivityCommentCollection from comments list
        /// </summary>
        /// <param name="comments">list of comments</param>
        internal ActivityCommentCollection(IEnumerable<ActivityComment> comments)
            : base(comments)
        {
        }

        /// <summary>
        /// Initializes ActivityCommentCollection from comments list
        /// </summary>
        /// <param name="comments">comments list</param>
        internal ActivityCommentCollection(IEnumerable<comment> comments)
            : this(comments == null ? null : from c in comments select new ActivityComment(c))
        {
        }

        /// <summary>
        /// Initializes ActivityCommentCollection from comments list
        /// </summary>
        /// <param name="comments">comments list</param>
        internal ActivityCommentCollection(IEnumerable<FacebookComment> comments)
            : this(comments == null ? null : from c in comments select new ActivityComment(c))
        {
        }
    }
}
