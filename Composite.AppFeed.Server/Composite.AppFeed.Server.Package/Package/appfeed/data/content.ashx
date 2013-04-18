<%@ WebHandler Language="C#" Class="AppFeed.ContentFeed" %>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Web;
using Composite.AppFeed.Server;

namespace AppFeed
{
	public class ContentFeed : IHttpHandler
	{
		public void ProcessRequest(HttpContext context)
		{
			context.Response.ContentType = "application/json";

			var content = FeedManager.Instance.Content.ToList();
			DataContractJsonSerializer ser = new DataContractJsonSerializer(content.GetType());
			ser.WriteObject(context.Response.OutputStream, content);
		}

		public bool IsReusable
		{
			get { return false; }
		}
	}
}