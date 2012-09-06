using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Composite.Core.ResourceSystem;
using Composite.C1Console.Security;

namespace Composite.Tools.PackageCreator.Types
{
	public interface IPackageCreatorItem
	{
		string Name
		{
			get;
		}

		string ActionLabel
		{
			get;
		}

		string ActionToolTip
		{
			get;
		}

		ResourceHandle ItemIcon
		{
			get;
		}

		ResourceHandle ActionIcon
		{
			get;
		}

		string CategoryName { get; }

		string[] CategoryAllNames { get; }

		string GetLabel();

		void AddToConfiguration(XElement config);
		void RemoveFromConfiguration(XElement config);
		
	}
}
