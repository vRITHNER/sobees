using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Sobees.Library.BFacebookLibV1.Schema.Graph
{
    /// <summary>
    /// Photo collection class
    /// </summary>
    [DataContract]
    public class VideoCollection : GraphDataObject
    {
        [DataMember(Name = "data")]
        public List<Video> Data { get; set; }
    }
    /// <summary>
    /// Class containing photo information
    /// </summary>
    [DataContract]
    public class Video : GraphDataObject
    {
        /// <summary>
        /// Photo Id
        /// </summary>
        [DataMember(Name = "id")]
        public string VideoId
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
        /// Description
        /// </summary>
        [DataMember(Name = "description")]
        public string Description
        {
            get;
            set;
        }


        /// <summary>
        /// Length
        /// </summary>
        [DataMember(Name = "length")]
        public long Length
        {
            get;
            set;
        }
        /// <summary>
        /// Created Time of the photo
        /// </summary>
        [DataMember(Name = "created_time")]
        public string CreatedTime
        {
            get;
            set;
        }


        /// <summary>
        /// updated time
        /// </summary>
        [DataMember(Name = "updated_time")]
        public string UpdatedTime
        {
            get;
            set;
        }

    }

}
