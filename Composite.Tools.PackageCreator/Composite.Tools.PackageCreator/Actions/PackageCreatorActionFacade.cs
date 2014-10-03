using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Composite.C1Console.Security;
using Composite.Core.Types;
using Composite.Tools.PackageCreator.Types;

namespace Composite.Tools.PackageCreator.Actions
{
    public static class PackageCreatorActionFacade
    {
        private volatile static Dictionary<PackCategoryAttribute, Type> _categoryTypes;
        private static readonly object _categoryTypesLock = new Object();

        public static Dictionary<PackCategoryAttribute, Type> CategoryTypes
        {
            get
            {
                lock (_categoryTypesLock)
                {
                    var result = _categoryTypes;

                    if (result == null)
                    {
                        result = new Dictionary<PackCategoryAttribute, Type>();
                        foreach (Assembly assembly in new[] { Assembly.GetExecutingAssembly(), AssemblyFacade.GetAppCodeAssembly() })
                        {
                            if (assembly == null)
                            {
                                // A website may not have App_Code
                                continue;
                            }

                            foreach (var type in assembly.GetTypes())
                            {
                                if (type.GetInterfaces().Contains(typeof(IPackItem)))
                                {

                                    var category = type.GetCategoryAtribute();
                                    if (category == null)
                                        continue;

                                    if (result.Any(d => d.Key.Name == category.Name))
                                    {
                                        throw new InvalidOperationException(string.Format("Category {0} already exist", category.Name));
                                    }

                                    result.Add(category, type);

                                }
                            }
                        }

                        _categoryTypes = result;
                    }

                    return result;
                }

            }
        }


        public static IEnumerable<IPackItem> GetPackageCreatorItems(EntityToken entityToken)
        {
            foreach (var category in CategoryTypes)
            {
                var type = category.Value;
                var methodInfo = type.GetMethod("Create", new[] { typeof(EntityToken) });
                var result = methodInfo.Invoke(Type.Missing, new object[] { entityToken });
                var pcItems = result as IEnumerable<IPackItem>;
                if (pcItems == null) continue;
                foreach (var pcItem in pcItems)
                {
                    yield return pcItem;
                }
            }
        }


        public static IPackItem GetPackageCreatorItem(string category, string name)
        {
            var type = CategoryTypes.Where(d => d.Key.Name == category).Select(d => d.Value).First();

			var manager = ItemManagerCache.GetItemManager(type);
			if (manager != null)
			{
				return manager.GetItem(type, name);

			}
			else
			{
				var result = Activator.CreateInstance(type, new object[] { name });
				return (IPackItem)result;
			}
        }
    }
}
