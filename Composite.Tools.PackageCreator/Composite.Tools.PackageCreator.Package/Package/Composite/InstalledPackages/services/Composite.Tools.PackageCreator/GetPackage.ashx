<%@ WebHandler Language="C#" Class="GetPackage" %>

using System;
using System.IO;
using System.Web;
using System.Linq;
using Composite.Core.IO;
using Composite.C1Console.Security;
using Composite.Tools.PackageCreator;
using Composite.C1Console.Events;
using Composite.Core.Logging;


public class GetPackage : IHttpHandler
{
	public void ProcessRequest(HttpContext context)
	{
		if (!PackageCreatorFacade.IsHaveAccess)
		{
			throw new Exception("Premission denied");
		}

		string configName = context.Request["config"];
		string packageName = context.Request["package"];

		string consoleId = context.Request["consoleId"];
		string viewId = context.Request["viewId"];

		string fullpath = null;

		if (configName != null)
		{
			fullpath = PackageCreatorFacade.GetPackageConfigPath(configName);
		}
		else if (packageName != null)
		{
			try{
				fullpath = PackageCreatorFacade.CreatePackage(packageName);
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

		if (File.Exists(fullpath))
		{
			context.Response.AddHeader("Content-Disposition", "attachment;filename=" + Path.GetFileName(fullpath));
            context.Response.ContentType = "application/zip";
			context.Response.WriteFile(fullpath);
		}
		else
		{
			throw new Exception("Package doesn't exists");
		}
	}

	public bool IsReusable
	{
		get
		{
			return false;
		}
	}

}