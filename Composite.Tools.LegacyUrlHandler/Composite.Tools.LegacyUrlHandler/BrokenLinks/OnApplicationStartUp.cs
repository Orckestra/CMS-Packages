using System;
using System.Globalization;
using System.Threading;
using Composite.Core.Application;
using Composite.C1Console.Events;
using Composite.Core.Threading;
using Composite.Core;
using System.IO;
using System.Web.Hosting;
using Composite.Core.IO;

namespace Composite.Tools.LegacyUrlHandler.BrokenLinks
{
	[ApplicationStartup]
	public class OnApplicationStartUp
	{
		private static Exception _exception;
		private static Timer _timer;
		private static string _lastRunDateTimeFilePath = "~/App_Data/Composite.Tools.LegacyUrlHandler/send404mailsDateTime.txt";


		public static void OnBeforeInitialize()
		{

		}

		public static void OnInitialized()
		{
			var hours = int.Parse(Config.SendEveryNHours);
			Log.LogInformation("Composite.Tools.LegacyUrlHandler", string.Format("Send404Mails Thread Timer Started(send emails every {0} hour)", Config.SendEveryNHours));

			_timer = new Timer(Run, null, 60000, 900000);
			GlobalEventSystemFacade.SubscribeToPrepareForShutDownEvent(RunOnShutDown);
			
			Functions.DeleteOldBrokenLinks();
		}

		public static void Run(object data)
		{
			using (ThreadDataManager.EnsureInitialize())
			{
				try
				{
					if (NeedToRun())
					{
						Functions.Send404MailReport();
					}
				}
				catch (Exception ex)
				{
					_exception = ex;
					Log.LogError("Composite.Tools.LegacyUrlHandler Send404Mails Thread", ex);
				}
			}
		}

		public static void RunOnShutDown(object data)
		{
			if (!NeedToRun()) return;
			Log.LogInformation("Composite.Tools.LegacyUrlHandler", "Send404Mails on shutdown");
			Functions.Send404MailReport();
		}

		private static bool NeedToRun()
		{
			var checkLastRunTimeFilePath = HostingEnvironment.MapPath(_lastRunDateTimeFilePath) ?? string.Empty;
			if (!C1File.Exists(checkLastRunTimeFilePath))
			{
				C1File.WriteAllText(checkLastRunTimeFilePath, DateTime.Now.ToString(CultureInfo.InvariantCulture));
				return true;
			}
			var lastrunDateTime = DateTime.Parse(File.ReadAllText(checkLastRunTimeFilePath), CultureInfo.InvariantCulture);
			var now = DateTime.Now;
			if (lastrunDateTime.AddHours(double.Parse(Config.SendEveryNHours)) < now)
			{
				C1File.WriteAllText(checkLastRunTimeFilePath, DateTime.Now.ToString(CultureInfo.InvariantCulture));
				return true;
			}
			return false;
		}
	}
}


