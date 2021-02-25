using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Sobees.Library.BFacebookLibV1.Schema.Graph
{
    /// <summary>
    /// Represents Group collection
    /// </summary>
    [DataContract]
    public class FacebookLinkCollection : GraphDataObject
    {
        [DataMember(Name = "data")]
        public List<FacebookLink> Data { get; set; }
    }    
    /// <summary>
    /// Class containing Event information
    /// </summary>
    [DataContract]
    public class FacebookLink : GraphDataObject
    {
        /// <summary>
        /// Event id
        /// </summary>
        [DataMember(Name = "id")]
        public long LinkId
        {
            get;
            set;
        }
        /// <summary>
        /// Owner
        /// </summary>
        [DataMember(Name = "from")]
        public IdNamePair From
        {
            get;
            set;
        }

        /// <summary>
        /// Event name
        /// </summary>
        [DataMember(Name = "link")]
        public string Link
        {
            get;
            set;
        }
        /// <summary>
        /// Event description
        /// </summary>
        [DataMember(Name = "message")]
        public string Message
        {
            get;
            set;
        }
        /// <summary>
        /// Event description
        /// </summary>
        [DataMember(Name = "updated_time")]
        public string UpdatedTime
        {
            get;
            set;
        }
    }

}
