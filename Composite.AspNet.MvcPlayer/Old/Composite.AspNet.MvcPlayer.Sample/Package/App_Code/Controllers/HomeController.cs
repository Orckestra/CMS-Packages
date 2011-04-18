using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcWebsite.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewData["Title"] = "Home Page";
            ViewData["Message"] = "Welcome to ASP.NET MVC!";

            return View();
        }

        public ActionResult About()
        {
            ViewData["Title"] = "About Page";

            return View();
        }

		public ActionResult UpdateStatus()
		{
			return Json(new { success = true }, JsonRequestBehavior.AllowGet);
		}



		public string GetStatus()
		{
			return "Status OK at " + DateTime.Now.ToLongTimeString();
		}

		public string UpdateForm(string textBox1)
		{
			if (textBox1 != "Enter text")
			{
				return "You entered: \"" + textBox1.ToString() + "\" at " +
					DateTime.Now.ToLongTimeString();
			}

			return String.Empty;
		}

    }
}
