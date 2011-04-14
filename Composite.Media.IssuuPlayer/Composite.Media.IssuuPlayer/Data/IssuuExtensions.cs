using System.Reflection;
using Composite.Data;

namespace Composite.Media.IssuuPlayer.Data
{
	public static class IssuuExtensions
	{
		public static T GetProperty<T>(this IData data, string name)
		{
			var propertyInfo = data.GetType().GetProperty(name);
			if (propertyInfo == null)
			{
				return default(T);
			}
			MethodInfo getMethodInfo = propertyInfo.GetGetMethod();
			return (T)getMethodInfo.Invoke(data, null);
		}

		public static void SetProperty<T>(this IData data, string name, T value)
		{
			var propertyInfo = data.GetType().GetProperty(name);
			if (propertyInfo == null)
			{
				return;
			}
			MethodInfo setMethodInfo = propertyInfo.GetSetMethod();
			setMethodInfo.Invoke(data, new object[] { value });
			return;
		}
	}
}
