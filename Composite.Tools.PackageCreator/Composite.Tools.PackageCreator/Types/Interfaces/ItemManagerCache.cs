using Composite.C1Console.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Composite.Tools.PackageCreator.Types
{

	internal static class ItemManagerCache
	{
		private static Dictionary<Type, IItemManager> _itemManagerCache = new Dictionary<Type, IItemManager>();

		private static object _lock = new object();

		static ItemManagerCache()
		{
			GlobalEventSystemFacade.SubscribeToFlushEvent(OnFlush);
		}

		public static IItemManager GetItemManager(Type type)
		{
			if (type == null) throw new ArgumentNullException("type");

			IItemManager actionExecutor = null;

			if (_itemManagerCache.TryGetValue(type, out actionExecutor) == false)
			{
				object[] attributes = type.GetCustomAttributes(typeof(ItemManagerAttribute), true);

				if (attributes.Length != 0)
				{
					ItemManagerAttribute attribute = (ItemManagerAttribute)attributes[0];
					if (attribute.Type == null) throw new InvalidOperationException(string.Format("Item manger type can not be null on the action token {0}", type));
					if (typeof(IItemManager).IsAssignableFrom(attribute.Type) == false) throw new InvalidOperationException(string.Format("Item manager {0} should implement the interface {1}", attribute.Type, typeof(IItemManager)));
					actionExecutor = (IItemManager)Activator.CreateInstance(attribute.Type);
				}
				_itemManagerCache.Add(type, actionExecutor);

			}
			return actionExecutor;
		}

		private static void Flush()
		{
			_itemManagerCache = new Dictionary<Type, IItemManager>();
		}

		private static void OnFlush(FlushEventArgs args)
		{
			Flush();
		}
	}
}

