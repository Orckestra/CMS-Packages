using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Xml.Linq;
using Composite.C1Console.Workflow;
using Composite.Core.Application;
using Composite.Core.IO;
using Composite.Core.Logging;
using System.Text;

namespace Composite.Community.Twitter.FeedAggregator
{
	[ApplicationStartup]
	public class FeedAggregatorFacade
	{
		internal static XElement Feed { get; set; }

		public static XElement GetFeed()
		{
			return Feed;
		}

		public static void OnBeforeInitialize()
		{

		}

		public static void OnInitialized()
		{
			Composite.GlobalInitializerFacade.InitializeTheSystem();
			try
			{
				var config = XElement.Load(PathUtil.Resolve("~/App_Data/Composite.Community.Twitter.FeedAggregator/Config.xml"));
				int delay;
				if (int.TryParse(config.AttributeValue("autoRefresh"), out delay))
				{
					var queries = config.Elements("Search").Select(d => d.AttributeValue("term")).NotNull().ToList();
					var refreshWorkflow = WorkflowFacade.CreateNewWorkflow(
						typeof(RefreshWorkflow),
						new Dictionary<string, object> {
							{ "Delay", delay },
							{ "Queries", queries }
						}
					);

					refreshWorkflow.Start();
					LoggingService.LogInformation("FeedAggregator", "Run RefreshWorkflow");
					WorkflowFacade.RunWorkflow(refreshWorkflow);
				}
				else
				{
					throw new InvalidOperationException("Wrong autoRefresh config value");
				}

			}
			catch (Exception e)
			{
				LoggingService.LogError("FeedAggregator", e);
			}
		}

		public static void UpdateFeeds(List<string> queries)
		{
			var entrys = new Dictionary<string, XElement>();
			XNamespace atom = "http://www.w3.org/2005/Atom";
			XNamespace twitter = "http://api.twitter.com/";

			foreach (var query in queries)
			{
				XElement queryFeed = null;
				try
				{
					queryFeed = GetQueryFeed(query);
				}
				catch (WebException e)
				{
					LoggingService.LogWarning("FeedAggregator", e);
					return;
				}
				catch (Exception e)
				{
					LoggingService.LogError("FeedAggregator", e);
				}
				if (queryFeed != null)
				{
					foreach (var entry in queryFeed.Elements(atom + "entry"))
					{
						entrys[entry.ElementValue(atom + "id")] = entry;
					}
				}
			};

			var feed = new XElement(atom + "feed",
				entrys.Values.OrderByDescending(d => d.ElementValue(atom + "updated"))
			);
			feed.HtmlfyNodes(twitter + "source");
			feed.HtmlfyNodes(atom + "content");
			Feed = feed;
		}

		private static XElement GetQueryFeed(string query)
		{
			if (query != string.Empty)
			{
				var feedUrl = string.Format("http://search.twitter.com/search.atom?q={0}", HttpUtility.UrlPathEncode(query));
				var webClient = new WebClient();
				var feed = XElement.Load(webClient.OpenRead(feedUrl));
				return feed;
			}
			return null;
		}
	}
}
