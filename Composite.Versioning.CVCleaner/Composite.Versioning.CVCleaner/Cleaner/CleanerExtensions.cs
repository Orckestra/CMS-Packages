using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Composite.Data;
using Composite.Versioning.ContentVersioning.Data;
using Composite.Data.Types;
using Composite.Core.IO;
using Composite.Core;

namespace Composite.Versioning.ContentVersioning.Cleaner
{
	/// <summary>
	/// Extensions for Xml.Linq v1.0
	/// </summary>
	internal static class CleanerExtensions
	{
		//public static string AttributeValue(this XElement element, XName attributeName)
		//{
		//    return element.Attributes(attributeName).Select(d => d.Value).FirstOrDefault();
		//}

		//public static string ElementValue(this XElement element, XName elementName)
		//{
		//    return element.Elements(elementName).Select(d => d.Value).FirstOrDefault();
		//}

		//public static IEnumerable<T> NotNull<T>(this IEnumerable<T> source)
		//{
		//    return source.Where(d => d != null);
		//}

		public static void DeleteActivity(this DataConnection conn, Guid activityId, DataSourceId dataSourceId)
		{
			try
			{
				var activity = conn.Get<IActivity>().Where(d => d.Id == activityId).FirstOrDefault();

				var activityChanges = conn.Get<IDataChanges>().Where(d => d.ActivityId == activityId).ToList();

				if (dataSourceId != null)
				{
					if (dataSourceId.InterfaceType == typeof(IMediaFile))
					{
						try
						{
							var mediaFileId = dataSourceId.DataId.GetProperty<Guid>("Id");
							var mediaFileActivityPath = CleanerFacade.GetMediaFileActivityPath(mediaFileId, activityId);
							C1File.Delete(mediaFileActivityPath);
						}
						catch (Exception e)
						{
							Log.LogWarning(CleanerFacade.Title, e);
						}
					}
				}

				conn.Delete<IDataChanges>(activityChanges);
				conn.Delete(activity);

				Log.LogVerbose(CleanerFacade.Title, "Delete activity '{0}'".Push(activityId));
			}
			catch
			{ }

		}

		public static string Push(this string format, params object[] args)
		{
			return string.Format(format, args);
		}
	}
}
