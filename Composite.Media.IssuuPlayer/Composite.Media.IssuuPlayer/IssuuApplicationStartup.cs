using System.Linq;
using Composite.Core.Application;
using Composite.Data;

namespace Composite.Media.IssuuPlayer
{
	[ApplicationStartup]
	public sealed class IssuuApplicationStartup
	{
		public static object _lock = new object();

		public static void OnBeforeInitialize()
		{
		}

		public static void OnInitialized()
		{
			DataEventSystemFacade.SubscribeToDataBeforeAdd<ApiKey>(OnAddApiKey);
		}

		private static void OnAddApiKey(object sender, DataEventArgs args)
		{
			lock (_lock)
			{
				if (!DataFacade.GetData<ApiKey>().Any())
				{
					(args.Data as ApiKey).Default = true;
				}
			}
		}
	}
}
