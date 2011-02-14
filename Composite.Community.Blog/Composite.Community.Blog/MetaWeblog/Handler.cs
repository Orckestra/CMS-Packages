using System.Collections.Generic;
using System.Linq;
using Composite.Data;
using Composite.Data.ProcessControlled.ProcessControllers.GenericPublishProcessController;
using CookComputing.XmlRpc;

namespace Composite.Community.Blog.MetaWeblog
{
	public class Handler : XmlRpcService, IMetaWeblog
	{
		public string AddPost(string blogid, string username, string password, Post post, bool publish)
		{
			using (var metaWeblog = new MetaWeblogBlog(username, password, blogid))
			{
				var publicationStatus = publish ? GenericPublishProcessController.Published : GenericPublishProcessController.Draft;
				return metaWeblog.AddPost(post.title, post.description, publicationStatus).ToString();
			}
		}

		public bool EditPost(string postid, string username, string password, Post post, bool publish)
		{
			using (var metaWeblog = new MetaWeblogPost(username, password, postid))
			{
				var publicationStatus = publish ? GenericPublishProcessController.Published : GenericPublishProcessController.Draft;
				return metaWeblog.EditPost(post.title, post.description, publicationStatus);
			}
		}

		public Post GetPost(string postid, string username, string password)
		{
			using (var metaWeblog = new MetaWeblogPost(username, password, postid))
			{
				var blogEntry = metaWeblog.GetPost();
				return new Post()
				{
					postid = blogEntry.Id.ToString(),
					dateCreated = blogEntry.Date,
					description = metaWeblog.FullMediaUrl(blogEntry.Content),
					title = blogEntry.Title,
					link = blogEntry.GetPublicUrl(),
				};
			}
		}

		public CategoryInfo[] GetCategories(string blogid, string username, string password)
		{
			using (var metaWeblog = new MetaWeblogBlog(username, password, blogid))
			{
				return new CategoryInfo[0];
			}
		}

		public Post[] GetRecentPosts(string blogid, string username, string password, int numberOfPosts)
		{
			using (var metaWeblog = new MetaWeblogBlog(username, password, blogid))
			{
				return metaWeblog.GetRecentPosts().Take(numberOfPosts)
					.Select(d =>
						new Post()
						{
							postid = d.Id.ToString(),
							dateCreated = d.Date,
							title = d.Title
						}
					).ToArray();
			}
		}

		public UrlData NewMediaObject(string blogid, string username, string password, FileData mediaObject)
		{
			using (var metaWeblog = new MetaWeblogMedia(username, password))
			{
				var url = metaWeblog.NewMediaObject(mediaObject);
				return new UrlData()
				{
					url = url
				};
			}

		}

		public bool DeletePost(string key, string postid, string username, string password, bool publish)
		{
			using (var metaWeblog = new MetaWeblogPost(username, password, postid))
			{
				return metaWeblog.DeletePost();
			}
		}

		public BlogInfo[] GetUsersBlogs(string key, string username, string password)
		{
			using (var metaWeblog = new MetaWeblogData(username, password))
			{

				List<BlogInfo> userBlogs = new List<BlogInfo>();
				foreach (var page in metaWeblog.GetBlogPages())
				{
					var pageUrl = string.Empty;
					userBlogs.Add(
						new BlogInfo()
						{
							blogid = page.Id.ToString(),
							blogName = page.GetLabel(),
							url = page.GetPublicUrl()
						}
					);
				}
				return userBlogs.ToArray();
			}
		}

		public UserInfo GetUserInfo(string key, string username, string password)
		{
			using (var metaWeblog = new MetaWeblogData(username, password))
			{
				return new UserInfo()
				{
					email = metaWeblog.Author.Email,
					nickname = metaWeblog.Author.Name,
					userid = metaWeblog.Author.Id.ToString()
				};
			}
		}
	}
}
