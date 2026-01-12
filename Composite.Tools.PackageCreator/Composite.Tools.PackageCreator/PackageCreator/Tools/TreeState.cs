using System;
using System.Collections.Generic;
using System.Linq;
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
        public static PageTree Page = new PageTree();
        public static MediaTree Media = new MediaTree();

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
    }

    public class PageTree : ITree<Guid>
    {
        private readonly object _syncRoot = new object();
        private Dictionary<Guid, TreeState> _list;


        public TreeState GetState(Guid pageId)
        {
            lock (_syncRoot)
            {
                if (_list != null) 
                    return GetState(pageId, _list);
                
                _list = new Dictionary<Guid, TreeState>();

                foreach (var page in PackageCreatorFacade.GetItems<PCPageTree>())
                {
                    if (!Guid.TryParse(page.Name, out var id)) 
                        continue;

                    _list[id] = TreeState.IncludedTree;
                }

                foreach (var page in PackageCreatorFacade.GetItems<PCExclude>().Where(d => d.Type == PCExclude.Exclude.Page))
                {
                    if (!Guid.TryParse(page.Name, out var id)) 
                        continue;

                    _list[id] = TreeState.ExcludedTree;
                }

                return GetState(pageId, _list);
            }
        }

        public void ClearCache()
        {
            lock (_syncRoot)
            {
                _list = null;
            }
        }

        private TreeState GetState(Guid pageId, Dictionary<Guid, TreeState> list)
        {
            if (list.TryGetValue(pageId, out var state))
                return state;

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
            return list[pageId];
        }
    }

    public class MediaTree : ITree<string>
    {
        private readonly object _syncRoot = new object();
        private Dictionary<string, TreeState> _list;
        
        public TreeState GetState(string path)
        {
            lock (_syncRoot)
            {
                if (_list != null)
                    return GetState(path, _list);

                _list = new Dictionary<string, TreeState>();

                foreach (var media in PackageCreatorFacade.GetItems<PCMediaFolder>())
                    _list[media.Name] = TreeState.IncludedTree;

                foreach (var media in PackageCreatorFacade.GetItems<PCExclude>().Where(d => d.Type == PCExclude.Exclude.MediaFolder))
                    _list[media.Name] = TreeState.ExcludedTree;

                return GetState(path, _list);
            }
        }

        public void ClearCache()
        {
            lock (_syncRoot)
            {
                _list = null;
            }
        }

        private static string GetParent(string path)
        {
            if (!path.StartsWith("/") || path == "/")
                return string.Empty;

            var lastIndex = path.LastIndexOf('/');

            return lastIndex == 0 ? "/" : path.Substring(0, lastIndex);
        }

        private static TreeState GetState(string path, Dictionary<string, TreeState> list)
        {
            if (list.TryGetValue(path, out var state))
                return state;

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
            return list[path];
        }
    }
}