namespace Sobees.Infrastructure.Cls
{
	public class BTag
	{
		#region Members

		private string _tag;
		private string _lang;
		private bool _createdByUser;
		private string _owner;

		#endregion

		#region Properties

		public string Tag
		{
			get
			{
				return _tag;
			}
			set
			{
				_tag = value;
			}
		}

		public string Owner
		{
			get
			{
				return _owner;
			}
			set
			{
				_owner = value;
			}
		}

		public string Lang
		{
			get
			{
				return _lang;
			}
			set
			{
				_lang = value;
			}
		}

		public bool CreatedByUser
		{
			get
			{
				return _createdByUser;
			}
			set
			{
				_createdByUser = value;
			}
		}

		#endregion

		#region Constructors

		public BTag() { }

		public BTag(string tag, bool createdByUser)
		{
			Tag = tag;
			Lang = "all";
			CreatedByUser = createdByUser;
		}

		#endregion

		public override bool Equals(object obj)
		{
			var keyword = obj as BTag;
			if (keyword == null) return false;

			if (keyword.Tag.Equals(Tag))
				return true;

			return false;
		}
	}
}
