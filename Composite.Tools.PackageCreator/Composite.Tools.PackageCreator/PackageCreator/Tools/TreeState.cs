using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using Composite.Data;
using Composite.Data.Types;
using Composite.Tools.PackageCreator.Types;

namespace Composite.Tools.PackageCreator
{

	public enum TreeState
	{
		IncludedTree,
		Included,
		ExcludedTree,
		Excluded,
		NotIncluded
	}

	public interface ITree<in T>
	{
		TreeState GetState(T key);
		void ClearCache();
	}

	public class Tree
	{
		static Tree()
		{
			DataEventSystemFacade.SubscribeToDataDeleted<IPageStructure>(OnPageStructureDeleted, true);
			DataEventSystemFacade.SubscribeToDataAfterUpdate<IMediaFileFolder>(OnMediaFolderChanged, true);
		}

		private static void OnMediaFolderChanged(object sender, DataEventArgs dataEventArgs)
		{
			Media.ClearCache();
		}

		private static void OnPageStructureDeleted(object sender, DataEventArgs dataEventArgs)
		{
			Page.ClearCache();
		}

		public static PageTree Page = new PageTree();
		public static MediaTree Media = new MediaTree();
	}

	public class PageTree : ITree<Guid>
	{

		private object _syncRoot = new object();
		private Dictionary<Guid, TreeState> _list = null;



		public TreeState GetState(Guid pageId)
		{
			lock (_syncRoot)
			{
				if (_list == null)
				{
					_list = new Dictionary<Guid, TreeState>();

					foreach (var page in PackageCreatorFacade.GetItems<PCPageTree>())
					{
						Guid id;
						if (!Guid.TryParse(page.Name, out id)) continue;
						_list[id] = TreeState.IncludedTree;
					}

					foreach (var page in PackageCreatorFacade.GetItems<PCExclude>().Where(d => d.Type == PCExclude.Exclude.Page))
					{
						Guid id;
						if (!Guid.TryParse(page.Name, out id)) continue;
						_list[id] = TreeState.ExcludedTree;
					}
				}

				return GetState(pageId, _list);
			}
		}

		private TreeState GetState(Guid pageId, Dictionary<Guid, TreeState> list)
		{
			if (!list.ContainsKey(pageId))
			{
				var status = TreeState.NotIncluded;
				var parentId = PageManager.GetParentId(pageId);
				if (parentId != Guid.Empty)
				{
					var parentStatus = GetState(parentId, list);
					switch (parentStatus)
					{
						case TreeState.Excluded:
						case TreeState.ExcludedTree:
							status = TreeState.Excluded;
							break;
						case TreeState.Included:
						case TreeState.IncludedTree:
							status = TreeState.Included;
							break;
					}
				}
				list[pageId] = status;
			}
			return list[pageId];
		}

		public void ClearCache()
		{
			lock (_syncRoot)
			{
				_list = null;
			}
		}
	}

	public class MediaTree : ITree<string>
	{

		private object _syncRoot = new object();
		private Dictionary<string, TreeState> _list = null;



		public TreeState GetState(string path)
		{
			lock (_syncRoot)
			{
				if (_list == null)
				{
					_list = new Dictionary<string, TreeState>();

					foreach (var media in PackageCreatorFacade.GetItems<PCMediaFolder>())
					{
						_list[media.Name] = TreeState.IncludedTree;
					}

					foreach (var media in PackageCreatorFacade.GetItems<PCExclude>().Where(d => d.Type == PCExclude.Exclude.MediaFolder))
					{
						_list[media.Name] = TreeState.ExcludedTree;
					}
				}

				return GetState(path, _list);
			}
		}

		private string GetParent(string path)
		{
			if (!path.StartsWith("/")) return string.Empty;
			if (path == "/") return string.Empty;
			var lastIndex = path.LastIndexOf('/');
			if (lastIndex == 0) return "/";
			return path.Substring(0, lastIndex);
		}

		private TreeState GetState(string path, Dictionary<string, TreeState> list)
		{
			if (!list.ContainsKey(path))
			{
				var status = TreeState.NotIncluded;
				var parentId = GetParent(path);
				if (parentId != string.Empty)
				{
					var parentStatus = GetState(parentId, list);
					switch (parentStatus)
					{
						case TreeState.Excluded:
						case TreeState.ExcludedTree:
							status = TreeState.Excluded;
							break;
						case TreeState.Included:
						case TreeState.IncludedTree:
							status = TreeState.Included;
							break;
					}
				}
				list[path] = status;
			}
			return list[path];
		}

		public void ClearCache()
		{
			lock (_syncRoot)
			{
				_list = null;
			}
		}
	}
}
