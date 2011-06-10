using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script.Services;
using System.Web.Services.Protocols;
using System.Web.Script.Serialization;
using Composite.Data;

namespace Composite.Community.Raty
{

	[WebService(Namespace = "http://www.composite.net/ns/management")]
	[ScriptService]
	public class Raty : System.Web.Services.WebService
	{
		private const string RATY_COOKIE_KEY = "Composite.Community.Raty";
		public Raty()
		{
			//Uncomment the following line if using designed components
			//InitializeComponent();
		}

		[WebMethod]
		[ScriptMethod]
		public string Rate(string score, string ratyId)
		{
			Guid ratyGuid = Guid.Parse(ratyId);
			
				using (DataConnection conn = new DataConnection())
				{
					var ratyItem = conn.Get<Results>().Where(r => r.RatyId == ratyGuid).SingleOrDefault();
					if (ratyItem != null)
					{
						ratyItem.Count += 1;
						ratyItem.TotalValue = ratyItem.TotalValue + decimal.Parse(score, System.Globalization.CultureInfo.InvariantCulture);
						conn.Update<Results>(ratyItem);
					}
					else
					{
						ratyItem = conn.CreateNew<Results>();
						ratyItem.RatyId = ratyGuid;
						ratyItem.Count = 1;
						ratyItem.TotalValue = decimal.Parse(score, System.Globalization.CultureInfo.InvariantCulture);
						conn.Add<Results>(ratyItem);
					}
					JavaScriptSerializer serializer = new JavaScriptSerializer();
					var data = new { TotalValue = ratyItem.TotalValue, Count = ratyItem.Count };

					SetCookie(ratyId);
			
					return serializer.Serialize(data);
				}
			
		}

		private static void SetCookie(string ratyId)
		{
			var httpCookie = HttpContext.Current.Request.Cookies[RATY_COOKIE_KEY];
			if (httpCookie != null)
			{
				List<string> ratyIds = httpCookie.Value.ToString().Split(',').ToList();
				if (!ratyIds.Contains(ratyId))
				{
					ratyIds.Add(ratyId);
					httpCookie.Value = string.Join(",", ratyIds.ToArray());
					httpCookie.Expires = DateTime.Now.AddDays(30);
					HttpContext.Current.Response.SetCookie(httpCookie);
				}
			}
			else
			{
				httpCookie = new HttpCookie(RATY_COOKIE_KEY);
				httpCookie.Value = ratyId;
				httpCookie.Expires = DateTime.Now.AddDays(30);
				HttpContext.Current.Response.Cookies.Add(httpCookie);
			}
		}
	}
}