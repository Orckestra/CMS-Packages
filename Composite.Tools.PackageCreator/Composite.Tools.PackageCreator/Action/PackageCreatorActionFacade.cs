using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Composite.Tools.PackageCreator.Types;
using Composite.C1Console.Security;
using Composite.Core.Types;

namespace Composite.Tools.PackageCreator
{
	public static class PackageCreatorActionFacade
	{
		private static Dictionary<PCCategoryAttribute, Type> _categoryTypes;
		private static readonly object _categoryTypesLock = new Object();
		public static Dictionary<PCCategoryAttribute, Type> CategoryTypes
		{
			get
			{
				lock(_categoryTypesLock)
				{
					if (_categoryTypes == null)
					{
						_categoryTypes = new Dictionary<PCCategoryAttribute, Type>();
						foreach (Assembly assembly in new[] { Assembly.GetExecutingAssembly(), AssemblyFacade.GetAppCodeAssembly() })
						{

							foreach (var type in assembly.GetTypes())
							{
								if (type.GetInterfaces().Contains(typeof(IPackageCreatorItem)))
								{

									var category = type.GetCategoryAtribute();
									if (category == null)
										continue;
									if (_categoryTypes.Where(d => d.Key.Name == category.Name).Any()) throw new InvalidOperationException(string.Format("Category {0} already exist", category.Name));

									_categoryTypes.Add(category, type);

								}
							}
						}
					}
					return _categoryTypes;
				}
				
			}
		}


		public static IEnumerable<IPackageCreatorItem> GetPackageCreatorItems(EntityToken entityToken)
		{
			foreach (var category in CategoryTypes)
			{
				var type = category.Value;
				var methodInfo = type.GetMethod("Create", new[]{ typeof(EntityToken)});
				var result = methodInfo.Invoke(Type.Missing, new object[] { entityToken });
				var pcItems = result as IEnumerable<IPackageCreatorItem>;
				if (pcItems == null) continue;
				foreach (var pcItem in pcItems)
				{
					yield return pcItem;
				}
			}
		}


		public static IPackageCreatorItem GetPackageCreatorItem(string category, string name)
		{
			var type = CategoryTypes.Where(d => d.Key.Name == category).Select(d => d.Value).First();
			
			var result = Activator.CreateInstance(type, new object[]{name});
			return (IPackageCreatorItem)result;
		}


/*
		private static ElementAction GetAction()
		{
			return new ElementAction(new ActionHandle(new PackageCreatorActionToken("")))
			{
				VisualData = new ActionVisualizedData
				{
					Label = "Add to the package",
					ToolTip = "Add this item to the package",
					Icon = new ResourceHandle("Composite.Icons", "package-element-closed-availableitem"),
					ActionLocation = new ActionLocation
					{
						ActionType = PackageCreatorFacade.ActionType,
						IsInFolder = false,
						IsInToolbar = false,
						ActionGroup = new ActionGroup("Develop", ActionGroupPriority.PrimaryLow)
					}
				}
			};
		}
*/

		/// <summary>
		/// Return list category items
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static IEnumerable<T> GetItems<T>() where T : class, IPackageCreatorItem
		{
			return GetItems(typeof(T)).Cast<T>();
		}

		/// <summary>
		/// Return list category items
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static IEnumerable<IPackageCreatorItem> GetItems(Type type)
		{
			if (type == null) throw new ArgumentNullException("type");
			return null;
		}

		
	}
}
