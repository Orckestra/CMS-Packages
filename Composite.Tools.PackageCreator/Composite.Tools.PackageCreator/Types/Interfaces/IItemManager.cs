using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Composite.Tools.PackageCreator.Types
{
	public interface IItemManager
	{
		IEnumerable<IPackageCreatorItem> GetItems(Type type, XElement config);

		IPackageCreatorItem GetItem(Type type, string id);
	}
}
