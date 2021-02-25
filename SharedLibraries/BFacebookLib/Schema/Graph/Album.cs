using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Sobees.Library.BFacebookLibV1.Schema.Graph
{
    /// <summary>
    /// Represents a collection of FacebookAlbum
    /// </summary>
    [DataContract]
    public class AlbumCollection : GraphDataObject
    {
        [DataMember(Name = "data")]
        public List<Album> Data { get; set; }
    }

    /// <summary>
    /// Class containing information about facebook album
    /// </summary>
    [DataContract]
    public class Album : GraphDataObject
    {
        /// <summary>
        /// Album id
        /// </summary>
        [DataMember(Name = "id")]
        public string AlbumId
        {
            get;
            internal set;
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
        /// Name of album
        /// </summary>
        [DataMember(Name = "name")]
        public string Name
        {
            get;
            internal set;
        }



        /// <summary>
        /// Created time of album
        /// </summary>
        [DataMember(Name = "created_time")]
        public string CreatedTime
        {
            get;set;
        }



        /// <summary>
        /// Modified time of album
        /// </summary>
        [DataMember(Name = "modified_time")]
        public string ModifiedTime
        {
            get;set;
        }

        /// <summary>
        /// Description of album
        /// </summary>
        [DataMember(Name = "description")]
        public string Description
        {
            get;
            internal set;
        }

        /// <summary>
        /// Location information
        /// </summary>
        [DataMember(Name = "location")]
        public string Location
        {
            get;
            internal set;
        }

        /// <summary>
        /// Link information
        /// </summary>
        [DataMember(Name = "link")]
        public string Link
        {
            get;
            internal set;
        }

        /// <summary>
        /// Number of photos in the album
        /// </summary>
        [DataMember(Name = "count")]
        public int Count
        {
            get;
            internal set;
        }


    }
}
