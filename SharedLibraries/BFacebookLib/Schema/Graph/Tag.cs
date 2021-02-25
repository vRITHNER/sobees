using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Sobees.Library.BFacebookLibV1.Schema.Graph
{
    /// <summary>
    /// Photo collection class
    /// </summary>
    [DataContract]
    public class TagCollection : GraphDataObject
    {
        [DataMember(Name = "data")]
        public List<Tag> Data { get; set; }
    }
    /// <summary>
    /// Class containing photo information
    /// </summary>
    [DataContract]
    public class Tag : GraphDataObject
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
        /// Url of the photo
        /// </summary>
        [DataMember(Name = "name")]
        public string Name
        {
            get;
            set;
        }
        /// <summary>
        /// Url of the photo
        /// </summary>
        [DataMember(Name = "x")]
        public decimal X
        {
            get;
            set;
        }
        /// <summary>
        /// Url of the photo
        /// </summary>
        [DataMember(Name = "y")]
        public decimal Y
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

    }

}
