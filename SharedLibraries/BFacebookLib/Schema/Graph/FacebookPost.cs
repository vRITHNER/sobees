using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Sobees.Library.BFacebookLibV1.Schema.Graph
{
    /// <summary>
    /// Photo collection class
    /// </summary>
    [DataContract]
    public class FacebookPostCollection : GraphDataObject
    {
        [DataMember(Name = "data")]
        public List<FacebookPost> Data { get; set; }
    }

    /// <summary>
    /// Class containing photo information
    /// </summary>
    [DataContract]
    public class FacebookPost : GraphDataObject
    {
        /// <summary>
        /// Post Id
        /// </summary>
        [DataMember(Name = "id")]
        public string PostId
        {
            get;
            set;
        }
        /// <summary>
        /// From
        /// </summary>
        [DataMember(Name = "from")]
        public IdNamePair From
        {
            get;
            set;
        }
        /// <summary>
        /// From
        /// </summary>
        [DataMember(Name = "to")]
        public PostToCollection To
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
        [DataMember(Name = "picture")]
        public string Picture
        {
            get;
            set;
        }


        /// <summary>
        /// link
        /// </summary>
        [DataMember(Name = "link")]
        public string Link
        {
            get;
            set;
        }
        /// <summary>
        /// name
        /// </summary>
        [DataMember(Name = "name")]
        public string Name
        {
            get;
            set;
        }
        /// <summary>
        /// caption
        /// </summary>
        [DataMember(Name = "caption")]
        public string Caption
        {
            get;
            set;
        }
        /// <summary>
        /// icon
        /// </summary>
        [DataMember(Name = "icon")]
        public string Icon
        {
            get;
            set;
        }
        /// <summary>
        /// description
        /// </summary>
        [DataMember(Name = "description")]
        public string Description
        {
            get;
            set;
        }
        /// <summary>
        /// source
        /// </summary>
        [DataMember(Name = "source")]
        public string Source
        {
            get;
            set;
        }
        /// <summary>
        /// source
        /// </summary>
        [DataMember(Name = "type")]
        public string Type
        {
            get;
            set;
        }
        /// <summary>
        /// Attribution
        /// </summary>
        [DataMember(Name = "attribution")]
        public string Attribution
        {
            get;
            set;
        }
        /// <summary>
        /// Attribution
        /// </summary>
        [DataMember(Name = "likes")]
        public long Likes
        {
            get;
            set;
        }
        /// <summary>
        /// Actions
        /// </summary>
        [DataMember(Name = "actions")]
        public List<LinkNamePair> Actions
        {
            get;
            set;
        }
        /// <summary>
        /// Actions
        /// </summary>
        [DataMember(Name = "comments")]
        public FacebookCommentCollection Comments
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
        [DataMember(Name = "updated_time")]
        public string UpdatedTime
        {
            get;
            set;
        }

    }

    public class PostToCollection
    {
        /// <summary>
        /// Url of the photo
        /// </summary>
        [DataMember(Name = "data")]
        public List<IdNamePair> Data
        {
            get;
            set;
        }
    }

}
