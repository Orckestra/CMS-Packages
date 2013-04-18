using System;
using System.Collections.Generic;
using System.Linq;
using Composite.AppFeed.Server;
using Composite.AppFeed.Server.Providers;
using Composite.Data;

namespace Composite.AppFeed.Provider.Magazine
{
	public class MagazineAppFeedProvider : IAppFeedContentProvider
	{
		private Action _cacheRefresh;

		internal MagazineAppFeedProvider(Action cacheRefresh)
		{
			_cacheRefresh = cacheRefresh;

			//4.0 only stuff
			//DataEvents<MagazineGroup>.OnStoreChanged += OnStoreChanged;
			//DataEvents<MagazineArticle>.OnStoreChanged += OnStoreChanged;

			DataEvents<MagazineGroup>.OnAfterAdd += (a, b) => cacheRefresh();
			DataEvents<MagazineGroup>.OnAfterUpdate += (a, b) => cacheRefresh();
			DataEvents<MagazineGroup>.OnDeleted += (a, b) => cacheRefresh();

			DataEvents<MagazineArticle>.OnAfterAdd += (a, b) => cacheRefresh();
			DataEvents<MagazineArticle>.OnAfterUpdate += (a, b) => cacheRefresh();
			DataEvents<MagazineArticle>.OnDeleted += (a, b) => cacheRefresh();
		}

		//4.0 only stuff
		//void OnStoreChanged(object sender, StoreEventArgs storeEventArgs)
		//{
		//	_cacheRefresh();
		//}

		public IEnumerable<Type> SourceDataTypes
		{
			get
			{
				yield return typeof(MagazineGroup);
				yield return typeof(MagazineArticle);
			}
		}

		public TimeSpan MaxCacheLifeTime
		{
			get
			{
				return TimeSpan.MaxValue;
			}
		}

		public IEnumerable<Group> GetGroups()
		{
			using (var connection = new DataConnection())
			{
				return from groupItem in connection.Get<MagazineGroup>()
					   select new Group
					   {
						   Name = string.IsNullOrEmpty(groupItem.Name) ? groupItem.Id.ToString() : groupItem.Name,
						   Title = groupItem.Title,
						   SubTitle = groupItem.SubTitle,
						   ImageId = groupItem.Image,
						   Description = groupItem.Html,
						   Priority = groupItem.Priority
					   };
			}
		}

		public IEnumerable<Server.Content> GetContent()
		{
			using (var connection = new DataConnection())
			{
				var groupNameMapper = connection.Get<MagazineGroup>().ToDictionary(key => key.Id, value => string.IsNullOrEmpty(value.Name) ? value.Id.ToString() : value.Name);

				foreach (var item in connection.Get<MagazineArticle>().OrderByDescending(f => f.Date))
				{
					yield return
						new Content
						{
							Id = item.Id.ToString(),
							GroupName = groupNameMapper[item.Group],
							Title = item.Title,
							SubTitle = item.SubTitle,
							ImageId = item.Image,
							Description = item.Html
						};
				}
			}
		}
	}
}
