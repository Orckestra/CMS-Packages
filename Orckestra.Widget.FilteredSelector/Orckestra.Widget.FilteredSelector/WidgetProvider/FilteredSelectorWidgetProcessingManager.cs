using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

using Composite.Core.Types;
using Composite.Core.WebClient.Renderings.Page;
using Composite.Data;

namespace Orckestra.Widget.FilteredSelector.WidgetProvider
{
    internal static class FilteredSelectorWidgetProcessingManager
    {
        public static IEnumerable<Guid> GetAssociatedPageIds(Guid pageId, SitemapScope sitemapScope) => PageStructureInfo.GetAssociatedPageIds(pageId, sitemapScope);
        private static IEnumerable<KeyValuePair> GetOptionsForDefault(Type t, Guid pageId, SitemapScope sitemapScope)
        {
            IEnumerable<Guid> availablePageIds = GetAssociatedPageIds(pageId, sitemapScope).Distinct();

            using (DataConnection con = new DataConnection())
            {
                MethodInfo generic = typeof(DataConnection).GetMethod("Get").MakeGenericMethod(t);
                IEnumerable<KeyValuePair> result = from a in (IEnumerable<IPageRelatedData>)generic.Invoke(con, null)
                                                   join b in availablePageIds
                                                   on a.PageId equals b
                                                   select new KeyValuePair(a.GetKeyProperties().First(k => k.Name == "Id").GetValue(a).ToString(), a.GetLabel());
                return result;
            }
        }

        internal static IEnumerable GetParameters(string parameters)
        {
            Match match = Regex.Match(parameters, 
                $"{Constants.PageIdParamName}:\"(.+?)\";{Constants.TypeNameParamName}:\"(.+?)\";{Constants.SitemapScopeIdParamName}:\"([0-9]+)\"");

            if (match.Success == false)
            {
                throw new ArgumentException(string.Format(Resources.default_text.FilteredSelectorWidgetProcManEx1, parameters));
            }

            Type type = TypeManager.GetType(match.Groups[2].Value);
            Guid pageId = new Guid(match.Groups[1].Value.ToString());
            SitemapScope sitemapScope = (SitemapScope)Enum.Parse(typeof(SitemapScope), match.Groups[3].Value.ToString());

            foreach (KeyValuePair pageItem in GetOptionsForDefault(type, pageId, sitemapScope))
            {
                yield return new { pageItem.Key, Label = pageItem.Value };
            }
        }
    }
}
