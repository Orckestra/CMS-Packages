using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Web.Mvc;

namespace HelloWorld.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			ViewData["ToGreet"] = "World";
			return View();
		}

		public ActionResult Ajax()
		{
			return View();
		}

		public string GetDate()
		{
			return DateTime.Now.ToLongTimeString();
		}

		public string SetText(string text)
		{
			return text + DateTime.Now.ToLongTimeString();
		}
	}
}