using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Sobees.Library.BFacebookLibV1.Schema.Graph
{
    /// <summary>
    /// Photo collection class
    /// </summary>
    [DataContract]
    public class FacebookCommentCollection : GraphDataObject
    {
        [DataMember(Name = "data")]
        public List<FacebookComment> Data { get; set; }
    }

    /// <summary>
    /// Class containing photo information
    /// </summary>
    [DataContract]
    public class FacebookComment : GraphDataObject
    {
        /// <summary>
        /// Photo Id
        /// </summary>
        [DataMember(Name = "id")]
        public string Id
        {
            get;
            set;
        }
        /// <summary>
        /// Photo Id
        /// </summary>
        [DataMember(Name = "from")]
        public FacebookItem From
        {
            get;
            set;
        }
        /// <summary>
        /// Message
        /// </summary>
        [DataMember(Name = "message")]
        public string Message
        {
            get;
            set;
        }
        /// <summary>
        /// Url of the photo
        /// </summary>
        [DataMember(Name = "created_time")]
        public string CreatedTime
        {
            get;
            set;
        }
    }

}
