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
			var writer = new XmlTextWriter(fileName, null)
			{
				Formatting = Formatting.Indented,
				Indentation = 1,
				IndentChar = '\t'
			};
			doc.Save(writer);
			writer.Close();
		}

		public static Type Get(this Dictionary<PackCategoryAttribute, Type> categoryTypes, string name)
		{
			return categoryTypes.Where(d => d.Key.Name == name).Select(d => d.Value).First();
		}

		

		public static PackCategoryAttribute GetCategoryAtribute(this Type type)
		{
			object[] attributes = type.GetCustomAttributes(typeof(PackCategoryAttribute), true);

			if (attributes.Length == 0) return null;

			var category = (PackCategoryAttribute)attributes[0];
			return category;
			
		}

		public static string GetCategoryName(this Type type)
		{
			var categoryAttribute = type.GetCategoryAtribute();
			return categoryAttribute.Name;
		}

		public static string GetCategoryName(this IPackItem item)
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

		public static string[] GetCategoryAllNames(this IPackItem item)
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

		public static void Add<TKey, TValue>(this Dictionary<TKey, List<TValue>> dictionary, TKey key, TValue value)
		{
			if (!dictionary.ContainsKey(key))
				dictionary[key] = new List<TValue>();
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

		public static bool IsInitable(this Type type)
		{
			return type.GetInterfaces().Contains(typeof(IPackInit));
		}

		public static bool IsPackagable(this Type type)
		{
			return type.GetInterfaces().Contains(typeof (IPack));
		}
	}
}
