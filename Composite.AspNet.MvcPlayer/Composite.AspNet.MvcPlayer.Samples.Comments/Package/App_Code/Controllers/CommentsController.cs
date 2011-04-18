using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Composite.Core.WebClient.Renderings.Page;

/// <summary>
/// Summary description for CommentsController
/// </summary>
public class CommentsController : Controller
{
	public ActionResult Index()
	{
		ViewData["Title"] = "Home Page";
		ViewData["Message"] = "Welcome to ASP.NET MVC!";
		ViewData["Comments"] = Comments.Instanse.GetComments(PageRenderer.CurrentPageId);
		return View();
	}

	public ActionResult AddComment(string name, string text)
	{
		Comments.Instanse.AddComment(name, text, PageRenderer.CurrentPageId);
		var comments = Comments.Instanse.GetComments(PageRenderer.CurrentPageId);
		return PartialView("Comments", comments);
	}
}
