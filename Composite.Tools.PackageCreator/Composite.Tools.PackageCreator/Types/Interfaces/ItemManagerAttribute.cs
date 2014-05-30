using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Composite.Tools.PackageCreator.Types
{
	public sealed class ItemManagerAttribute : Attribute
	{

		private Type _type;

		/// <exclude />
		public ItemManagerAttribute(Type type)
		{
			_type = type;
		}

		/// <exclude />
		public Type Type
		{
			get { return _type; }
		}

	}
}
