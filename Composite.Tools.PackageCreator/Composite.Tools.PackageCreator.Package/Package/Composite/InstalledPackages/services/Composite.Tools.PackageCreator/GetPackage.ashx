<%@ WebHandler Language="C#" Class="GetPackage" %>

using System;
using System.IO;
using System.Web;
using Composite.C1Console.Security;
using Composite.Tools.PackageCreator;
using Composite.C1Console.Events;
using Composite.Core.Logging;


public class GetPackage : IHttpHandler
{
	public void ProcessRequest(HttpContext context)
	{
		if (!UserValidationFacade.IsLoggedIn())
		{
			throw new Exception("Permission denied");
		}

		var configName = context.Request["config"];
		var packageName = context.Request["package"];

		var consoleId = context.Request["consoleId"];
		var viewId = context.Request["viewId"];

		string fullPath = null;

		if (configName != null)
		{
			fullPath = PackageCreatorFacade.GetPackageConfigPath(configName);
		}
		else if (packageName != null)
		{
			try{
				fullPath = PackageCreatorFacade.CreatePackage(packageName);
			}
			catch(Exception e)
			{
				ConsoleMessageQueueFacade.Enqueue(new MessageBoxMessageQueueItem
				{
					DialogType = DialogType.Error,
					Message = e.Message,
					Title = "Package creation failed"
				}, 
				consoleId);
				LoggingService.LogError("Composite.Tools.PackageCreator",e);
				throw new Exception("Package creation failed", e);
			}
		}
		else
		{
			throw new Exception("Parameters empty");
		}

		if (File.Exists(fullPath))
		{
			context.Response.AddHeader("Content-Disposition", "attachment;filename=" + Path.GetFileName(fullPath));
            context.Response.ContentType = "application/zip";
			context.Response.WriteFile(fullPath);
		}
		else
		{
			throw new Exception("Package doesn't exists");
		}
	}

	public bool IsReusable => false;
}