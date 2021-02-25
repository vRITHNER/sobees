using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Sobees.Library.BFacebookLibV1.Schema.Graph
{
    /// <summary>
    /// Photo collection class
    /// </summary>
    [DataContract]
    public class PhotoCollection : GraphDataObject
    {
        [DataMember(Name = "data")]
        public List<Photo> Data { get; set; }
    }
    /// <summary>
    /// Class containing photo information
    /// </summary>
    [DataContract]
    public class Photo : GraphDataObject
    {
        /// <summary>
        /// Photo Id
        /// </summary>
        [DataMember(Name = "id")]
        public string PhotoId
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
        [DataMember(Name = "picture")]
        public string Picture
        {
            get;
            set;
        }


        /// <summary>
        /// Url of the photo
        /// </summary>
        [DataMember(Name = "src")]
        public string Source
        {
            get;
            set;
        }
        /// <summary>
        /// Height of the photo
        /// </summary>
        [DataMember(Name = "height")]
        public long Height
        {
            get;
            set;
        }
        /// <summary>
        /// Width of the photo
        /// </summary>
        [DataMember(Name = "width")]
        public string Width
        {
            get;
            set;
        }
        /// <summary>
        /// Url of the photo
        /// </summary>
        [DataMember(Name = "link")]
        public string Link
        {
            get;
            set;
        }


        /// <summary>
        /// Url of the photo
        /// </summary>
        [DataMember(Name = "icon")]
        public string Icon
        {
            get;
            set;
        }
        /// <summary>
        /// Tags
        /// </summary>
        [DataMember(Name = "tags")]
        public TagCollection Tags
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
        /// Url of the photo
        /// </summary>
        [DataMember(Name = "modified_time")]
        public string ModifiedTime
        {
            get;
            set;
        }

    }

}
