using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Xml;
using System.Xml.Linq;
using System.Linq;
using Composite.Core;
using Composite.Core.Application;
using Composite.Core.IO;
using Composite.Core.Logging;
using Composite.Data;
using Composite.Functions;
using System.Collections.Generic;
using Composite.Versioning.ContentVersioning.Data;
using Composite.Data.ProcessControlled;

namespace Composite.Versioning.ContentVersioning.Cleaner
{
	[ApplicationStartup]
	public class CleanerFacade
	{
		private const string _packagePath = "App_Data/Composite/Versioning/ContentVersioning";

		public static string Title { get { return "Composite.Versioning.ContentVersioning.Cleaner"; } }

		public static void OnBeforeInitialize()
		{

		}

		public static void OnInitialized()
		{


			try
			{
				int delay;

				if (!int.TryParse(ConfigurationManager.AppSettings["CompositeVersioningCleanerDelay"], out delay)) delay = 86400;

				Log.LogVerbose(CleanerFacade.Title, "Run RefreshWorkflow");

				RefreshWorkflow.Run(delay, CleanerFacade.Clean);

			}
			catch (Exception e)
			{
				Log.LogError(CleanerFacade.Title, e);
			}
		}

		public static void Clean()
		{
			var archiveList = new Dictionary<Guid, DataSourceId>();
			using (var conn = new DataConnection(PublicationScope.Unpublished))
			{
				var activities = from a in conn.Get<IActivity>()
								 join t in conn.Get<ITask>() on a.TaskId equals t.Id
								 join tt in conn.Get<ITaskTarget>() on t.TaskTargetId equals tt.Id
								 select new { a.Id, a.TaskId, a.ActivityTime, tt.TargetDataSourceId };

				var activityByTargets = (from a in activities
										 group a by a.TargetDataSourceId).ToDictionary(d => d.Key, d => d.OrderBy(a => a.ActivityTime).ToList());



				var publishingByTargets = (from t in conn.Get<ITask>()
										   join tt in conn.Get<ITaskTarget>() on t.TaskTargetId equals tt.Id
										   where t.TaskType == "Publish"
										   group t by tt.TargetDataSourceId).ToDictionary(d => d.Key, d => d.OrderBy(a => a.StartTime).ToList());



				//var targets = conn.Get<ITaskTarget>().ToDictionary(d => d.Id, d => );

				//Response.Write(targets.Count);


				Log.LogVerbose(CleanerFacade.Title, activities.Count() + "<br />");

				foreach (var activity in activities.Where(d => d.ActivityTime < DateTime.Now.AddMonths(-1)).OrderBy(d => d.ActivityTime).ToList())
				{
					var targetDataSourceId = activity.TargetDataSourceId;
					DataSourceId dataSourceId = null;
					if (DataSourceId.TryDeserialize(targetDataSourceId, out dataSourceId))
					{
						if (dataSourceId.InterfaceType.GetInterfaces().Contains(typeof(IPublishControlled)))
						{
							if (publishingByTargets.ContainsKey(targetDataSourceId))
							{
								var nextPublishing = publishingByTargets[targetDataSourceId].Where(d => d.StartTime > activity.ActivityTime).FirstOrDefault();
								var nextActivity = activityByTargets[targetDataSourceId].Where(d => d.ActivityTime > activity.ActivityTime).FirstOrDefault();
								if (nextPublishing != null && nextActivity != null && nextActivity.ActivityTime < nextPublishing.StartTime)
								{
									archiveList.Add(activity.Id, dataSourceId);
								}
							}
						}
						else
						{
							if (activityByTargets[activity.TargetDataSourceId].Any(d => d.ActivityTime > activity.ActivityTime && d.ActivityTime < activity.ActivityTime.AddHours(1)))
							{
								archiveList.Add(activity.Id, dataSourceId);
							}
						}
					}
					else
					{
						archiveList.Add(activity.Id, null);
					}
					//var data = targets.First().DataSourceId.
				}

				Log.LogVerbose(CleanerFacade.Title, archiveList.Count().ToString());
				foreach (var item in archiveList)
				{
					conn.DeleteActivity(item.Key, item.Value);

				}
			}
		}


		internal static string GetMediaFileActivityPath(Guid mediaFileId, Guid activityId)
		{
			return Path.Combine(PathUtil.Resolve(PathUtil.BaseDirectory), _packagePath, mediaFileId.ToString(), activityId.ToString());
		}
	}
}
