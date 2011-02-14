using System;
using CookComputing.XmlRpc;

namespace Composite.Community.Blog.MetaWeblog
{
	public struct BlogInfo
	{
		public string blogid;
		public string url;
		public string blogName;
	}

	public struct Category
	{
		public string categoryId;
		public string categoryName;
	}

	[Serializable]
	public struct CategoryInfo
	{
		public string description;
		public string htmlUrl;
		public string rssUrl;
		public string title;
		public string categoryid;
	}

	[XmlRpcMissingMapping(MappingAction.Ignore)]
	public struct Enclosure
	{
		public int length;
		public string type;
		public string url;
	}

	[XmlRpcMissingMapping(MappingAction.Ignore)]
	public struct Post
	{
		public string title;
		public string description;
		public string[] categories;
		public DateTime dateCreated;
		public object postid;
		public string link;

		public object mt_allow_comments;
		public object mt_allow_pings;
		public object mt_convert_breaks;
		public string mt_text_more;
		public string mt_excerpt;
	}

	[XmlRpcMissingMapping(MappingAction.Ignore)]
	public struct Source
	{
		public string name;
		public string url;
	}

	public struct UserInfo
	{
		public string userid;
		public string firstname;
		public string lastname;
		public string nickname;
		public string email;
		public string url;
	}

	[XmlRpcMissingMapping(MappingAction.Ignore)]
	public struct FileData
	{
		public string name;
		public string type;
		public byte[] bits;
	}

	[Serializable]
	public struct UrlData
	{
		public string url;
	}
}
