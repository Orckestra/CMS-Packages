using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Linq;
using Composite.Tools.PackageCreator.Types;
using System.IO;

namespace Composite.Tools.PackageCreator
{
	public static class Extension
	{
		public static void SaveTabbed(this XDocument doc, string fileName)
		{
			XmlTextWriter writer = new XmlTextWriter(fileName, null);
			writer.Formatting = Formatting.Indented;
			writer.Indentation = 1;
			writer.IndentChar = '\t';
			doc.Save(writer);
			writer.Close();
		}

		public static Type Get(this Dictionary<PCCategoryAttribute, Type> categoryTypes, string name)
		{
			return categoryTypes.Where(d => d.Key.Name == name).Select(d => d.Value).First();
		}

		

		public static PCCategoryAttribute GetCategoryAtribute(this Type type)
		{
			object[] attributes = type.GetCustomAttributes(typeof(PCCategoryAttribute), true);

			if (attributes.Length == 0) return null;

			PCCategoryAttribute category = (PCCategoryAttribute)attributes[0];
			return category;
			
		}

		//public static PCCategoryAttribute GetCategoryAtribute(this IPackageCreatorItem item)
		//{
		//    item.GetType().GetCategoryAtribute();
		//}

		//public static IEnumerable<string> GetCategoryNamesAtribute(this Type type)
		//{
		//    var categoryAttribute = type.GetCategoryAtribute();
		//    if(!string.IsNullOrWhiteSpace(categoryAttribute.CommonName))
		//    {
		//        categoryAttribute.CommonName;
		//    }
		//    yield return categoryAttribute.Name;
		//}


		//public static IEnumerable<string> GetCategoryNamesAtribute(this IPackageCreatorItem item)
		//{
		//    return item.GetType().GetCategoryNamesAtribute();
		//}

		public static string GetCategoryName(this Type type)
		{
			var categoryAttribute = type.GetCategoryAtribute();
			return categoryAttribute.Name;
		}

		public static string GetCategoryName2(this IPackageCreatorItem item)
		{
			return item.GetType().GetCategoryName();
		}

		public static string[] GetCategoryAllNames(this Type type)
		{
			var categoryAttribute = type.GetCategoryAtribute();
			if (categoryAttribute.AliasNames == null)
				return new string[] { categoryAttribute.Name };
			else
				return new string[] { categoryAttribute.Name }.Concat(categoryAttribute.AliasNames).ToArray();
		}

		public static string[] GetCategoryAllNames2(this IPackageCreatorItem item)
		{
			return item.GetType().GetCategoryAllNames();
		}

		public static string GetProperty(this object o, string propertyName)
		{
			try
			{
				var property = o.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				return property.GetValue(o, null).ToString();
			}
			catch { }
			return null;

		}
		public static T GetProperty<T>(this object o, string propertyName)
		{
			try
			{
				var property = o.GetType().GetProperty(propertyName);
				return (T)property.GetValue(o, null);
			}
			catch { }
			return default(T);
		}

		public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source)
		{
			return new HashSet<T>(source);
		}

		public static void Add<TKey, TValue>(this Dictionary<TKey, HashSet<TValue>> dictionary, TKey key, TValue value)
		{
			if (!dictionary.ContainsKey(key))
				dictionary[key] = new HashSet<TValue>();
			dictionary[key].Add(value);
		}

		public static bool IsBinFolder(this Assembly assembly)
		{
			return !assembly.IsDynamic && Path.GetFileName(Path.GetDirectoryName(assembly.CodeBase)).ToLower().Equals("bin");
		}

		public static void Add(this Dictionary<string, Dictionary<string, XElement>> dictionary, string key, string listKey, XElement listValue)
		{
			if(!dictionary.ContainsKey(key))
				dictionary[key] = new Dictionary<string,XElement>();
			dictionary[key][listKey] = listValue;
		}

		public static bool IsIPackagable(this Type type)
		{
			return type.GetInterfaces().Contains(typeof (IPackagable));
		}
	}
}
