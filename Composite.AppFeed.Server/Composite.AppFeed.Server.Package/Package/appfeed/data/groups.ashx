<%@ WebHandler Language="C#" Class="AppFeed.GroupFeed" %>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Web;
using Composite.AppFeed.Server;

namespace AppFeed
{
	public class GroupFeed : IHttpHandler
	{

		public void ProcessRequest(HttpContext context)
		{
			context.Response.ContentType = "application/json";

			var groups = FeedManager.Instance.Groups.ToList();
			DataContractJsonSerializer ser = new DataContractJsonSerializer(groups.GetType());
			ser.WriteObject(context.Response.OutputStream, groups);
		}

		public bool IsReusable
		{
			get
			{
				return false;
			}
		}
	}
}