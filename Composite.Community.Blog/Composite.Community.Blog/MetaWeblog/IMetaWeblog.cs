using CookComputing.XmlRpc;


namespace Composite.Community.Blog.MetaWeblog
{
	public interface IMetaWeblog
	{
		[XmlRpcMethod("metaWeblog.newPost")]
		string AddPost(string blogid, string username, string password, Post post, bool publish);

		[XmlRpcMethod("metaWeblog.editPost")]
		bool EditPost(string postid, string username, string password, Post post, bool publish);

		[XmlRpcMethod("metaWeblog.getPost")]
		Post GetPost(string postid, string username, string password);

		[XmlRpcMethod("metaWeblog.getCategories")]
		CategoryInfo[] GetCategories(string blogid, string username, string password);

		[XmlRpcMethod("metaWeblog.getRecentPosts")]
		Post[] GetRecentPosts(string blogid, string username, string password, int numberOfPosts);

		[XmlRpcMethod("metaWeblog.newMediaObject")]
		UrlData NewMediaObject(string blogid, string username, string password,
			FileData mediaObject);

		#region Blogger API

		[XmlRpcMethod("blogger.deletePost")]
		[return: XmlRpcReturnValue(Description = "Returns true.")]
		bool DeletePost(string key, string postid, string username, string password, bool publish);

		[XmlRpcMethod("blogger.getUsersBlogs")]
		BlogInfo[] GetUsersBlogs(string key, string username, string password);

		[XmlRpcMethod("blogger.getUserInfo")]
		UserInfo GetUserInfo(string key, string username, string password);

		#endregion
	}
}
