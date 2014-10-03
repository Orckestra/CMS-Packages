using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Composite.Tools.PackageCreator.Types
{
	public interface IPackItemManager
	{
		IEnumerable<IPackItem> GetItems(Type type, XElement config);

		IPackItem GetItem(Type type, string id);
	}
}
