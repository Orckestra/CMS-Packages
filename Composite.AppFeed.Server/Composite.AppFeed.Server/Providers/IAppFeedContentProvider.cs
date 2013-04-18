using System.Collections.Generic;

namespace Composite.AppFeed.Server.Providers
{
	public interface IAppFeedContentProvider
	{
		IEnumerable<Group> GetGroups();
		IEnumerable<Content> GetContent();
	}
}
