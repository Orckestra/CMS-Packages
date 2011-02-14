using System;
using System.Linq;
using System.Linq.Expressions;
using Composite.Core.WebClient.Services.WysiwygEditor;
using Composite.Data;
using CookComputing.XmlRpc;

namespace Composite.Community.Blog.MetaWeblog
{
	internal class MetaWeblogPost : MetaWeblogData
	{
		public Guid BlogEntryId { get; private set; }

		private Expression<Func<Entries, bool>> predicate;

		public MetaWeblogPost(string username, string password, string postid)
			: base(username, password)
		{
			BlogEntryId = ParseBlogEntryId(postid);
			predicate = d => d.Id == BlogEntryId && d.Author == Author.Id;
		}

		protected static Guid ParseBlogEntryId(string postid)
		{
			Guid blogEntryId;
			if (Guid.TryParse(postid, out blogEntryId))
			{
				return blogEntryId;
			}
			throw new XmlRpcFaultException(0, "Invalid post id");
		}

		public bool EditPost(string title, string description, string publicationStatus)
		{
			using (var entry = new UpdateDataWrapper<Entries>(predicate))
			{
				entry.Data.Title = title;
				entry.Data.Content = RelativeMediaUrl(MarkupTransformationServices.TidyHtml(description).Output.Root);
				entry.Data.PublicationStatus = publicationStatus;
				return true;
			}
		}

		public Entries GetPost()
		{
			return DataFacade.GetData<Entries>(predicate).Single();
		}

		public bool DeletePost()
		{
			DataFacade.Delete<Entries>(predicate);
			return true;
		}


	}
}
