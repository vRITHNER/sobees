using Sobees.Library.BFacebookLibV1.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Net;
using System.Web.Script.Serialization;
using System.Xml.Serialization;
#if !SILVERLIGHT
#endif

namespace Sobees.Library.BFacebookLibV1.Schema
{

#pragma warning disable 1591 // Disable "Missing XML comment" warnings for all code in this file


    [XmlRoot("stream_dataPosts_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
    public class stream_dataPosts_response : stream_dataPosts
    { }
    [XmlRoot("stream_dataProfiles_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
    public class stream_dataProfiles_response : stream_dataProfiles
    { }
    [XmlRoot("permissions_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
    public class permissions_response
    { 
        public permissions permissions;
    }
    public class permissions
    {
        public bool read_stream;
        public bool status_update;
        public bool photo_upload;
        public bool publish_stream;
        public bool email;
        public bool create_listing;
        public bool offline_access;
        public bool rsvp_event;
        public bool sms;
        public bool video_upload;
        public bool create_event;
        public bool create_note;
        public bool share_item;
        public bool manage_mailbox;
        public bool read_mailbox;

        public bool bookmarked;  
        public bool tab_added;  

        // New Permissions
        public bool read_insights;
        public bool user_about_me;
        public bool user_activities;
        public bool user_birthday;
        public bool user_education_history;
        public bool user_events;
        public bool user_groups;
        public bool user_hometown;
        public bool user_interests;
        public bool user_likes;
        public bool user_location;
        public bool user_notes;
        public bool user_online_presence;
        public bool user_photo_video_tags;
        public bool user_photos;
        public bool user_relationships;
        public bool user_religion_politics;
        public bool user_status;
        public bool user_videos;
        public bool user_website;
        public bool user_work_history;
        public bool read_friendlists;
        public bool read_requests;
        public bool friends_about_me;
        public bool friends_activities;
        public bool friends_birthday;
        public bool friends_education_history;
        public bool friends_events;
        public bool friends_groups;
        public bool friends_hometown;
        public bool friends_interests;
        public bool friends_likes;
        public bool friends_location;
        public bool friends_notes;
        public bool friends_online_presence;
        public bool friends_photo_video_tags;
        public bool friends_photos;
        public bool friends_relationships;
        public bool friends_religion_politics;
        public bool friends_status;
        public bool friends_videos;
        public bool friends_website;
        public bool friends_work_history;
    }

	[XmlType(AnonymousType = true, Namespace = "http://api.facebook.com/2.0/")]
	public class request_argsLocalType
	{
		[XmlAttribute]
		public bool list { get; set; }

		[XmlIgnore()]
		public bool listSpecified { get; set; }

		[XmlElement("arg")]
		public List<arg> arg { get; set; }
	}

	[XmlType(Namespace = "http://api.facebook.com/2.0/")]
	[XmlRoot("error_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class error_response
	{
		public int error_code {get; set;}
		public string error_msg {get; set;}

		public request_argsLocalType request_args {get; set;}
	}
    public partial class album
    {
        [XmlElement("created", IsNullable=true)]
        public string createdString { 
            get { return this.createdField.ToString(); } 
            set {
                if (value != null)
                    createdField = long.Parse(value);
                else
                    createdField = 0;
            } 
        }
        [XmlElement("modified", IsNullable = true)]
        public string modifiedString
        {
            get { return this.modifiedField.ToString(); }
            set
            {
                if (value != null)
                    modifiedField = long.Parse(value);
                else
                    modifiedField = 0;
            }
        }

    }
    public partial class page
    {
        private string fan_countField;
        [XmlElement(IsNullable = true)]
        public string fan_count
        {
            get
            {
                return this.fan_countField;
            }
            set
            {
                this.fan_countField = value;
            }
        }
    }
	public partial class album
	{
		public DateTime created_date
		{
			get { return DateHelper.ConvertDoubleToDate(Convert.ToDouble(this.created)); }
		}
		public DateTime modified_date
		{
			get { return DateHelper.ConvertDoubleToDate(Convert.ToDouble(this.modified)); }
		}
	}
	public partial class photo_tag : IEquatable<photo_tag>
	{
		public DateTime created_date
		{
			get { return DateHelper.ConvertDoubleToDate(Convert.ToDouble(this.created)); }
		}

        #region IEquatable<photo_tag> Members

        public bool Equals(photo_tag other)
        {
            if (this.pid == other.pid && this.xcoord == other.xcoord && this.ycoord == other.ycoord && this.text == other.text)
                return true;
            else
                return false;

        }

        #endregion
    }
	public partial class photo
	{
#if !SILVERLIGHT
        private Image _picture;
		private Image _pictureBig;
		private Image _pictureSmall;

		public Image picture
		{
			get
			{
				if (this.src == null)
				{
				    return ImageHelper.MissingPicture;
				}
				else if (_picture == null)
				{
					WebClient webClient = new WebClient();
					Byte[] pictureBytes = webClient.DownloadData(this.src);
					_picture = ImageHelper.ConvertBytesToImage(pictureBytes);
					return _picture;
				}
				else
				{
					return _picture;
				}
			}
		}
		public Image picture_big
		{
			get
			{
				if (this.src_big == null)
				{
                    return ImageHelper.MissingPicture;
				}
				else if (_pictureBig == null)
				{
					WebClient webClient = new WebClient();
					Byte[] pictureBytes = webClient.DownloadData(this.src_big);
					_pictureBig = ImageHelper.ConvertBytesToImage(pictureBytes);
					return _pictureBig;
				}
				else
				{
					return _pictureBig;
				}
			}
		}
		public Image picture_small
		{
			get
			{
				if (this.src_small == null)
				{
                    return ImageHelper.MissingPicture;
				}
				else if (_pictureSmall == null)
				{
					WebClient webClient = new WebClient();
					Byte[] pictureBytes = webClient.DownloadData(this.src_small);
					_pictureSmall = ImageHelper.ConvertBytesToImage(pictureBytes);
					return _pictureSmall;
				}
				else
				{
					return _pictureSmall;
				}
			}
		}

#endif

		public DateTime created_date
		{
			get { return DateHelper.ConvertDoubleToDate(Convert.ToDouble(this.created)); }
		}
	}
	public partial class facebookevent
	{
#if !SILVERLIGHT
        
        private Image _picture;
		private Image _pictureBig;
		private Image _pictureSmall;

		public Image picture
		{
			get
			{
				if (this.pic == null)
				{
                    return ImageHelper.MissingPicture;
				}
				else if (_picture == null)
				{
					WebClient webClient = new WebClient();
					Byte[] pictureBytes = webClient.DownloadData(this.pic);
					_picture = ImageHelper.ConvertBytesToImage(pictureBytes);
					return _picture;
				}
				else
				{
					return _picture;
				}
			}
		}
		public Image picture_big
		{
			get
			{
				if (this.pic_big == null)
				{
                    return ImageHelper.MissingPicture;
				}
				else if (_pictureBig == null)
				{
					WebClient webClient = new WebClient();
					Byte[] pictureBytes = webClient.DownloadData(this.pic_big);
					_pictureBig = ImageHelper.ConvertBytesToImage(pictureBytes);
					return _pictureBig;
				}
				else
				{
					return _pictureBig;
				}
			}
		}
		public Image picture_small
		{
			get
			{
				if (this.pic_small == null)
				{
                    return ImageHelper.MissingPicture;
				}
				else if (_pictureSmall == null)
				{
					WebClient webClient = new WebClient();
					Byte[] pictureBytes = webClient.DownloadData(this.pic_small);
					_pictureSmall = ImageHelper.ConvertBytesToImage(pictureBytes);
					return _pictureSmall;
				}
				else
				{
					return _pictureSmall;
				}
			}
		}

#endif

		public DateTime start_date
		{
			get { return DateHelper.ConvertDoubleToDate(Convert.ToDouble(this.start_time)); }
			set { this.start_time = (int)DateHelper.ConvertDateToDouble(value); }
		}
		public DateTime end_date
		{
			get { return DateHelper.ConvertDoubleToDate(Convert.ToDouble(this.end_time)); }
			set { this.end_time = (int)DateHelper.ConvertDateToDouble(value); }
		}
		public DateTime update_date
		{
			get { return DateHelper.ConvertDoubleToDate(Convert.ToDouble(this.update_time)); }
		}
	}
    public partial class stream_post
    {
        private string filter_keyField;
        public string filter_key
        {
            get
            {
                return this.filter_keyField;
            }
            set
            {
                this.filter_keyField = value;
            }
        }

    }
	public partial class group
	{
#if !SILVERLIGHT

		private Image _picture;
		private Image _pictureBig;
		private Image _pictureSmall;
		public Image picture
		{
			get
			{
				if (this.pic == null)
				{
                    return ImageHelper.MissingPicture;
				}
				else if (_picture == null)
				{
					WebClient webClient = new WebClient();
					Byte[] pictureBytes = webClient.DownloadData(this.pic);
					_picture = ImageHelper.ConvertBytesToImage(pictureBytes);
					return _picture;
				}
				else
				{
					return _picture;
				}
			}
		}
		public Image picture_big
		{
			get
			{
				if (this.pic_big == null)
				{
                    return ImageHelper.MissingPicture;
				}
				else if (_pictureBig == null)
				{
					WebClient webClient = new WebClient();
					Byte[] pictureBytes = webClient.DownloadData(this.pic_big);
					_pictureBig = ImageHelper.ConvertBytesToImage(pictureBytes);
					return _pictureBig;
				}
				else
				{
					return _pictureBig;
				}
			}
		}
		public Image picture_small
		{
			get
			{
				if (this.pic_small == null)
				{
                    return ImageHelper.MissingPicture;
				}
				else if (_pictureSmall == null)
				{
					WebClient webClient = new WebClient();
					Byte[] pictureBytes = webClient.DownloadData(this.pic_small);
					_pictureSmall = ImageHelper.ConvertBytesToImage(pictureBytes);
					return _pictureSmall;
				}
				else
				{
					return _pictureSmall;
				}
			}
		}

#endif

		public DateTime update_date
		{
			get { return DateHelper.ConvertDoubleToDate(Convert.ToDouble(this.update_time)); }
		}
	}
	public partial class user
	{
#if !SILVERLIGHT

		private Image _picture;
		private Image _pictureBig;
		private Image _pictureSmall;
		private Image _pictureSquare;
        private string _birthdayDate;
        [XmlElement(IsNullable = true, ElementName="birthday_date")]
        public string birthday_date
        {
            get
            {
                return this._birthdayDate;
            }
            set
            {
                this._birthdayDate = value;
            }
        }


        public Image picture
		{
			get
			{
				if (this.pic == null)
				{
                    return ImageHelper.MissingPicture;

				}
				else if (_picture == null)
				{
					WebClient webClient = new WebClient();
					Byte[] pictureBytes = webClient.DownloadData(this.pic);
					_picture = ImageHelper.ConvertBytesToImage(pictureBytes);
					return _picture;
				}
				else
				{
					return _picture;
				}
			}
		}
		public Image picture_big
		{
			get
			{
				if (this.pic_big == null)
				{
                    return ImageHelper.MissingPicture;
				}
				else if (_pictureBig == null)
				{
					WebClient webClient = new WebClient();
					Byte[] pictureBytes = webClient.DownloadData(this.pic_big);
					_pictureBig = ImageHelper.ConvertBytesToImage(pictureBytes);
					return _pictureBig;
				}
				else
				{
					return _pictureBig;
				}
			}
		}
		public Image picture_small
		{
			get
			{
				if (this.pic_small == null)
				{
                    return ImageHelper.MissingPicture;
				}
				else if (_pictureSmall == null)
				{
					WebClient webClient = new WebClient();
					Byte[] pictureBytes = webClient.DownloadData(this.pic_small);
					_pictureSmall = ImageHelper.ConvertBytesToImage(pictureBytes);
					return _pictureSmall;
				}
				else
				{
					return _pictureSmall;
				}
			}
		}
		public Image picture_square
		{
			get
			{
				if (this.pic_square == null)
				{
                    return ImageHelper.MissingPicture;
				}
				else if (_pictureSquare == null)
				{
					WebClient webClient = new WebClient();
					Byte[] pictureBytes = webClient.DownloadData(this.pic_square);
					_pictureSquare = ImageHelper.ConvertBytesToImage(pictureBytes);
					return _pictureSquare;
				}
				else
				{
					return _pictureSquare;
				}
			}
		}

#endif
        private string profile_urlField;
        private string proxied_emailField;
        private string username_Field; 
        private string email_Field;  

        [XmlElement(IsNullable = true)]
        public string proxied_email
        {
            get
            {
                return this.proxied_emailField;
            }
            set
            {
                this.proxied_emailField = value;
            }
        }
        [XmlElement(IsNullable = true)]
        public string profile_url
        {
            get
            {
                return this.profile_urlField;
            }
            set
            {
                this.profile_urlField = value;
            }
        }


        [XmlElement(IsNullable = true)]
        public string username 
        {
            get
            {
                return this.username_Field;
            }
            set
            {
                this.username_Field = value;
            }
        }

        [XmlElement(IsNullable = true)]
        public string email 
        {
            get
            {
                return this.email_Field;
            }
            set
            {
                this.email_Field = value;
            }
        }


	}
	public class feed_image
	{
		/// <summary>
		/// The URL of an image to be displayed in the News Feed story.
		/// </summary>
		public string image_url { get; set; }

		/// <summary>
		/// The URL destination after a click on the image referenced by ImageURL.
		/// </summary>
		public string image_link_url { get; set; }

		/// <summary>
		/// constructor
		/// </summary>
		public feed_image(string image_url, string image_link_url)
		{
			this.image_url = image_url;
			this.image_link_url = image_link_url;
		}
	}
    public partial class dashboard_bundle
    {


        public dashboard_bundle()
        {
            if ((this.action_link == null))
            {
                this.action_link = new action_link();
            }
        }
        public string message
        {
            get;set;
        }
        public action_link action_link
        {
            get;set;
        }
    }



    public partial class dashboard_activity
    {


        public dashboard_activity()
        {
            if ((this.bundle == null))
            {
                this.bundle = new dashboard_bundle();
            }
        }

        public dashboard_bundle bundle
        {
            get;
            set;
        }

        public long time
        {
            get;
            set;
        }
        public long fbid
        {
            get;
            set;
        }
        public string activity_id
        {
            get;
            set;
        }

    }
#if !SILVERLIGHT
    public class Dashboard_GetActivityResponseConverter : JavaScriptConverter
    {

        public override IEnumerable<Type> SupportedTypes
        {
            //Define the ListItemCollection as a supported type.
            get { return new ReadOnlyCollection<Type>(new List<Type>(new Type[] { typeof(List<dashboard_activity>) })); }
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            return new Dictionary<string, object>();
        }

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            var result = new List<dashboard_activity>();
            foreach (var item in dictionary)
            {
                var activity = new dashboard_activity();
                activity.activity_id = item.Key;
                Dictionary<string, object> inner = (Dictionary<string, object>)item.Value;
                activity.fbid = long.Parse(inner["fbid"].ToString());
                activity.time = long.Parse(inner["time"].ToString());
                activity.bundle.message = inner["message"].ToString();
                Dictionary<string, object> al = (Dictionary<string, object>)inner["action_link"];
                activity.bundle.action_link.href = al["href"].ToString();
                activity.bundle.action_link.text = al["text"].ToString();
                result.Add(activity);
            }
            return result;
        }

    }
    public class FacebookExceptionConverter : JavaScriptConverter
    {

        public override IEnumerable<Type> SupportedTypes
        {
            //Define the ListItemCollection as a supported type.
            get { return new ReadOnlyCollection<Type>(new List<Type>(new Type[] { typeof(FacebookApiException) })); }
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            return new Dictionary<string, object>();
        }

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            var result = new FacebookApiException()
            {
                error_code = int.Parse(dictionary["error_code"].ToString()),
                error_msg = dictionary["error_msg"].ToString()
            };
            ArrayList parms = (ArrayList)dictionary["request_args"];
            foreach (var parm in parms)
            {
                var dict = parm as Dictionary<string, object>;
                result.request_args.list = true;
                result.request_args.listSpecified = true;

                result.request_args.arg.Add(new arg() { key = dict["key"].ToString(), value = dict["value"].ToString() });
            }

            return result;
        }

    }
    public class Dashboard_GetNewsResponseConverter : JavaScriptConverter
    {

        public override IEnumerable<Type> SupportedTypes
        {
            //Define the ListItemCollection as a supported type.
            get { return new ReadOnlyCollection<Type>(new List<Type>(new Type[] { typeof(List<dashboard_news>) })); }
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            return new Dictionary<string, object>();
        }

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            var result = new List<dashboard_news>();
            foreach (var item in dictionary)
            {
                var news = new dashboard_news();
                news.news_id = item.Key;
                Dictionary<string, object> inner = (Dictionary<string, object>)item.Value;
                news.fbid = long.Parse(inner["fbid"].ToString());
                news.time = long.Parse(inner["time"].ToString());
                news.image = inner["image"].ToString();
                if (inner["news"].GetType() == typeof(ArrayList))
                {
                    var bundles = (ArrayList)inner["news"];
                    foreach (Dictionary<string, object> b in bundles)
                    {
                        var bundle = new dashboard_bundle();
                        bundle.message = b["message"].ToString();
                        Dictionary<string, object> al = (Dictionary<string, object>)b["action_link"];
                        if (al != null)
                        {
                            bundle.action_link.href = al["href"].ToString();
                            bundle.action_link.text = al["text"].ToString();
                        }
                        else
                        {
                            bundle.action_link = null;
                        }

                        news.news.Add(bundle);
                    }
                }
                else
                {
                    var bundles = (Object[])inner["news"];
                    foreach (Dictionary<string, object> b in bundles)
                    {
                        var bundle = new dashboard_bundle();
                        bundle.message = b["message"].ToString();
                        Dictionary<string, object> al = (Dictionary<string, object>)b["action_link"];
                        if (al != null)
                        {
                            bundle.action_link.href = al["href"].ToString();
                            bundle.action_link.text = al["text"].ToString();
                        }
                        else
                        {
                            bundle.action_link = null;
                        }

                        news.news.Add(bundle);
                    }

                }
                result.Add(news);
            }
            return result;
        }

    }
#else
    public class JsonConverterManager
    {
        public JsonConverterManager()
        {
            Converters = new List<JsonConverter>();
        }
        public List<JsonConverter> Converters{get;set;}
        public object Deserialize(string json, Type type)
        {
            JsonConverter converter = Converters.FirstOrDefault(c => c.SupportedType == type);
            if(converter != null)
            {
                return converter.Deserialize(json,type);
            }
            else
            {
                return null;
            }
        }
    }
    public class JsonConverter
    {
        public Type SupportedType{get;set;}
        public virtual object Deserialize(string json, Type type)
        {
            return null;
        }
    }
    public class JsonDictionaryStringObjectConverter : JsonConverter
    {
        public JsonDictionaryStringObjectConverter()
        {
            SupportedType = typeof(Dictionary<string,object>);
        }
        public override object Deserialize(string json, Type type)
        {
            using (var mo = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                System.Json.JsonObject result = (System.Json.JsonObject)System.Json.JsonArray.Load(mo);
                Dictionary<string, object> ret = new Dictionary<string, object>();
                for (int i = 0; i < result.Count; i++)
                {
                    ret.Add(result.Keys.ElementAt(i).ToString(), result.Values.ElementAt(i).ToString());
                }

                return ret;
            }
        }
    }
    public class JsonDictionaryStringBoolConverter : JsonConverter
    {
        public JsonDictionaryStringBoolConverter()
        {
            SupportedType = typeof(Dictionary<string, bool>);
        }
        public override object Deserialize(string json, Type type)
        {
            using (var mo = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                System.Json.JsonObject result = (System.Json.JsonObject)System.Json.JsonArray.Load(mo);
                Dictionary<string, bool> ret = new Dictionary<string, bool>();
                for (int i = 0; i < result.Count; i++)
                {
                    ret.Add(result.Keys.ElementAt(i).ToString(), bool.Parse(result.Values.ElementAt(i).ToString()));
                }

                return ret;
            }
        }
    }
    public class JsonDictionaryStringIntConverter : JsonConverter
    {
        public JsonDictionaryStringIntConverter()
        {
            SupportedType = typeof(Dictionary<string, int>);
        }
        public override object Deserialize(string json, Type type)
        {
            using (var mo = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                System.Json.JsonObject result = (System.Json.JsonObject)System.Json.JsonArray.Load(mo);
                Dictionary<string, int> ret = new Dictionary<string, int>();
                for (int i = 0; i < result.Count; i++)
                {
                    ret.Add(result.Keys.ElementAt(i).ToString(), int.Parse(result.Values.ElementAt(i).ToString()));
                }

                return ret;
            }
        }
    }
    public class JsonDictionaryStringDictionaryConverter : JsonConverter
    {
        public JsonDictionaryStringDictionaryConverter()
        {
            SupportedType = typeof(Dictionary<string, Dictionary<string, object>>);
        }
        public override object Deserialize(string json, Type type)
        {
            using (var mo = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                System.Json.JsonObject result = (System.Json.JsonObject)System.Json.JsonArray.Load(mo);
                Dictionary<string, Dictionary<string, object>> ret = new Dictionary<string, Dictionary<string, object>>();
                for (int i = 0; i < result.Count; i++)
                {
                    Dictionary<string, object> inner = new Dictionary<string, object>();
                    System.Json.JsonObject value = (System.Json.JsonObject)result.Values.ElementAt(i);
                    for (int j = 0; j < value.Count; j++)
                    {
                        inner.Add(value.Keys.ElementAt(i).ToString(), value.Values.ElementAt(i).ToString());
                    }
                    ret.Add(result.Keys.ElementAt(i).ToString(), inner);

                }

                return ret;
            }
        }
    }
    public class JsonDictionaryStringNewsConverter : JsonConverter
    {
        public JsonDictionaryStringNewsConverter()
        {
            SupportedType = typeof(Dictionary<string, List<dashboard_news>>);
        }
        public override object Deserialize(string json, Type type)
        {
            using (var mo = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                System.Json.JsonObject result = (System.Json.JsonObject)System.Json.JsonArray.Load(mo);
                Dictionary<string, List<dashboard_news>> ret = new Dictionary<string, List<dashboard_news>>();
                for (int i = 0; i < result.Count; i++)
                {
                    List<dashboard_news> inner = new List<dashboard_news>();
                    System.Json.JsonObject value = (System.Json.JsonObject)result.Values.ElementAt(i);
                    for (int j = 0; j < value.Count; j++)
                    {
                        System.Json.JsonArray news = (System.Json.JsonArray)value.Values.ElementAt(j)["news"];
                        List<dashboard_bundle> bundles = new List<dashboard_bundle>();
                        foreach(var item in news)
                        {
                            var bundle = new dashboard_bundle()
                            {
                                message = item["message"].ToString().Replace("\"", string.Empty)
                            };
                            var action_link = new action_link();
                            var al = item["action_link"];
                            if (al != null && al.Count > 0)
                            {
                                action_link.href = al["href"].ToString().Replace("\"", string.Empty);
                                action_link.text = al["text"].ToString().Replace("\"", string.Empty);
                                bundle.action_link = action_link;
                            }
                            bundles.Add(bundle);
                        }
                        var news_item = new dashboard_news()
                        {
                            news_id = value.Keys.ElementAt(j).ToString().Replace("\"", string.Empty),
                            fbid = long.Parse(value.Values.ElementAt(j)["fbid"].ToString()),
                            image = value.Values.ElementAt(j)["image"].ToString().Replace("\"", string.Empty),
                            time = long.Parse(value.Values.ElementAt(j)["time"].ToString()),
                            news = bundles
                        };

                        inner.Add(news_item);
                    }
                    ret.Add(result.Keys.ElementAt(i).ToString(), inner);

                }

                return ret;
            }
        }
    }
    public class JsonFacebookApiExceptionConverter : JsonConverter
    {
        public JsonFacebookApiExceptionConverter()
        {
            SupportedType = typeof(FacebookApiException);
        }
        public override object Deserialize(string json, Type type)
        {
            using (var mo = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                System.Json.JsonObject result = (System.Json.JsonObject)System.Json.JsonArray.Load(mo);
                FacebookApiException ret = new FacebookApiException()
                    {
                        error_code = int.Parse(result["error_code"].ToString().Replace("\"", string.Empty)),
                        error_msg = result["error_msg"].ToString().Replace("\"", string.Empty)
                    };
                System.Json.JsonArray args = (System.Json.JsonArray)result["request_args"];
                foreach (var arg in args)
                {
                    ret.request_args.list = true;
                    ret.request_args.listSpecified = true;

                    ret.request_args.arg.Add(new arg(){key = arg["key"].ToString().Replace("\"", string.Empty), value = arg["value"].ToString().Replace("\"", string.Empty)});
                }

                return ret;
            }
        }
    }
    public class JsonDashboardActivityListConverter : JsonConverter
    {
        public JsonDashboardActivityListConverter()
        {
            SupportedType = typeof(List<dashboard_activity>);
        }
        public override object Deserialize(string json, Type type)
        {
            using (var mo = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                System.Json.JsonObject result = (System.Json.JsonObject)System.Json.JsonArray.Load(mo);
                List<dashboard_activity> ret = new List<dashboard_activity>();
                for (int i = 0; i < result.Count; i++)
                {
                        var bundle = new dashboard_bundle()
                        {
                            message = result.Values.ElementAt(i)["message"].ToString().Replace("\"", string.Empty)
                        };
                        var action_link = new action_link();
                        var al = result.Values.ElementAt(i)["action_link"];
                        if (al != null && al.Count > 0)
                        {
                            action_link.href = al["href"].ToString().Replace("\"", string.Empty);
                            action_link.text = al["text"].ToString().Replace("\"", string.Empty);
                            bundle.action_link = action_link;
                        }
                        var activity = new dashboard_activity()
                        {
                            activity_id = result.Keys.ElementAt(i).ToString().Replace("\"", string.Empty),
                            fbid = long.Parse(result.Values.ElementAt(i)["fbid"].ToString()),
                            time = long.Parse(result.Values.ElementAt(i)["time"].ToString()),
                            bundle = bundle
                        };

                        ret.Add(activity);
                    }
                return ret;
            }
        }
    }
    public class JsonDashboardNewsListConverter : JsonConverter
    {
        public JsonDashboardNewsListConverter()
        {
            SupportedType = typeof(List<dashboard_news>);
        }
        public override object Deserialize(string json, Type type)
        {
            using (var mo = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                System.Json.JsonObject result = (System.Json.JsonObject)System.Json.JsonArray.Load(mo);
                List<dashboard_news> ret = new List<dashboard_news>();
                for (int i = 0; i < result.Count; i++)
                {
                    System.Json.JsonArray news = (System.Json.JsonArray)result.Values.ElementAt(i)["news"];
                    List<dashboard_bundle> bundles = new List<dashboard_bundle>();
                    foreach (var item in news)
                    {
                        var bundle = new dashboard_bundle()
                        {
                            message = item["message"].ToString().Replace("\"", string.Empty)
                        };
                        var action_link = new action_link();
                        var al = item["action_link"];
                        if (al != null && al.Count > 0)
                        {
                            action_link.href = al["href"].ToString().Replace("\"", string.Empty);
                            action_link.text = al["text"].ToString().Replace("\"", string.Empty);
                            bundle.action_link = action_link;
                        }
                        bundles.Add(bundle);
                    }
                    var news_item = new dashboard_news()
                    {
                        news_id = result.Keys.ElementAt(i).ToString().Replace("\"", string.Empty),
                        image = result.Keys.ElementAt(i).ToString().Replace("\"", string.Empty),
                        fbid = long.Parse(result.Values.ElementAt(i)["fbid"].ToString()),
                        time = long.Parse(result.Values.ElementAt(i)["time"].ToString()),
                        news = bundles
                    };

                    ret.Add(news_item);
                }

                return ret;
            }
        }
    }

#endif
    public partial class dashboard_news
    {

        public dashboard_news()
        {
            if ((this.news == null))
            {
                this.news = new List<dashboard_bundle>();
            }
        }

        public string image
        {
            get;set;
        }

        public List<dashboard_bundle> news
        {
            get;set;
        }

        public long fbid
        {
            get;set;
        }

        public long time
        {
            get;set;
        }
        public string news_id
        {
            get;
            set;
        }
    }




	#region Classes not generated by Xsd2Code

	public class BooleanResponse
	{
		[XmlText]
		public bool TypedValue { get; set; }
	}

	public class IntResponse
	{
		[XmlText]
		public int TypedValue { get; set; }
	}

	public class LongResponse
	{
		[XmlText]
		public long TypedValue { get; set; }
	}

	public class StringResponse
	{
		[XmlText]
		public string TypedValue { get; set; }
	}

	[XmlRoot("admin_banUsers_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class admin_banUsers_response : BooleanResponse { }

	[XmlRoot("admin_getAllocation_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class admin_getAllocation_response : IntResponse { }

	[XmlRoot("admin_getAppProperties_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class admin_getAppProperties_response : StringResponse { }
	
	[XmlRoot("admin_getRestrictionInfo_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class admin_getRestrictionInfo_response : StringResponse { }

	[XmlRoot("admin_setAppProperties_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class admin_setAppProperties_response : BooleanResponse { }
	
	[XmlRoot("admin_setRestrictionInfo_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class admin_setRestrictionInfo_response : BooleanResponse { }

	[XmlRoot("admin_unbanUsers_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class admin_unbanUsers_response : BooleanResponse { }

	[XmlRoot("application_getPublicInfo_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class application_getPublicInfo_response : app_info { }

	[XmlRoot("auth_createToken_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class auth_createToken_response : StringResponse { }

	[XmlRoot("auth_expireSession_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class auth_expireSession_response : BooleanResponse { }

	[XmlRoot("auth_getSession_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class auth_getSession_response :	session_info { }

	[XmlRoot("auth_promoteSession_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class auth_promoteSession_response : StringResponse { }

	[XmlRoot("auth_revokeAuthorization_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class auth_revokeAuthorization_response : BooleanResponse { }

	[XmlRoot("auth_revokeExtendedPermission_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class auth_revokeExtendedPermission_response : BooleanResponse { }

	[XmlRoot("comments_add_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class comments_add_response : IntResponse { }

	[XmlRoot("comments_remove_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class comments_remove_response : BooleanResponse { }

	[XmlRoot("connect_getUnconnectedFriendsCount_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class connect_getUnconnectedFriendsCount_response : IntResponse { }

	[XmlRoot("data_createObject_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class data_createObject_response : LongResponse { }

	[XmlRoot("data_createObjectType_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class data_createObjectType_response : BooleanResponse { }

	[XmlRoot("data_defineAssociation_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class data_defineAssociation_response : BooleanResponse { }

	[XmlRoot("data_defineObjectProperty_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class data_defineObjectProperty_response : BooleanResponse { }

	[XmlRoot("data_deleteObject_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class data_deleteObject_response : BooleanResponse { }

	[XmlRoot("data_deleteObjects_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class data_deleteObjects_response : BooleanResponse { }

	[XmlRoot("data_dropObjectType_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class data_dropObjectType_response : BooleanResponse { }

	[XmlRoot("data_getAssociatedObjectCount_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class data_getAssociatedObjectCount_response : IntResponse { }

	[XmlRoot("data_getAssociationDefinition_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class data_getAssociationDefinition_response : object_assoc_info { }

	[XmlRoot("data_getHashValue_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class data_getHashValue_response : StringResponse { }

	[XmlRoot("data_getObject_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class data_getObject_response : Dictionary<string,string> { }

	[XmlRoot("data_getObjectProperty_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class data_getObjectProperty_response : StringResponse { }

	[XmlRoot("data_getUserPreference_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class data_getUserPreference_response : StringResponse { }

	[XmlRoot("data_incHashValue_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class data_incHashValue_response : IntResponse { }

	[XmlRoot("data_removeAssociatedObjects_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class data_removeAssociatedObjects_response : BooleanResponse { }
	
	[XmlRoot("data_removeAssociation_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class data_removeAssociation_response : BooleanResponse { }

	[XmlRoot("data_removeAssociations_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class data_removeAssociations_response : BooleanResponse { }

	[XmlRoot("data_removeHashKey_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class data_removeHashKey_response : BooleanResponse { }

	[XmlRoot("data_removeHashKeys_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class data_removeHashKeys_response : BooleanResponse { }
	
	[XmlRoot("data_renameAssociation_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class data_renameAssociation_response : BooleanResponse { }

	[XmlRoot("data_renameObjectProperty_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class data_renameObjectProperty_response : BooleanResponse { }

	[XmlRoot("data_renameObjectType_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class data_renameObjectType_response : BooleanResponse { }

	[XmlRoot("data_setAssociation_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class data_setAssociation_response : BooleanResponse { }

	[XmlRoot("data_setAssociations_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class data_setAssociations_response : BooleanResponse { }

	[XmlRoot("data_setCookie_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class data_setCookie_response : BooleanResponse { }

	[XmlRoot("data_setHashValue_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class data_setHashValue_response : LongResponse { }

	[XmlRoot("data_setObjectProperty_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class data_setObjectProperty_response : BooleanResponse { }

	[XmlRoot("data_setUserPreference_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class data_setUserPreference_response : BooleanResponse { }

	[XmlRoot("data_setUserPreferences_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class data_setUserPreferences_response : BooleanResponse { }

	[XmlRoot("data_undefineAssociation_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class data_undefineAssociation_response : BooleanResponse { }

	[XmlRoot("data_undefineObjectProperty_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class data_undefineObjectProperty_response : BooleanResponse { }

	[XmlRoot("data_updateObject_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class data_updateObject_response : BooleanResponse { }

	[XmlRoot("events_cancel_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class events_cancel_response : BooleanResponse { }

	[XmlRoot("events_create_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class events_create_response : LongResponse { }

	[XmlRoot("events_edit_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class events_edit_response : BooleanResponse { }

	[XmlRoot("events_getMembers_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class events_getMembers_response : event_members { }

	[XmlRoot("events_rsvp_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class events_rsvp_response : BooleanResponse { }

	[XmlRoot("fbml_deleteCustomTags_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class fbml_deleteCustomTags_response : BooleanResponse { }

	[XmlRoot("fbml_refreshImgSrc_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class fbml_refreshImgSrc_response : BooleanResponse { }

	[XmlRoot("fbml_refreshRefUrl_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class fbml_refreshRefUrl_response : BooleanResponse { }

	[XmlRoot("fbml_registerCustomTags_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class fbml_registerCustomTags_response : IntResponse { }

	[XmlRoot("fbml_setRefHandle_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class fbml_setRefHandle_response : BooleanResponse { }

	[XmlRoot("fbml_uploadNativeStrings_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class fbml_uploadNativeStrings_response : BooleanResponse { }

	[XmlRoot("feed_deactivateTemplateBundleByID_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class feed_deactivateTemplateBundleByID_response : BooleanResponse { }

	[XmlRoot("feed_getRegisteredTemplateBundleByID_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class feed_getRegisteredTemplateBundleByID_response : template_bundle { }

	[XmlRoot("feed_publishTemplatizedAction_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class feed_publishTemplatizedAction_response : BooleanResponse { }

	[XmlRoot("feed_registerTemplateBundle_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class feed_registerTemplateBundle_response : LongResponse { }

	[XmlRoot("groups_getMembers_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class groups_getMembers_response : group_members { }

    [XmlRoot("intl_uploadNativeStrings_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
    public class intl_uploadNativeStrings_response : LongResponse { }
    
    [XmlRoot("links_post_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class links_post_response : LongResponse { }

	[XmlRoot("liveMessage_send_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class liveMessage_send_response : BooleanResponse { }

	[XmlRoot("marketplace_createListing_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class marketplace_createListing_response : LongResponse { }

	[XmlRoot("marketplace_removeListing_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class marketplace_removeListing_response : BooleanResponse { }

	[XmlRoot("notes_create_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class notes_create_response : LongResponse { }

	[XmlRoot("notes_delete_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class notes_delete_response : BooleanResponse { }

	[XmlRoot("notes_edit_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class notes_edit_response : BooleanResponse { }

	[XmlRoot("notifications_get_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class notifications_get_response : notifications { }

	[XmlRoot("notifications_send_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class notifications_send_response : StringResponse { }

	[XmlRoot("notifications_sendEmail_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class notifications_sendEmail_response : StringResponse { }

	[XmlRoot("pages_isAdmin_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class pages_isAdmin_response : BooleanResponse { }

	[XmlRoot("pages_isAppAdded_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class pages_isAppAdded_response : BooleanResponse { }

	[XmlRoot("pages_isFan_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class pages_isFan_response : BooleanResponse { }

	[XmlRoot("permissions_grantApiAccess_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class permissions_grantApiAccess_response : BooleanResponse { }

	[XmlRoot("permissions_revokeApiAccess_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class permissions_revokeApiAccess_response : BooleanResponse { }

	[XmlRoot("photos_addTag_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class photos_addTag_response : BooleanResponse { }

	[XmlRoot("photos_createAlbum_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
    public class notifications_markRead_response : BooleanResponse { }

    [XmlRoot("notifications_GetList_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
    public class notifications_GetList_response : notification_data { }

    [XmlRoot("photos_createAlbum_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
    public class photos_createAlbum_response : album { }

	[XmlRoot("photos_upload_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class photos_upload_response : photo { }

	[XmlRoot("profile_getFBML_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class profile_getFBML_response : StringResponse { }

	[XmlRoot("profile_getInfo_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class profile_getInfo_response : user_info { }

	[XmlRoot("profile_setFBML_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class profile_setFBML_response : BooleanResponse { }

	[XmlRoot("profile_setInfo_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class profile_setInfo_response : BooleanResponse { }

	[XmlRoot("profile_setInfoOptions_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class profile_setInfoOptions_response : BooleanResponse { }

	[XmlRoot("status_set_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class status_set_response : BooleanResponse { }

	[XmlRoot("stream_addComment_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class stream_addComment_response : StringResponse { }

	[XmlRoot("stream_addLike_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class stream_addLike_response : BooleanResponse { }

	[XmlRoot("stream_get_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class stream_get_response : stream_data { }

	[XmlRoot("stream_publish_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class stream_publish_response : StringResponse { }

	[XmlRoot("stream_remove_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class stream_remove_response : BooleanResponse { }

	[XmlRoot("stream_removeComment_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class stream_removeComment_response : BooleanResponse { }

	[XmlRoot("stream_removeLike_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class stream_removeLike_response : BooleanResponse { }

	[XmlRoot("users_getLoggedInUser_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class users_getLoggedInUser_response : LongResponse { }

	[XmlRoot("users_hasAppPermission_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class users_hasAppPermission_response : BooleanResponse { }

	[XmlRoot("users_isAppUser_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class users_isAppUser_response : BooleanResponse { }

	[XmlRoot("users_isVerified_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class users_isVerified_response : BooleanResponse { }

	[XmlRoot("users_setStatus_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class users_setStatus_response : BooleanResponse { }

	[XmlRoot("video_getUploadLimits_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class video_getUploadLimits_response : video_limits { }

	[XmlRoot("video_upload_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
	public class video_upload_response : video { }

    [XmlRoot("dashboard_addGlobalNews_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
    public class dashboard_addGlobalNews_response : LongResponse { }

    [XmlRoot("dashboard_addNews_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
    public class dashboard_addNews_response : LongResponse { }

    [XmlRoot("dashboard_clearGlobalNews_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
    public class dashboard_clearGlobalNews_response : Dictionary<string,bool> { }

    [XmlRoot("dashboard_clearNews_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
    public class dashboard_clearNews_response : Dictionary<string, bool> { }
    [XmlRoot("dashboard_decrementCount_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
    public class dashboard_decrementCount_response : BooleanResponse { }
    [XmlRoot("dashboard_getCount_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
    public class dashboard_getCount_response : IntResponse { }
    [XmlRoot("dashboard_incrementCount_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
    public class dashboard_incrementCount_response : BooleanResponse { }
    [XmlRoot("dashboard_multiAddNews_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
    public class dashboard_multiAddNews_response : Dictionary<string,object> { }
    [XmlRoot("dashboard_multiClearNews_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
    public class dashboard_multiClearNews_response : Dictionary<string, Dictionary<string, object>> { }
    [XmlRoot("dashboard_multiDecrementCount_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
    public class dashboard_multiDecrementCount_response : Dictionary<string, bool> { }
    [XmlRoot("dashboard_multiGetCount_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
    public class dashboard_multiGetCount_response : Dictionary<string, int> { }
    [XmlRoot("dashboard_multiGetNews_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
    public class dashboard_multiGetNews_response : Dictionary<string, Dictionary<string, object>> { }
    [XmlRoot("dashboard_multiIncrementCount_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
    public class dashboard_multiIncrementCount_response : Dictionary<string, bool> { }
    [XmlRoot("dashboard_multiSetCount_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
    public class dashboard_multiSetCount_response : Dictionary<string, bool> { }
    [XmlRoot("dashboard_publishActivity_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
    public class dashboard_publishActivity_response : LongResponse { }
    [XmlRoot("dashboard_removeActivity_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
    public class dashboard_removeActivity_response : Dictionary<string, bool> { }
    [XmlRoot("dashboard_setCount_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
    public class dashboard_setCount_response : BooleanResponse { }
    [XmlRoot("sms_send_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
    public class sms_send_response : LongResponse { }

    [XmlRoot("sms_canSend_response", Namespace = "http://api.facebook.com/2.0/", IsNullable = false)]
    public class sms_canSend_response : LongResponse { }

	#endregion

#pragma warning restore 1591

}
