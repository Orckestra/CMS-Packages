using System.Linq;
using System.Reflection;
using Composite.Data;

namespace Composite.Tools.StaticDataTypeCreator
{
	public static class Reflection
	{
		public static T CallStaticMethod<T>(string typeName, string methodName, params object[] parameters)
		{
			var type = typeof(IData).Assembly.GetType(typeName);
			var methodInfos = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Static);
			var methodInfo = methodInfos.First(m => m.Name == methodName && !m.IsGenericMethod);
			return (T)methodInfo.Invoke(null, parameters);
		}

		public static T CallMethod<T>(this object o, string methodName, params object[] parameters)
		{
			var type = o.GetType();
			var methodInfos = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
			var methodInfo = methodInfos.First(m => m.Name == methodName && !m.IsGenericMethod);
			return (T)methodInfo.Invoke(o, parameters);
		}
	}
}
